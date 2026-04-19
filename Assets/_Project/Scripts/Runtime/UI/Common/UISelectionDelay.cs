using UnityEngine;

namespace CarTrickRush.UI.Common
{
    /// =========================================================================================
    /// <summary>
    /// UI 選択の遅延判定用クラス.
    /// </summary>
    /// =========================================================================================
    public static class UISelectionDelay
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// 遅延時間.
        /// </summary>
        private const float _defaultDelaySeconds = 0.12f;

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// 遅延判定を行う.
        /// </summary>
        /// <param name="lostSinceUnscaled">初回検知時刻（未計測は負値）.</param>
        /// <param name="hasValidFocus">意図した選択が生きているか.</param>
        /// <param name="delaySeconds">遅延時間（秒・unscaled）.</param>
        /// <returns>遅延判定結果.</returns>
        public static bool ShouldRestoreNow(
            ref float lostSinceUnscaled, bool hasValidFocus,
            float delaySeconds = _defaultDelaySeconds
        )
        {
            if (hasValidFocus)
            {
                lostSinceUnscaled = -1f;
                return false;
            }

            if (lostSinceUnscaled < 0f)
            {
                lostSinceUnscaled = Time.unscaledTime;
            }

            return Time.unscaledTime - lostSinceUnscaled >= delaySeconds;
        }

        #endregion
    }
}
