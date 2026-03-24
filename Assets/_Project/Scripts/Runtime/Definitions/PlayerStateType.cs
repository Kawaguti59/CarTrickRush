namespace CarTrickRush.Definitions
{
    /// =========================================================================================
    /// <summary>
    /// プレイヤーの状態種別.
    /// </summary>
    /// =========================================================================================
    public enum PlayerStateType
    {
        /// <summary>
        /// 未定義.
        /// </summary>
        None = 0,

        /// <summary>
        /// 地上.
        /// </summary>
        Ground = 1,

        /// <summary>
        /// 空中.
        /// </summary>
        Air = 2,

        /// <summary>
        /// ペナルティ.
        /// </summary>
        Penalty = 3,
    }
}