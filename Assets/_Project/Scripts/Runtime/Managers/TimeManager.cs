using UnityEngine;

namespace CarTrickRush.Managers
{
    /// =========================================================================================
    /// <summary>
    /// 制限時間や時間進行の管理Manager.
    /// </summary>
    /// =========================================================================================
    public sealed class TimeManager : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// インスタンス.
        /// </summary>
        private static TimeManager _instance;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// インスタンス.
        /// </summary>
        public static TimeManager Instance => _instance;

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