using UnityEngine;

namespace CarTrickRush.Definitions
{
    /// =========================================================================================
    /// <summary>
    /// ペナルティフェーズ種別定義.
    /// </summary>
    /// =========================================================================================
    public enum PenaltyPhase
    {
        /// <summary>
        /// 未定義.
        /// </summary>
        None = 0,

        /// <summary>
        /// 車体非表示.
        /// </summary>
        CarHidden = 1,

        /// <summary>
        /// 点滅復帰.
        /// </summary>
        BlinkRecover = 2,
    }
}