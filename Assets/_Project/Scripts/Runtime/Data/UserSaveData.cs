namespace CarTrickRush.Data
{
    /// =========================================================================================
    /// <summary>
    /// 永続保存用データを保持するクラス.
    /// </summary>
    /// =========================================================================================
    public sealed class UserSaveData
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// ベストスコア.
        /// </summary>
        private int _bestScore = default;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// ベストスコア.
        /// </summary>
        public int BestScore => _bestScore;

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// ベストスコアを設定する.
        /// </summary>
        /// <param name="score">ベストスコア</param>
        public void SetBestScore(int score)
        {
            _bestScore = score;
        }

        #endregion
    }
}