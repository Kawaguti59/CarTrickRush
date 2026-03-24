using UnityEngine;

namespace CarTrickRush.Managers
{
    /// =========================================================================================
    /// <summary>
    /// ゲーム全体の進行状態管理Manager.
    /// </summary>
    /// =========================================================================================
    public sealed class GameManager : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// インスタンス.
        /// </summary>
        private static GameManager _instance;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// インスタンス.
        /// </summary>
        public static GameManager Instance => _instance;

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