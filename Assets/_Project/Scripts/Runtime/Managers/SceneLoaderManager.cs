using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

using System.Collections;
using System.Collections.Generic;

using CarTrickRush.Core;
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
        /// インスタンス.
        /// </summary>
        private static SceneLoadManager _instance = default;

        /// <summary>
        /// ルールフェード用カタログ.
        /// </summary>
        [SerializeField] private SceneTransitionCatalog _sceneTransitionCatalog = default;

        /// <summary>
        /// ルールフェードのオーバーレイ色.
        /// </summary>
        [SerializeField] private Color _transitionOverlayColor = Color.black;

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
        /// インスタンス.
        /// </summary>
        private static SceneLoadManager Instance => _instance;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            ManagerLocator.Register(this);
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
        public static void LoadScene(string sceneName, int transitionSetId = -1)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("SceneLoadManager.LoadScene failed. sceneName is null or empty.");
                return;
            }

            if (Instance == null)
            {
                Debug.LogError("SceneLoadManager.LoadScene failed. Instance was not initialized.");
                return;
            }

            if (Instance._singleLoadTransitionRunning)
            {
                Debug.LogWarning("SceneLoadManager.LoadScene: 別のシングル遷移が進行中のため無視しました.");
                return;
            }

            if (transitionSetId < 0)
            {
                Instance.ClearAdditiveOverlayStateForSingleLoad();
                SceneManager.LoadScene(sceneName);
                return;
            }

            Instance.ResolveSceneTransitionCatalog();
            var catalog = Instance._sceneTransitionCatalog;
            if (catalog == null)
            {
                Debug.LogWarning(
                    "SceneLoadManager.LoadScene: SceneTransitionCatalog が null のためフェードをスキップしました. " +
                    $"BootScene の Bootstrap にカタログをアサインするか、Data.Load 用に Resources 配下へ「{SceneTransitionCatalog.ResourcesAssetName}.asset」があるか確認してください.");
                Instance.ClearAdditiveOverlayStateForSingleLoad();
                SceneManager.LoadScene(sceneName);
                return;
            }

            if (!catalog.TryGet(transitionSetId, out var entry))
            {
                Debug.LogWarning(
                    $"SceneLoadManager.LoadScene: transitionSetId={transitionSetId} がカタログに無いためフェードをスキップしました. " +
                    "カタログの Sets に同じ ID のエントリがあるか確認してください.");
                Instance.ClearAdditiveOverlayStateForSingleLoad();
                SceneManager.LoadScene(sceneName);
                return;
            }

            Instance.StartCoroutine(Instance.LoadSceneWithRuleFadeRoutine(sceneName, entry));
        }

        /// <summary>
        /// 指定したシーンを加算読み込みする.
        /// </summary>
        /// <param name="sceneName">読み込むシーン名.</param>
        /// <param name="blockUnderlyingInput">true のとき、元シーンの UI（EventSystem）とゲームプレイ入力を無効にする.</param>
        public static void LoadSceneAdditive(string sceneName, bool blockUnderlyingInput = true)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("SceneLoadManager.LoadSceneAdditive failed. sceneName is null or empty.");
                return;
            }

            if (Instance == null)
            {
                Debug.LogError("SceneLoadManager.LoadSceneAdditive failed. Instance was not initialized.");
                return;
            }

            if (IsSceneLoaded(sceneName))
            {
                return;
            }

            Instance.StartCoroutine(Instance.LoadSceneAdditiveCoroutine(sceneName, blockUnderlyingInput));
        }

        /// <summary>
        /// 指定した加算シーンをアンロードする.
        /// </summary>
        /// <param name="sceneName">アンロードするシーン名.</param>
        public static void UnloadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("SceneLoadManager.UnloadScene failed. sceneName is null or empty.");
                return;
            }

            if (Instance == null)
            {
                Debug.LogError("SceneLoadManager.UnloadScene failed. Instance was not initialized.");
                return;
            }

            if (!IsSceneLoaded(sceneName)) { return; }

            Instance.StartCoroutine(Instance.UnloadSceneCoroutine(sceneName));
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
        /// <returns>コルーチン.</returns>
        private IEnumerator LoadSceneWithRuleFadeRoutine(string sceneName, SceneTransitionSetEntry entry)
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
                yield break;
            }

            overlay.Configure(entry.FadeOutMask, entry.Softness);
            overlay.SetProgress(0f);
            overlay.Show();

            yield return overlay.AnimateProgress(0f, 1f, entry.CoverDuration);

            var asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            if (asyncOperation == null)
            {
                Debug.LogError($"SceneLoadManager.LoadSceneWithRuleFadeRoutine failed. sceneName:{sceneName}");
                overlay.Hide();
                _singleLoadTransitionRunning = false;
                yield break;
            }

            while (asyncOperation.isDone == false)
            {
                yield return null;
            }

            yield return null;

            overlay.Configure(entry.FadeInMask, entry.Softness);
            overlay.SetProgress(1f);
            yield return overlay.AnimateProgress(1f, 0f, entry.RevealDuration);
            overlay.Hide();

            _singleLoadTransitionRunning = false;
        }

        /// <summary>
        /// 加算読み込みコルーチン.
        /// </summary>
        /// <param name="sceneName">読み込むシーン名.</param>
        /// <param name="blockUnderlyingInput">元シーンの操作を無効にするか.</param>
        /// <returns>コルーチン.</returns>
        private IEnumerator LoadSceneAdditiveCoroutine(string sceneName, bool blockUnderlyingInput)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            if (asyncOperation == null)
            {
                Debug.LogError($"SceneLoadManager.LoadSceneAdditiveCoroutine failed. sceneName:{sceneName}");
                yield break;
            }

            while (!asyncOperation.isDone)
            {
                yield return null;
            }

            if (!blockUnderlyingInput)
            {
                _disabledEventSystemLayers.Push(new List<EventSystem>());
                _inputGateLayers.Push(false);
                yield break;
            }

            var overlayScene = SceneManager.GetSceneByName(sceneName);
            if (!overlayScene.IsValid())
            {
                Debug.LogError($"SceneLoadManager.LoadSceneAdditiveCoroutine: scene not found after load. sceneName:{sceneName}");
                yield break;
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
        /// シーンアンロードコルーチン.
        /// </summary>
        /// <param name="sceneName">アンロードするシーン名.</param>
        /// <returns>コルーチン.</returns>
        private IEnumerator UnloadSceneCoroutine(string sceneName)
        {
            var asyncOperation = SceneManager.UnloadSceneAsync(sceneName);

            if (asyncOperation == null)
            {
                yield break;
            }

            while (asyncOperation.isDone == false)
            {
                yield return null;
            }

            RestoreOneAdditiveOverlayLayer();
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