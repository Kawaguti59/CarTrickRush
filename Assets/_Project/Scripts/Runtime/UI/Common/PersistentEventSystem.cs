using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace CarTrickRush.UI.Common
{
    /// =========================================================================================
    /// <summary>
    /// EventSystem をアプリ単位で1つに保つ.
    /// </summary>
    /// =========================================================================================
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EventSystem))]
    public sealed class PersistentEventSystem : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// インスタンス.
        /// </summary>
        private static PersistentEventSystem _instance;

        #endregion

        #region ------------------ Methods ------------------
        
        /// <summary>
        /// シーン読み込み時に EventSystem を登録する.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterSceneLoaded()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        /// <summary>
        /// シーン読み込み時に EventSystem を登録する.
        /// </summary>
        /// <param name="scene">読み込まれたシーン.</param>
        /// <param name="mode">読み込みモード.</param>
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (mode != LoadSceneMode.Additive || !scene.IsValid() || _instance == null)
            {
                return;
            }

            var systems = Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
            for (var index = 0; index < systems.Length; index++)
            {
                var eventSystem = systems[index];
                if (eventSystem == null || eventSystem.gameObject.scene != scene)
                {
                    continue;
                }

                if (eventSystem.gameObject == _instance.gameObject)
                {
                    continue;
                }

                Object.Destroy(eventSystem.gameObject);
            }
        }

        /// <summary>
        /// インスタンスを取得する.
        /// </summary>
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

        /// <summary>
        /// インスタンスを破棄する.
        /// </summary>
        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        #endregion
    }
}
