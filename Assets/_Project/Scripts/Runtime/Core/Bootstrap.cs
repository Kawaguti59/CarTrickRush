using CarTrickRush.Managers;
using UnityEngine;

namespace CarTrickRush.Core
{
    /// =========================================================================================
    /// <summary>
    /// ゲーム起動時に各Managerを初期化し最初のシーンへ遷移するエントリーポイント.
    /// </summary>
    /// =========================================================================================
    public sealed class Bootstrap : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// 最初の背にするタイトル名.
        /// </summary>
        [SerializeField] private string _firstSceneName = "TitleScene";

        #endregion

        #region ------------------ Properties ------------------



        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            InitializeManagers();
        }

        private void Start()
        {
            SceneLoadManager.LoadScene(_firstSceneName);
        }

        #endregion

        #region ------------------ Interface Methods ------------------



        #endregion

        #region ------------------ Public Methods ------------------



        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// 必要なManagerを生成する.
        /// </summary>
        private void InitializeManagers()
        {
            CreateManager<GameManager>("GameManager");
            CreateManager<SceneLoadManager>("SceneLoadManager");
            CreateManager<InputManager>("InputManager");
            CreateManager<TimeManager>("TimeManager");
        }

        /// <summary>
        /// 指定したManagerが存在しない場合のみ生成する.
        /// </summary>
        /// <typeparam name="T">生成するManagerの型</typeparam>
        /// <param name="objectName">生成するGameObject名</param>
        private void CreateManager<T>(string objectName) where T : Component
        {
            if (FindFirstObjectByType<T>() != null)
            {
                return;
            }

            GameObject managerObject = new GameObject(objectName);
            managerObject.AddComponent<T>();
        }

        #endregion
    }
}