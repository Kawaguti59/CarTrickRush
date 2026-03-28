using CarTrickRush.Definitions;
using CarTrickRush.Characters.Player.Interfaces;

namespace CarTrickRush.Characters.Player.States
{
    /// =========================================================================================
    /// <summary>
    /// 地上状態クラス.
    /// </summary>
    /// =========================================================================================
    public sealed class GroundState : IPlayerState
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// プレイヤー参照.
        /// </summary>
        private readonly PlayerController _playerController = default;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// 状態種別.
        /// </summary>
        public PlayerStateType StateType => PlayerStateType.Ground;

        #endregion

        #region ------------------ Interface Methods ------------------

        public void Enter() { }
        public void Exit() { }
        public void HandleInput() { }
        public void Update() { }

        public void FixedUpdate()
        {
            _playerController.MoveForward();
        }

        #endregion

        public GroundState(PlayerController playerController)
        {
            _playerController = playerController;
        }
    }
}