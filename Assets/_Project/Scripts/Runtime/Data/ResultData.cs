namespace CarTrickRush.Data
{
    /// =========================================================================================
    /// <summary>
    /// リザルト表示用データクラス.
    /// </summary>
    /// =========================================================================================
    public sealed class ResultData
    {
        #region ------------------ Properties ------------------

        /// <summary>
        /// スコア.
        /// </summary>
        public int CurrentScore { get; }

        /// <summary>
        /// ベストスコア.
        /// </summary>
        public int BestScore { get; }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// リザルトデータを生成する.
        /// </summary>
        /// <param name="currentScore">今回のスコア.</param>
        /// <param name="bestScore">ベストスコア.</param>
        public ResultData(int currentScore, int bestScore)
        {
            CurrentScore = currentScore;
            BestScore = bestScore;
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        /// <summary>
        /// 単体でリザルトシーンを開いたときなど用の仮データを生成する.
        /// </summary>
        public static ResultData CreateDebugPlaceholder()
        {
            return new ResultData(12345, 12000);
        }
#endif

        #endregion
    }
}
