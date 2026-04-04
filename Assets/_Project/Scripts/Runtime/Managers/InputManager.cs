using UnityEngine;
using UnityEngine.InputSystem;

using System;

using CarTrickRush.Core;

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
        /// インスタンス.
        /// </summary>
        private static InputManager _instance = default;

        /// <summary>
        /// CarTrickRushInputActions の Player マップ（バインド中のみ）.
        /// </summary>
        private InputActionMap _playerMap = default;

        /// <summary>
        /// Player/Pause（購読中のみ）.
        /// </summary>
        private InputAction _playerPauseAction = default;

        /// <summary>
        /// Player の回転アクション（購読中のみ）.
        /// </summary>
        private InputAction _rotateRightAction = default;
        private InputAction _rotateLeftAction = default;
        private InputAction _rotateUpAction = default;
        private InputAction _rotateDownAction = default;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// インスタンス.
        /// </summary>
        public static InputManager Instance => _instance;

        #endregion

        #region ------------------ Events ------------------

        /// <summary>
        /// 右回転を実行するイベント.
        /// </summary>  
        public event Action RotateRightPerformed;

        /// <summary>
        /// 左回転を実行するイベント.
        /// </summary>
        public event Action RotateLeftPerformed;

        /// <summary>
        /// 上回転を実行するイベント.
        /// </summary>
        public event Action RotateUpPerformed;

        /// <summary>
        /// 下回転を実行するイベント.
        /// </summary>
        public event Action RotateDownPerformed;

        /// <summary>
        /// ポーズを実行するイベント.
        /// </summary>
        public event Action PausePerformed;

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
            ManagerLocator.Register(this);
        }

        private void OnDestroy()
        {
            UnbindPlayerPauseAction();
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

            RotateRightPerformed?.Invoke();
        }

        /// <summary>
        /// 左回転を実行する.
        /// </summary>
        public void InvokeRotateLeft()
        {
            if (AdditiveOverlayInputGate.IsBlocked) { return; }

            RotateLeftPerformed?.Invoke();
        }

        /// <summary>
        /// 上回転を実行する.
        /// </summary>
        public void InvokeRotateUp()
        {
            if (AdditiveOverlayInputGate.IsBlocked) { return; }

            RotateUpPerformed?.Invoke();
        }

        /// <summary>
        /// 下回転を実行する.
        /// </summary>
        public void InvokeRotateDown()
        {
            if (AdditiveOverlayInputGate.IsBlocked) { return; }

            RotateDownPerformed?.Invoke();
        }

        /// <summary>
        /// ポーズを実行する.
        /// </summary>
        public void InvokePause()
        {
            PausePerformed?.Invoke();
        }

        /// <summary>
        /// 右回転を実行する.
        /// </summary>
        public void OnRotateRight(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                InputManager.Instance.InvokeRotateRight();
            }
        }

        /// <summary>
        /// 左回転を実行する.
        /// </summary>
        public void OnRotateLeft(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                InputManager.Instance.InvokeRotateLeft();
            }
        }

        /// <summary>
        /// 上回転を実行する.
        /// </summary>
        public void OnRotateUp(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                InputManager.Instance.InvokeRotateUp();
            }
        }

        /// <summary>
        /// 下回転を実行する.
        /// </summary>
        public void OnRotateDown(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                InputManager.Instance.InvokeRotateDown();
            }
        }

        /// <summary>
        /// ポーズを実行する.
        /// </summary>
        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                InputManager.Instance.InvokePause();
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