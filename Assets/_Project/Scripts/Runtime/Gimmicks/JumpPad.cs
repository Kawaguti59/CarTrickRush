using UnityEngine;

using CarTrickRush.Gimmicks.Interfaces;
using CarTrickRush.Player;

namespace CarTrickRush.Gimmicks
{
    /// =========================================================================================
    /// <summary>
    /// ジャンプ台クラス.
    /// </summary>
    /// =========================================================================================
    public sealed class JumpPad : MonoBehaviour, IGimmick
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// ジャンプ力.
        /// </summary>
        [SerializeField] private float _jumpPower = 10.0f;

        #endregion


        #region ------------------ Properties ------------------

        private float JumpPower => _jumpPower;

        #endregion


        #region ------------------ MonoBehaviour Methods ------------------

        private void OnTriggerEnter(Collider other)
        {
            OnPlayerEnter(other);
        }

        private void OnTriggerStay(Collider other)
        {
            OnPlayerStay(other);
        }

        private void OnTriggerExit(Collider other)
        {
            OnPlayerExit(other);
        }

        #endregion

        #region ------------------ Interface Methods ------------------

        /// <summary>
        /// 接触開始時の処理.
        /// </summary>
        /// <param name="other">接触相手Collider.</param>
        public void OnPlayerEnter(Collider other)
        {
            if (other == null)
            {
                return;
            }

            if (other.TryGetComponent<PlayerController>(out var playerController))
            {
                playerController.OnJumpPadTriggered(JumpPower);
                return;
            }
        }

        /// <summary>
        /// 接触中の処理.
        /// </summary>
        /// <param name="other">接触相手Collider.</param>
        public void OnPlayerStay(Collider other)
        {
        }

        /// <summary>
        /// 接触終了時の処理.
        /// </summary>
        /// <param name="other">接触相手Collider.</param>
        public void OnPlayerExit(Collider other)
        {
        }

        #endregion
    }
}