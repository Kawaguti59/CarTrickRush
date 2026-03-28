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
        /// 遷移直前に呼ぶ。スコア反映後の表示用ベストと、ニューレコードかを渡す.
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
        /// 遷移直前に呼ぶ。スコア反映後の表示用ベストと、ニューレコードかを渡す.
        /// </summary>
        public static void SetPending(ResultData data, bool isNewRecord)
        {
            _pendingData = data;
            _pendingIsNewRecord = isNewRecord;
            _hasPending = true;
        }

        /// <summary>
        /// 保留があれば取り出してクリアする。無ければ false.
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
        /// 保留が無いときだけデバッグ用データを入れる（単体で Result シーンを開く場合など）.
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
