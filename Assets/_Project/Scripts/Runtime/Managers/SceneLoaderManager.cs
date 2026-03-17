using UnityEngine;
using UnityEngine.SceneManagement;

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
        }

        #endregion

        #region ------------------ Interface Methods ------------------



        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// 指定したシーンへ遷移する.
        /// </summary>
        /// <param name="sceneName">遷移先シーン名</param>
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

        #endregion

        #region ------------------ Private Methods ------------------



        #endregion
    }
}