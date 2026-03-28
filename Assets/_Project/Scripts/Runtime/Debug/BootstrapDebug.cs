using UnityEngine;

using CarTrickRush.Data;
using CarTrickRush.Managers;

namespace CarTrickRush.Core
{
    /// =========================================================================================
    /// <summary>
    /// デバッグ起動用のBoot処理を行うクラス.
    /// </summary>
    /// =========================================================================================
    public sealed class BootstrapDebug : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// シーン遷移カタログ（Bootstrap と同様に SceneLoadManager へ注入）.
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

        #endregion
    }
}
