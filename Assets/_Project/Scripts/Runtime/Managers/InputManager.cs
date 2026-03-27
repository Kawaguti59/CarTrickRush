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
        private static InputManager _instance;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// インスタンス.
        /// </summary>
        public static InputManager Instance => _instance;

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
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            // Fallback polling for Input System keyboard.
            // This keeps gameplay input working even if PlayerInput/UnityEvent wiring is missing.
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

            if (keyboard.escapeKey.wasPressedThisFrame)
            {
                InvokePause();
            }
        }

        #endregion

        #region ------------------ Events ------------------

        public event Action RotateRightPerformed;
        public event Action RotateLeftPerformed;
        public event Action RotateUpPerformed;
        public event Action RotateDownPerformed;

        public event Action PausePerformed;

        #endregion

        #region ------------------ Public Methods ------------------

        public void InvokeRotateRight() => RotateRightPerformed?.Invoke();
        public void InvokeRotateLeft() => RotateLeftPerformed?.Invoke();
        public void InvokeRotateUp() => RotateUpPerformed?.Invoke();
        public void InvokeRotateDown() => RotateDownPerformed?.Invoke();

        public void InvokePause() => PausePerformed?.Invoke();
        public void OnRotateRight(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                InputManager.Instance.InvokeRotateRight();
            }
        }

        public void OnRotateLeft(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                InputManager.Instance.InvokeRotateLeft();
            }
        }

        public void OnRotateUp(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                InputManager.Instance.InvokeRotateUp();
            }
        }

        public void OnRotateDown(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                InputManager.Instance.InvokeRotateDown();
            }
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                InputManager.Instance.InvokePause();
            }
        }

        #endregion
    }
}