namespace CarTrickRush.Data
{
    /// =========================================================================================
    /// <summary>
    /// 永続保存用データを保持するクラス.
    /// </summary>
    /// =========================================================================================
    public sealed class UserSaveData
    {
        #region ------------------ Fields & Constants ------------------

        /// <summary>
        /// ベストスコア.
        /// </summary>
        private int _bestScore;
        #endregion

        #region ------------------ Properties & Events ------------------

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