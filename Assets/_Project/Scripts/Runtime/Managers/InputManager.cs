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
        /// CarTrickRushInputActions の Player/Pause（有効化中のみ）.
        /// </summary>
        private InputAction _playerPauseAction = default;

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

        private void Update()
        {
            if (AdditiveOverlayInputGate.IsBlocked)
            {
                return;
            }

            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) { return; }

            if (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame)
            {
                InvokeRotateRight();
            }

            if (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame)
            {
                InvokeRotateLeft();
            }

            if (keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame)
            {
                InvokeRotateUp();
            }

            if (keyboard.sKey.wasPressedThisFrame || keyboard.downArrowKey.wasPressedThisFrame)
            {
                InvokeRotateDown();
            }
        }

        private void OnDestroy()
        {
            UnbindPlayerPauseAction();
        }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// CarTrickRushInputActions の Player/Pause を有効化し、押下で InvokePause する.
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
            _playerPauseAction = map?.FindAction("Pause", throwIfNotFound: false);
            if (_playerPauseAction == null)
            {
                return;
            }

            _playerPauseAction.performed += OnPlayerPausePerformed;
            _playerPauseAction.Enable();
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
        /// Player/Pause の購読と無効化を解除する.
        /// </summary>
        private void UnbindPlayerPauseAction()
        {
            if (_playerPauseAction == null)
            {
                return;
            }

            _playerPauseAction.performed -= OnPlayerPausePerformed;
            _playerPauseAction.Disable();
            _playerPauseAction = null;
        }

        #endregion
    }
}