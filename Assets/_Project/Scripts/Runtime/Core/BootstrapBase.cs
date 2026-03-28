using UnityEngine;

namespace CarTrickRush.Core
{
    /// =========================================================================================
    /// <summary>
    /// Boot処理の共通初期化を提供するクラス.
    /// </summary>
    /// =========================================================================================
    public static class BootstrapBase
    {
        /// <summary>
        /// 必要なManagerを初期化する.
        /// </summary>
        public static void InitializeManagers()
        {
            CreateManager<CarTrickRush.Managers.GameManager>("GameManager");
            CreateManager<CarTrickRush.Managers.SceneLoadManager>("SceneLoadManager");
            CreateManager<CarTrickRush.Managers.InputManager>("InputManager");
            CreateManager<CarTrickRush.Managers.TimeManager>("TimeManager");
            CreateManager<CarTrickRush.Managers.SaveManager>("SaveManager");
            CreateManager<CarTrickRush.Managers.ScoreManager>("ScoreManager");
            CreateManager<CarTrickRush.Managers.AudioManager>("AudioManager");
        }

        /// <summary>
        /// 指定したManagerが存在しない場合のみ生成する.
        /// </summary>
        private static void CreateManager<T>(string objectName) where T : Component
        {
            if (Object.FindFirstObjectByType<T>() != null)
            {
                return;
            }

            GameObject managerObject = new GameObject(objectName);
            Object.DontDestroyOnLoad(managerObject);
            managerObject.AddComponent<T>();
        }
    }
}