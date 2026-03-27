using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

using System.Collections;
using System.Collections.Generic;

using CarTrickRush.Core;

namespace CarTrickRush.Managers
{
    /// <summary>
    /// 加算シーン表示中に、元シーンのゲームプレイ入力（InputManager 経由）を無効にするためのゲート.
    /// </summary>
    public static class AdditiveOverlayInputGate
    {
        private static int _depth;

        public static bool IsBlocked => _depth > 0;

        internal static void Push()
        {
            _depth++;
        }

        internal static void Pop()
        {
            if (_depth > 0)
            {
                _depth--;
            }
        }

        /// <summary>
        /// シングルシーン遷移などで加算シーンが破棄されたとき、参照カウンタを初期化する.
        /// </summary>
        internal static void ResetDepth()
        {
            _depth = 0;
        }
    }

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
        private static SceneLoadManager _instance;

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
        }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// 指定したシーンへ遷移する.
        /// </summary>
        /// <param name="sceneName">遷移先シーン名.</param>
        public static void LoadScene(string sceneName)
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

            Instance.ClearAdditiveOverlayStateForSingleLoad();
            SceneManager.LoadScene(sceneName);
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

            if (!IsSceneLoaded(sceneName))
            {
                return;
            }

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
            for (var i = 0; i < systems.Length; i++)
            {
                var es = systems[i];
                if (es == null || es.gameObject.scene == overlayScene)
                {
                    continue;
                }

                if (es.enabled)
                {
                    es.enabled = false;
                    disabled.Add(es);
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
            if (_inputGateLayers.Count == 0)
            {
                return;
            }

            var hadInputGate = _inputGateLayers.Pop();
            if (hadInputGate)
            {
                AdditiveOverlayInputGate.Pop();
            }

            if (_disabledEventSystemLayers.Count > 0)
            {
                var disabled = _disabledEventSystemLayers.Pop();
                for (var i = 0; i < disabled.Count; i++)
                {
                    var es = disabled[i];
                    if (es != null)
                    {
                        es.enabled = true;
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