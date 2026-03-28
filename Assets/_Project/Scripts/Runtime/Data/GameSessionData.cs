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

        private int _currentScore = default;
        private float _progressRate = default;

        #endregion

        #region ------------------ Properties ------------------

        public int CurrentScore => _currentScore;
        public float ProgressRate => _progressRate;

        #endregion

        #region ------------------ Public Methods ------------------

        public void Reset()
        {
            _currentScore = 0;
            _progressRate = 0f;
        }

        public void SetScore(int score)
        {
            _currentScore = score;
        }

        public void SetProgress(float progressRate)
        {
            _progressRate = Mathf.Clamp01(progressRate);
        }

        #endregion
    }
}