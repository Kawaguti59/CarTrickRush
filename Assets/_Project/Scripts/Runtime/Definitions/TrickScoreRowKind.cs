namespace CarTrickRush.Definitions
{
    /// =========================================================================================
    /// <summary>
    /// トリックスコア行の種別 (プレハブ切り替え用).
    /// </summary>
    /// =========================================================================================
    public enum TrickScoreRowKind
    {
        /// <summary>
        /// 未定義.
        /// </summary>
        None = 0,   

        /// <summary>
        /// 通常スコア.
        /// </summary>
        Normal = 1,

        /// <summary>
        /// ボーナススコア.
        /// </summary>
        Bonus = 2,
    }
}