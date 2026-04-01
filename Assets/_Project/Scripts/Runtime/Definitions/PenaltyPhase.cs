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
        /// 車体非表示.
        /// </summary>
        CarHidden = 0,

        /// <summary>
        /// 点滅復帰.
        /// </summary>
        BlinkRecover = 1,
    }
}