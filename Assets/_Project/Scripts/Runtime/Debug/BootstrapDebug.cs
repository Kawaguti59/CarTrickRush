using UnityEngine;
using UnityEngine.InputSystem;

using VContainer;

using CarTrickRush.Data;
using CarTrickRush.Managers;

namespace CarTrickRush.Core
{
    /// =========================================================================================
    /// <summary>
    /// デバッグ起動用のBoot処理を行うクラス.
    /// </summary>
    /// =========================================================================================
    [DefaultExecutionOrder(100)]
    public sealed class BootstrapDebug : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// シーン遷移カタログ（Bootstrap と同様に SceneLoadManager へ注入）.
        /// </summary>
        [SerializeField] private SceneTransitionCatalog _sceneTransitionCatalog = default;

        /// <summary>
        /// 入力アクションアセット.
        /// </summary>
        [SerializeField] private InputActionAsset _inputActionAsset = default;

        #endregion

        #region ------------------ VContainer Methods ------------------

        [Inject]
        void Construct(SceneLoadManager sceneLoadManager, InputManager inputManager)
        {
            if (_sceneTransitionCatalog != null)
            {
                sceneLoadManager.ApplyBootstrapSceneTransitionCatalog(_sceneTransitionCatalog);
            }

            inputManager.BindPlayerPauseAction(_inputActionAsset);
        }

        #endregion
    }
}
