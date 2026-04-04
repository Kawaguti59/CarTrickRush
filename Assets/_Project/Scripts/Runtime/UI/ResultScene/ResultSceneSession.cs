using CarTrickRush.Data;

namespace CarTrickRush.UI.Result
{
    /// =========================================================================================
    /// <summary>
    /// リザルトシーンへ渡すデータを保持するクラス.
    /// </summary>
    /// =========================================================================================
    public static class ResultSceneSession
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// 保留中のリザルトデータ.
        /// </summary>   
        private static ResultData _pendingData = default;

        /// <summary>
        /// ニューレコードかどうか.
        /// </summary>
        private static bool _pendingIsNewRecord = default;

        /// <summary>
        /// 保留があるかどうか.
        /// </summary>
        private static bool _hasPending = default;

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// リザルトデータを設定する.
        /// </summary>
        /// <param name="data">リザルトデータ.</param>
        /// <param name="isNewRecord">ニューレコードかどうか.</param>
        public static void SetPending(ResultData data, bool isNewRecord)
        {
            _pendingData = data;
            _pendingIsNewRecord = isNewRecord;
            _hasPending = true;
        }

        /// <summary>
        /// 保留中のリザルトデータを取り出してクリアする.
        /// </summary>
        /// <param name="data">リザルトデータ.</param>
        /// <param name="isNewRecord">ニューレコードかどうか.</param>
        /// <returns>保留中のリザルトデータがあるかどうか.</returns>
        /// </summary>
        public static bool TryConsume(out ResultData data, out bool isNewRecord)
        {
            if (!_hasPending)
            {
                data = null;
                isNewRecord = false;
                return false;
            }

            data = _pendingData;
            isNewRecord = _pendingIsNewRecord;
            _pendingData = null;
            _hasPending = false;
            return true;
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        /// <summary>
        /// 保留中のリザルトデータが無いときだけデバッグ用データを設定する.
        /// </summary>
        public static void AssignDebugPlaceholderIfEmpty()
        {
            if (_hasPending) { return; }

            SetPending(ResultData.CreateDebugPlaceholder(), true);
        }
#endif

        #endregion

    }
}
