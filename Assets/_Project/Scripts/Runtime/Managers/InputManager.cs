using UnityEngine;

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

        #region ------------------ Interface Methods ------------------



        #endregion

        #region ------------------ Public Methods ------------------



        #endregion

        #region ------------------ Private Methods ------------------



        #endregion
    }
}