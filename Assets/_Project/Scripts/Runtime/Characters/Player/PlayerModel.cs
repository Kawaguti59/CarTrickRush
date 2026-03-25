using System;

using CarTrickRush.Definitions;

namespace CarTrickRush.Character.Player.Model
{
    /// <summary>
    /// プレイヤー状態データを保持するモデル.
    /// </summary>
    public sealed class PlayerModel
    {
        /// <summary>
        /// 現在の状態種別.
        /// </summary>
        public PlayerStateType CurrentStateType { get; private set; }

        /// <summary>
        /// ペナルティ中か.
        /// </summary>
        public bool IsPenalty { get; private set; }

        /// <summary>
        /// 空中にいるか.
        /// </summary>
        public bool IsAirborne { get; private set; }

        /// <summary>
        /// モデルを初期化する.
        /// </summary>
        public void Initialize()
        {
            CurrentStateType = PlayerStateType.Ground;
            IsPenalty = false;
            IsAirborne = false;
        }

        /// <summary>
        /// 状態を更新する.
        /// </summary>
        /// <param name="stateType">状態種別.</param>
        public void ChangeState(PlayerStateType stateType)
        {
            CurrentStateType = stateType;
        }

        /// <summary>
        /// 空中状態を更新する.
        /// </summary>
        /// <param name="isAirborne">空中状態.</param>
        public void SetAirborne(bool isAirborne)
        {
            IsAirborne = isAirborne;
        }

        /// <summary>
        /// ペナルティ状態を更新する.
        /// </summary>
        /// <param name="isPenalty">ペナルティ状態.</param>
        public void SetPenalty(bool isPenalty)
        {
            IsPenalty = isPenalty;
        }
    }
}