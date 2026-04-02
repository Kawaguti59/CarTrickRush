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
        #region ------------------ Fields ------------------

        /// <summary>
        /// Managerルートオブジェクト名.
        /// </summary>
        private const string ManagersRootName = "Managers";

        /// <summary>
        /// Managerルートのキャッシュ.
        /// </summary>
        private static GameObject _managersRoot = default;

        #endregion

        #region ------------------ Public Methods ------------------
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

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// 指定したManagerが存在しない場合のみ生成する.
        /// </summary>
        /// <typeparam name="T">Managerの型.</typeparam>
        /// <param name="objectName">Managerのオブジェクト名.</param>
        private static void CreateManager<T>(string objectName) where T : Component
        {
            if (Object.FindFirstObjectByType<T>() != null)
            {
                return;
            }

            GameObject managerObject = new GameObject(objectName);
            managerObject.AddComponent<T>();
            managerObject.transform.SetParent(GetOrCreateManagersRoot().transform, false);
        }

        /// <summary>
        /// Managersルートを取得または生成する.
        /// </summary>
        private static GameObject GetOrCreateManagersRoot()
        {
            if (_managersRoot != null) { return _managersRoot; }

            var root = GameObject.Find(ManagersRootName);
            if (root != null)
            {
                if (root.transform.parent == null)
                {
                    Object.DontDestroyOnLoad(root);
                    _managersRoot = root;
                    return _managersRoot;
                }
            }

            root = new GameObject(ManagersRootName);
            Object.DontDestroyOnLoad(root);
            _managersRoot = root;
            return _managersRoot;
        }

        #endregion
    }
}