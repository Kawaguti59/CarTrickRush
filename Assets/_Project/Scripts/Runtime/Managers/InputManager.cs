using UnityEngine;
using UnityEngine.InputSystem;
using System;

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

        #endregion
    }
}