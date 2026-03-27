using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;

using CarTrickRush.Core;

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
        private static SceneLoadManager _instance;

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

            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// 指定したシーンを加算読み込みする.
        /// </summary>
        /// <param name="sceneName">読み込むシーン名.</param>
        public static void LoadSceneAdditive(string sceneName)
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

            Instance.StartCoroutine(Instance.LoadSceneAdditiveCoroutine(sceneName));
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

            if (IsSceneLoaded(sceneName) == false)
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
        /// <returns>コルーチン.</returns>
        private IEnumerator LoadSceneAdditiveCoroutine(string sceneName)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            if (asyncOperation == null)
            {
                Debug.LogError($"SceneLoadManager.LoadSceneAdditiveCoroutine failed. sceneName:{sceneName}");
                yield break;
            }

            while (asyncOperation.isDone == false)
            {
                yield return null;
            }
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
        }

        #endregion
    }
}