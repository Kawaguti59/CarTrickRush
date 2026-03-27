using CarTrickRush.Definitions;

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

        /// <summary>
        /// ニューレコードか.
        /// </summary>
        public bool IsNewRecord { get; }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// リザルトデータを生成する.
        /// </summary>
        /// <param name="currentScore">今回スコア.</param>
        /// <param name="bestScore">ベストスコア.</param>
        /// <param name="isNewRecord">ニューレコードか.</param>
        public ResultData(int currentScore, int bestScore, bool isNewRecord)
        {
            CurrentScore = currentScore;
            BestScore = bestScore;
            IsNewRecord = isNewRecord;
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        /// <summary>
        /// エディタ／Development ビルド向け。単体で Result シーンを開いたときなど用の仮データ.
        /// </summary>
        public static ResultData CreateDebugPlaceholder()
        {
            return new ResultData(
                currentScore: 12345,
                bestScore: 12000,
                isNewRecord: true
            );
        }
#endif

        #endregion
    }
}