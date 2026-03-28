using UnityEngine;

using CarTrickRush.Definitions;
using CarTrickRush.Characters.Player.Interfaces;
using TMPro;

namespace CarTrickRush.Characters.Player.States
{
    /// =========================================================================================
    /// <summary>
    /// 空中状態クラス.
    /// </summary>
    /// =========================================================================================
    public sealed class AirState : IPlayerState
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// プレイヤー参照.
        /// </summary>
        private readonly PlayerController _playerController = default;

        /// <summary>
        /// 地面離脱済みフラグ.
        /// </summary>
        private bool _hasLeftGround = default;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// 状態種別.
        /// </summary>
        public PlayerStateType StateType => PlayerStateType.Air;

        /// <summary>
        /// 接地判定.
        /// </summary>
        public bool IsGrounded => _playerController.IsGrounded();

        #endregion

        #region ------------------ Interface Methods ------------------

        /// <summary>
        /// 状態開始処理.
        /// </summary>
        public void Enter()
        {
            _hasLeftGround = false;
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
            if (!_hasLeftGround && !IsGrounded)
            {
                _hasLeftGround = true;
                return;
            }

            if (_hasLeftGround && IsGrounded)
            {
                _playerController.OnLanding();
                _playerController.ChangeState(_playerController.GroundState);
            }
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
        /// 空中状態初期化.
        /// </summary>
        /// <param name="playerController">プレイヤー本体参照.</param>
        public AirState(PlayerController playerController)
        {
            _playerController = playerController;
        }

        #endregion

        #region ------------------ Private Methods ------------------



        #endregion
    }
}