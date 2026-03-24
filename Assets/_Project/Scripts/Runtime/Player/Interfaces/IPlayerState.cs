using CarTrickRush.Definitions;

namespace CarTrickRush.Player.Interfaces
{
    /// =========================================================================================
    /// <summary>
    /// プレイヤー状態の共通インターフェース.
    /// </summary>
    /// =========================================================================================
    public interface IPlayerState
    {
        #region ------------------ Properties ------------------

        /// <summary>
        /// 状態種別.
        /// </summary>
        PlayerStateType StateType { get; }

        #endregion

        #region ------------------ Interface Methods ------------------

        /// <summary>
        /// 初期化処理.
        /// </summary>
        void Enter();

        /// <summary>
        /// 終了処理.
        /// </summary>
        void Exit();

        /// <summary>
        /// 入力処理.
        /// </summary>
        void HandleInput();

        /// <summary>
        /// 毎フレーム更新処理.
        /// </summary>
        void Update();

        /// <summary>
        /// 物理更新処理.
        /// </summary>
        void FixedUpdate();

        #endregion
    }
}