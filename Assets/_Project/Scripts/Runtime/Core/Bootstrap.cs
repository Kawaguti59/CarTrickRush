using UnityEngine;
using UnityEngine.InputSystem;

using VContainer;

using CarTrickRush.Data;
using CarTrickRush.Managers;

namespace CarTrickRush.Core
{
    /// =========================================================================================
    /// <summary>
    /// 通常起動用のBoot処理を行うクラス.
    /// </summary>
    /// =========================================================================================
    [DefaultExecutionOrder(100)]
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

        /// <summary>
        /// 入力アクションアセット.
        /// </summary>
        [SerializeField] private InputActionAsset _inputActionAsset = default;

        private SceneLoadManager _sceneLoadManager = default;
        private InputManager _inputManager = default;

        #endregion

        #region ------------------ VContainer Methods ------------------

        [Inject]
        void Construct(SceneLoadManager sceneLoadManager, InputManager inputManager)
        {
            _sceneLoadManager = sceneLoadManager;
            _inputManager = inputManager;

            if (_sceneTransitionCatalog != null)
            {
                _sceneLoadManager.ApplyBootstrapSceneTransitionCatalog(_sceneTransitionCatalog);
            }

            _inputManager.BindPlayerPauseAction(_inputActionAsset);
        }

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Start()
        {
            _sceneLoadManager.LoadScene(_firstSceneName);
        }

        #endregion
    }
}
