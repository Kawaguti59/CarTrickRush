using UnityEngine;
using UnityEngine.InputSystem;

using R3;

namespace CarTrickRush.Managers
{
    /// =========================================================================================
    /// <summary>
    /// 入力管理Manager.
    /// </summary>
    /// =========================================================================================
    public sealed class InputManager : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// CarTrickRushInputActions の Player マップ.
        /// </summary>
        private InputActionMap _playerMap = default;

        /// <summary>
        /// Player/Pause.
        /// </summary>
        private InputAction _playerPauseAction = default;

        /// <summary>
        /// Player/RotateRight.
        /// </summary>
        private InputAction _rotateRightAction = default;

        /// <summary>
        /// Player/RotateLeft.
        /// </summary>
        private InputAction _rotateLeftAction = default;

        /// <summary>
        /// Player/RotateUp.
        /// </summary>
        private InputAction _rotateUpAction = default;

        /// <summary>
        /// Player/RotateDown.
        /// </summary>
        private InputAction _rotateDownAction = default;

        /// <summary>
        /// 右回転通知.
        /// </summary>
        private readonly Subject<Unit> _rotateRightPerformed = new();

        /// <summary>
        /// 左回転通知.
        /// </summary>
        private readonly Subject<Unit> _rotateLeftPerformed = new();

        /// <summary>
        /// 上回転通知.
        /// </summary>
        private readonly Subject<Unit> _rotateUpPerformed = new();

        /// <summary>
        /// 下回転通知.
        /// </summary>
        private readonly Subject<Unit> _rotateDownPerformed = new();

        /// <summary>
        /// ポーズ通知.
        /// </summary>
        private readonly Subject<Unit> _pausePerformed = new();

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// 右回転を実行する通知.
        /// </summary>
        public Observable<Unit> RotateRightPerformed => _rotateRightPerformed;

        /// <summary>
        /// 左回転を実行する通知.
        /// </summary>
        public Observable<Unit> RotateLeftPerformed => _rotateLeftPerformed;

        /// <summary>
        /// 上回転を実行する通知.
        /// </summary>
        public Observable<Unit> RotateUpPerformed => _rotateUpPerformed;

        /// <summary>
        /// 下回転を実行する通知.
        /// </summary>
        public Observable<Unit> RotateDownPerformed => _rotateDownPerformed;

        /// <summary>
        /// ポーズを実行する通知.
        /// </summary>
        public Observable<Unit> PausePerformed => _pausePerformed;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void OnDestroy()
        {
            UnbindPlayerPauseAction();
            _rotateRightPerformed.Dispose();
            _rotateLeftPerformed.Dispose();
            _rotateUpPerformed.Dispose();
            _rotateDownPerformed.Dispose();
            _pausePerformed.Dispose();
        }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// CarTrickRushInputActions の Player マップを有効化する（ポーズ・回転は Input System 経由）.
        /// </summary>
        /// <param name="asset">Input Actions アセット.</param>
        public void BindPlayerPauseAction(InputActionAsset asset)
        {
            UnbindPlayerPauseAction();
            if (asset == null)
            {
                return;
            }

            var map = asset.FindActionMap("Player", throwIfNotFound: false);
            if (map == null)
            {
                return;
            }

            _playerMap = map;
            _playerPauseAction = map.FindAction("Pause", throwIfNotFound: false);
            _rotateRightAction = map.FindAction("RotateRight", throwIfNotFound: false);
            _rotateLeftAction = map.FindAction("RotateLeft", throwIfNotFound: false);
            _rotateUpAction = map.FindAction("RotateUp", throwIfNotFound: false);
            _rotateDownAction = map.FindAction("RotateDown", throwIfNotFound: false);

            if (_playerPauseAction != null)
            {
                _playerPauseAction.performed += OnPlayerPausePerformed;
            }

            if (_rotateRightAction != null)
            {
                _rotateRightAction.performed += OnRotateRight;
            }

            if (_rotateLeftAction != null)
            {
                _rotateLeftAction.performed += OnRotateLeft;
            }

            if (_rotateUpAction != null)
            {
                _rotateUpAction.performed += OnRotateUp;
            }

            if (_rotateDownAction != null)
            {
                _rotateDownAction.performed += OnRotateDown;
            }

            map.Enable();
        }

        /// <summary>
        /// 右回転を実行する.
        /// </summary>
        public void InvokeRotateRight()
        {
            if (AdditiveOverlayInputGate.IsBlocked) { return; }

            _rotateRightPerformed.OnNext(Unit.Default);
        }

        /// <summary>
        /// 左回転を実行する.
        /// </summary>
        public void InvokeRotateLeft()
        {
            if (AdditiveOverlayInputGate.IsBlocked) { return; }

            _rotateLeftPerformed.OnNext(Unit.Default);
        }

        /// <summary>
        /// 上回転を実行する.
        /// </summary>
        public void InvokeRotateUp()
        {
            if (AdditiveOverlayInputGate.IsBlocked) { return; }

            _rotateUpPerformed.OnNext(Unit.Default);
        }

        /// <summary>
        /// 下回転を実行する.
        /// </summary>
        public void InvokeRotateDown()
        {
            if (AdditiveOverlayInputGate.IsBlocked) { return; }

            _rotateDownPerformed.OnNext(Unit.Default);
        }

        /// <summary>
        /// ポーズを実行する.
        /// </summary>
        public void InvokePause()
        {
            _pausePerformed.OnNext(Unit.Default);
        }

        /// <summary>
        /// 右回転を実行する.
        /// </summary>
        public void OnRotateRight(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                InvokeRotateRight();
            }
        }

        /// <summary>
        /// 左回転を実行する.
        /// </summary>
        public void OnRotateLeft(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                InvokeRotateLeft();
            }
        }

        /// <summary>
        /// 上回転を実行する.
        /// </summary>
        public void OnRotateUp(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                InvokeRotateUp();
            }
        }

        /// <summary>
        /// 下回転を実行する.
        /// </summary>
        public void OnRotateDown(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                InvokeRotateDown();
            }
        }

        /// <summary>
        /// ポーズを実行する.
        /// </summary>
        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                InvokePause();
            }
        }

        #endregion

        #region ------------------ Private Methods ------------------

        private void OnPlayerPausePerformed(InputAction.CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            }

            InvokePause();
        }

        /// <summary>
        /// Player マップの購読と無効化を解除する.
        /// </summary>
        private void UnbindPlayerPauseAction()
        {
            if (_playerPauseAction != null)
            {
                _playerPauseAction.performed -= OnPlayerPausePerformed;
                _playerPauseAction = null;
            }

            if (_rotateRightAction != null)
            {
                _rotateRightAction.performed -= OnRotateRight;
                _rotateRightAction = null;
            }

            if (_rotateLeftAction != null)
            {
                _rotateLeftAction.performed -= OnRotateLeft;
                _rotateLeftAction = null;
            }

            if (_rotateUpAction != null)
            {
                _rotateUpAction.performed -= OnRotateUp;
                _rotateUpAction = null;
            }

            if (_rotateDownAction != null)
            {
                _rotateDownAction.performed -= OnRotateDown;
                _rotateDownAction = null;
            }

            if (_playerMap != null)
            {
                _playerMap.Disable();
                _playerMap = null;
            }
        }

        #endregion
    }
}
