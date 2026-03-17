using UnityEngine;

using CarTrickRush.Definitions;
using CarTrickRush.Player.Interfaces;

namespace CarTrickRush.Player.States
{
    /// =========================================================================================
    /// <summary>
    /// ペナルティ状態クラス.
    /// </summary>
    /// =========================================================================================
    public sealed class PenaltyState : IPlayerState
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// プレイヤー参照.
        /// </summary>
        private readonly PlayerController _playerController;

        /// <summary>
        /// ペナルティ残り時間.
        /// </summary>
        private float _penaltyTimer;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// 状態種別.
        /// </summary>
        public PlayerStateType StateType => PlayerStateType.Penalty;

        #endregion

        #region ------------------ Interface Methods ------------------

        /// <summary>
        /// 状態開始処理.
        /// </summary>
        public void Enter()
        {
            _penaltyTimer = _playerController.PenaltyDuration;
        }

        /// <summary>
        /// 状態終了処理.
        /// </summary>
        public void Exit()
        {
        }

        /// <summary>
        /// 入力処理.
        /// </summary>
        public void HandleInput()
        {
        }

        /// <summary>
        /// フレーム更新処理.
        /// </summary>
        public void Update()
        {
            _penaltyTimer -= Time.deltaTime;

            if (_penaltyTimer > 0f)
            {
                return;
            }

            _playerController.ChangeState(_playerController.GroundState);
        }

        /// <summary>
        /// 物理更新処理.
        /// </summary>
        public void FixedUpdate()
        {
        }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// ペナルティ状態初期化.
        /// </summary>
        /// <param name="playerController">プレイヤー本体参照.</param>
        public PenaltyState(PlayerController playerController)
        {
            _playerController = playerController;
        }

        #endregion

        #region ------------------ Private Methods ------------------



        #endregion
    }
}