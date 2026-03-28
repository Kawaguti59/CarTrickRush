using UnityEngine;

using CarTrickRush.Data;
using CarTrickRush.Managers;

namespace CarTrickRush.Core
{
    /// =========================================================================================
    /// <summary>
    /// 通常起動用のBoot処理を行うクラス.
    /// </summary>
    /// =========================================================================================
    public sealed class Bootstrap : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// 最初に遷移するシーン名.
        /// </summary>
        [SerializeField] private string _firstSceneName = "TitleScene";

        /// <summary>
        /// シーン遷移カタログ.
        /// </summary>
        [SerializeField] private SceneTransitionCatalog _sceneTransitionCatalog = default;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            BootstrapBase.InitializeManagers();
            if (_sceneTransitionCatalog != null && ManagerLocator.SceneLoadManager != null)
            {
                ManagerLocator.SceneLoadManager.ApplyBootstrapSceneTransitionCatalog(_sceneTransitionCatalog);
            }
        }

        private void Start()
        {
            SceneLoadManager.LoadScene(_firstSceneName);
        }

        #endregion
    }
}