using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

using CarTrickRush.UI.Common;

using System.Collections.Generic;
using System.Threading;

using Cysharp.Threading.Tasks;

using CarTrickRush.Data;
using CarTrickRush.UI.SceneTransition;

namespace CarTrickRush.Managers
{
    /// =========================================================================================
    /// <summary>
    /// シーン遷移管理Manager.
    /// </summary>
    /// =========================================================================================
    public sealed class SceneLoadManager : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// ルールフェード用カタログ.
        /// </summary>
        [SerializeField] private SceneTransitionCatalog _sceneTransitionCatalog = default;

        /// <summary>
        /// ルールフェードのオーバーレイ色.
        /// </summary>
        [SerializeField] private Color _transitionOverlayColor = Color.black;

        /// <summary>
        /// Cover で全面が黒になったあと、シーン読み込みを始める前に待つ時間（秒、unscaled）.
        /// </summary>
        [SerializeField] private float _minFullBlackHoldDuration = 0.1f;

        /// <summary>
        /// シングル遷移のルールフェード実行中.
        /// </summary>
        private bool _singleLoadTransitionRunning = default;

        /// <summary>
        /// ルールフェード用オーバーレイ.
        /// </summary>
        private SceneRuleFadeOverlay _ruleFadeOverlay = default;

        /// <summary>
        /// 加算シーンごとに無効化した EventSystem の復元用（LIFO）.
        /// </summary>
        private readonly Stack<List<EventSystem>> _disabledEventSystemLayers = new Stack<List<EventSystem>>();

        /// <summary>
        /// 加算読み込み時にゲートを積んだか（LIFO）.
        /// </summary>
        private readonly Stack<bool> _inputGateLayers = new Stack<bool>();

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// シングル遷移が進行中か.
        /// </summary>
        public bool IsSingleLoadTransitionRunning => _singleLoadTransitionRunning;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            ResolveSceneTransitionCatalog();
        }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// Boot から注入したシーン遷移カタログを適用する（非 null のときのみ上書き）.
        /// </summary>
        /// <param name="catalog">カタログ.</param>
        public void ApplyBootstrapSceneTransitionCatalog(SceneTransitionCatalog catalog)
        {
            if (catalog == null) { return; }

            _sceneTransitionCatalog = catalog;
        }

        /// <summary>
        /// 指定したシーンへ遷移する.
        /// </summary>
        /// <param name="sceneName">遷移先シーン名.</param>
        /// <param name="transitionSetId">ルールフェードのセットID.</param>
        public void LoadScene(string sceneName, int transitionSetId = -1)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("SceneLoadManager.LoadScene failed. sceneName is null or empty.");
                return;
            }

            if (_singleLoadTransitionRunning)
            {
                Debug.LogWarning("SceneLoadManager.LoadScene: 別のシングル遷移が進行中のため無視しました.");
                return;
            }

            if (transitionSetId < 0)
            {
                ClearAdditiveOverlayStateForSingleLoad();
                SceneManager.LoadScene(sceneName);
                return;
            }

            ResolveSceneTransitionCatalog();
            var catalog = _sceneTransitionCatalog;
            if (catalog == null)
            {
                Debug.LogWarning(
                    "SceneLoadManager.LoadScene: SceneTransitionCatalog が null のためフェードをスキップしました. " +
                    $"BootScene の Bootstrap / BootSceneDebug の BootstrapDebug にカタログをアサインするか、Resources 配下へ「{SceneTransitionCatalog.ResourcesAssetName}.asset」があるか確認してください.");
                ClearAdditiveOverlayStateForSingleLoad();
                SceneManager.LoadScene(sceneName);
                return;
            }

            if (!catalog.TryGet(transitionSetId, out var entry))
            {
                Debug.LogWarning(
                    $"SceneLoadManager.LoadScene: transitionSetId={transitionSetId} がカタログに無いためフェードをスキップしました. " +
                    "カタログの Sets に同じ ID のエントリがあるか確認してください.");
                ClearAdditiveOverlayStateForSingleLoad();
                SceneManager.LoadScene(sceneName);
                return;
            }

            LoadSceneWithRuleFadeAsync(sceneName, entry, destroyCancellationToken).Forget();
        }

        /// <summary>
        /// 指定したシーンを加算読み込みする.
        /// </summary>
        /// <param name="sceneName">読み込むシーン名.</param>
        /// <param name="blockUnderlyingInput">true のとき、元シーンの UI（EventSystem）とゲームプレイ入力を無効にする.</param>
        public void LoadSceneAdditive(string sceneName, bool blockUnderlyingInput = true)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("SceneLoadManager.LoadSceneAdditive failed. sceneName is null or empty.");
                return;
            }

            if (IsSceneLoaded(sceneName))
            {
                return;
            }

            LoadSceneAdditiveAsync(sceneName, blockUnderlyingInput, destroyCancellationToken).Forget();
        }

        /// <summary>
        /// 指定した加算シーンをアンロードする.
        /// </summary>
        /// <param name="sceneName">アンロードするシーン名.</param>
        public void UnloadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("SceneLoadManager.UnloadScene failed. sceneName is null or empty.");
                return;
            }

            if (!IsSceneLoaded(sceneName)) { return; }

            UnloadSceneAsync(sceneName, destroyCancellationToken).Forget();
        }

        /// <summary>
        /// シーン読込済みか判定する.
        /// </summary>
        /// <param name="sceneName">判定対象シーン名.</param>
        /// <returns>読込済みか.</returns>
        public static bool IsSceneLoaded(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                return false;
            }

            var scene = SceneManager.GetSceneByName(sceneName);
            return scene.IsValid() && scene.isLoaded;
        }

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// カタログ参照を解決する.
        /// </summary>
        private void ResolveSceneTransitionCatalog()
        {
            if (_sceneTransitionCatalog != null) { return; }

            _sceneTransitionCatalog = CarTrickRush.DataLoading.Data.Load<SceneTransitionCatalog>(SceneTransitionCatalog.ResourcesAssetName);
        }

        /// <summary>
        /// ルールフェード用オーバーレイを確保する.
        /// </summary>
        /// <returns>オーバーレイ.</returns>
        private SceneRuleFadeOverlay EnsureRuleFadeOverlay()
        {
            if (_ruleFadeOverlay != null)
            {
                return _ruleFadeOverlay;
            }

            var gameObject = new GameObject("SceneTransitionOverlay");
            gameObject.transform.SetParent(transform, false);
            _ruleFadeOverlay = gameObject.AddComponent<SceneRuleFadeOverlay>();
            _ruleFadeOverlay.EnsureBuilt(_transitionOverlayColor);
            return _ruleFadeOverlay;
        }

        /// <summary>
        /// ルールフェード付きシングルシーン読み込み.
        /// </summary>
        /// <param name="sceneName">遷移先シーン名.</param>
        /// <param name="entry">カタログエントリ.</param>
        /// <param name="cancellationToken">キャンセルトークン.</param>
        private async UniTaskVoid LoadSceneWithRuleFadeAsync(
            string sceneName,
            SceneTransitionSetEntry entry,
            CancellationToken cancellationToken)
        {
            _singleLoadTransitionRunning = true;
            ClearAdditiveOverlayStateForSingleLoad();

            var overlay = EnsureRuleFadeOverlay();
            overlay.EnsureBuilt(_transitionOverlayColor);
            if (!overlay.IsReady)
            {
                Debug.LogError("SceneLoadManager: ルールフェード用シェーダーが無効のため即時遷移します.");
                SceneManager.LoadScene(sceneName);
                _singleLoadTransitionRunning = false;
                return;
            }

            overlay.Configure(entry.FadeOutMask, entry.Softness);
            overlay.SetProgress(0f);
            overlay.Show();

            await overlay.AnimateProgressAsync(0f, 1f, entry.CoverDuration, cancellationToken);

            var holdBeforeLoad = Mathf.Max(0f, _minFullBlackHoldDuration);
            if (holdBeforeLoad > 0f)
            {
                await UniTask.Delay(
                    System.TimeSpan.FromSeconds(holdBeforeLoad),
                    ignoreTimeScale: true,
                    cancellationToken: cancellationToken);
            }

            var asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            if (asyncOperation == null)
            {
                Debug.LogError($"SceneLoadManager.LoadSceneWithRuleFadeAsync failed. sceneName:{sceneName}");
                overlay.Hide();
                _singleLoadTransitionRunning = false;
                return;
            }

            await WaitForAsyncOperation(asyncOperation, cancellationToken);
            await UniTask.Yield(cancellationToken);

            overlay.Configure(entry.FadeInMask, entry.Softness);
            overlay.SetProgress(1f);
            await overlay.AnimateProgressAsync(1f, 0f, entry.RevealDuration, cancellationToken);
            overlay.Hide();

            _singleLoadTransitionRunning = false;
        }

        /// <summary>
        /// 加算読み込み.
        /// </summary>
        /// <param name="sceneName">読み込むシーン名.</param>
        /// <param name="blockUnderlyingInput">元シーンの操作を無効にするか.</param>
        /// <param name="cancellationToken">キャンセルトークン.</param>
        private async UniTaskVoid LoadSceneAdditiveAsync(
            string sceneName,
            bool blockUnderlyingInput,
            CancellationToken cancellationToken)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            if (asyncOperation == null)
            {
                Debug.LogError($"SceneLoadManager.LoadSceneAdditiveAsync failed. sceneName:{sceneName}");
                return;
            }

            await WaitForAsyncOperation(asyncOperation, cancellationToken);

            if (!blockUnderlyingInput)
            {
                _disabledEventSystemLayers.Push(new List<EventSystem>());
                _inputGateLayers.Push(false);
                return;
            }

            var overlayScene = SceneManager.GetSceneByName(sceneName);
            if (!overlayScene.IsValid())
            {
                Debug.LogError($"SceneLoadManager.LoadSceneAdditiveAsync: scene not found after load. sceneName:{sceneName}");
                return;
            }

            var disabled = new List<EventSystem>();
            var systems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
            for (var index = 0; index < systems.Length; index++)
            {
                var eventSystem = systems[index];
                if (eventSystem == null || eventSystem.gameObject.scene == overlayScene)
                {
                    continue;
                }

                if (eventSystem.GetComponent<PersistentEventSystem>() != null)
                {
                    continue;
                }

                if (eventSystem.enabled)
                {
                    eventSystem.enabled = false;
                    disabled.Add(eventSystem);
                }
            }

            _disabledEventSystemLayers.Push(disabled);
            AdditiveOverlayInputGate.Push();
            _inputGateLayers.Push(true);
        }

        /// <summary>
        /// シーンアンロード.
        /// </summary>
        /// <param name="sceneName">アンロードするシーン名.</param>
        /// <param name="cancellationToken">キャンセルトークン.</param>
        private async UniTaskVoid UnloadSceneAsync(string sceneName, CancellationToken cancellationToken)
        {
            var asyncOperation = SceneManager.UnloadSceneAsync(sceneName);

            if (asyncOperation == null)
            {
                return;
            }

            await WaitForAsyncOperation(asyncOperation, cancellationToken);

            RestoreOneAdditiveOverlayLayer();
        }

        /// <summary>
        /// AsyncOperation の完了を待つ.
        /// </summary>
        /// <param name="asyncOperation">非同期操作.</param>
        /// <param name="cancellationToken">キャンセルトークン.</param>
        private static async UniTask WaitForAsyncOperation(AsyncOperation asyncOperation, CancellationToken cancellationToken)
        {
            while (!asyncOperation.isDone)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await UniTask.Yield(cancellationToken);
            }
        }

        /// <summary>
        /// 直近の加算シーンに対応する入力／EventSystem ブロックを解除する.
        /// </summary>
        private void RestoreOneAdditiveOverlayLayer()
        {
            if (_inputGateLayers.Count == 0) { return; }

            var hadInputGate = _inputGateLayers.Pop();
            if (hadInputGate)
            {
                AdditiveOverlayInputGate.Pop();
            }

            if (_disabledEventSystemLayers.Count > 0)
            {
                var disabled = _disabledEventSystemLayers.Pop();
                for (var index = 0; index < disabled.Count; index++)
                {
                    var eventSystem = disabled[index];
                    if (eventSystem != null)
                    {
                        eventSystem.enabled = true;
                    }
                }
            }
        }

        /// <summary>
        /// シングルシーン読み込みで加算シーンがまとめて破棄される場合、スタックと入力ゲートをリセットする.
        /// </summary>
        private void ClearAdditiveOverlayStateForSingleLoad()
        {
            _disabledEventSystemLayers.Clear();
            _inputGateLayers.Clear();
            AdditiveOverlayInputGate.ResetDepth();
        }

        #endregion
    }
}