using UnityEngine;

namespace CarTrickRush.Data
{
    /// =========================================================================================
    /// <summary>
    /// ゲームセッションデータ.
    /// </summary>
    /// =========================================================================================
    public sealed class GameSessionData
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// 現在のスコア.
        /// </summary>
        private int _currentScore = default;

        /// <summary>
        /// 現在の進行率.
        /// </summary>
        private float _progressRate = default;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// 現在のスコア.
        /// </summary>
        public int CurrentScore => _currentScore;

        /// <summary>
        /// 現在の進行率.
        /// </summary>
        public float ProgressRate => _progressRate;

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// セッションデータをリセットする.
        /// </summary>
        public void Reset()
        {
            _currentScore = 0;
            _progressRate = 0f;
        }

        /// <summary>
        /// スコアを設定する.
        /// </summary>
        /// <param name="score">設定するスコア.</param>
        public void SetScore(int score)
        {
            _currentScore = score;
        }

        /// <summary>
        /// 進行率を設定する.
        /// </summary>
        /// <param name="progressRate">設定する進行率.</param>
        public void SetProgress(float progressRate)
        {
            _progressRate = Mathf.Clamp01(progressRate);
        }

        #endregion
    }
}