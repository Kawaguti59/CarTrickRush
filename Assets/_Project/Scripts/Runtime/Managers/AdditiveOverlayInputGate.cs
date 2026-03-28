namespace CarTrickRush.Managers
{
    /// =========================================================================================
    /// <summary>
    /// 加算シーン表示中に元シーンのゲームプレイ入力を無効にするためのゲート.
    /// </summary>
    /// =========================================================================================
    public static class AdditiveOverlayInputGate
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// ゲートの深さ.
        /// </summary>
        private static int _depth = default;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// ゲートが有効かどうか.
        /// </summary>
        public static bool IsBlocked => _depth > 0;

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// ゲートを積む.
        /// </summary>
        internal static void Push()
        {
            _depth++;
        }

        /// <summary>
        /// ゲートを取り除く.
        /// </summary>
        internal static void Pop()
        {
            if (_depth > 0)
            {
                _depth--;
            }
        }

        /// <summary>
        /// シングルシーン遷移などで加算シーンが破棄されたとき、参照カウンタを初期化する.
        /// </summary>
        internal static void ResetDepth()
        {
            _depth = 0;
        }

        #endregion
    }
}
