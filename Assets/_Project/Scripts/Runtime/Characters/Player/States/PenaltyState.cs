using UnityEngine;

using CarTrickRush.Definitions;
using CarTrickRush.Characters.Player.Interfaces;

namespace CarTrickRush.Characters.Player.States
{
    /// =========================================================================================
    /// <summary>
    /// ペナルティ状態クラス. 
    /// </summary>
    /// =========================================================================================
    public sealed class PenaltyState : IPlayerState
    {
        #region ------------------ Fields ------------------

        private readonly PlayerController _playerController = default;

        private PenaltyPhase _phase = default;
        private float _phaseTimer = default;

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
            _playerController.StartPenalty();
            _phase = PenaltyPhase.CarHidden;
            _phaseTimer = Mathf.Max(0f, _playerController.PenaltyHideTime);
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
            _phaseTimer -= Time.deltaTime;

            if (_phaseTimer > 0f)
            {
                return;
            }

            if (_phase == PenaltyPhase.CarHidden)
            {
                _playerController.StartPenaltyBlink();
                _phase = PenaltyPhase.BlinkRecover;
                _phaseTimer = Mathf.Max(0f, _playerController.PenaltyBlinkTime);
                return;
            }

            _playerController.ChangeState(_playerController.GroundState);
        }

        /// <summary>
        /// 物理更新処理.
        /// </summary>
        public void FixedUpdate()
        {
            _playerController.MoveForward();
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
    }
}
