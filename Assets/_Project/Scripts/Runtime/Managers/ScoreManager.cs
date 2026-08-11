using UnityEngine;

using R3;

using CarTrickRush.Core;
using CarTrickRush.Data;

namespace CarTrickRush.Managers
{
    /// =========================================================================================
    /// <summary>
    /// スコアの保持と更新を管理するクラス.
    /// </summary>
    /// =========================================================================================
    public sealed class ScoreManager : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// ゲームセッションデータ.
        /// </summary>
        private GameSessionData _gameSessionData = default;

        /// <summary>
        /// 現在のスコア.
        /// </summary>
        private readonly ReactiveProperty<int> _score = new(0);

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// 現在のスコア.
        /// </summary>
        public int CurrentScore => _score.CurrentValue;

        /// <summary>
        /// スコア変更通知.
        /// </summary>
        public ReadOnlyReactiveProperty<int> Score => _score;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            ManagerLocator.Register(this);
            _gameSessionData = new GameSessionData();
            _gameSessionData.Reset();
            _score.Value = _gameSessionData.CurrentScore;
        }

        private void OnDestroy()
        {
            _score.Dispose();
        }

        #endregion
        
        #region ------------------ Public Methods ------------------

        /// <summary>
        /// スコアを初期化する.
        /// </summary>
        public void ResetScore()
        {
            if (_gameSessionData == null) { return; }

            _gameSessionData.SetScore(0);
            _score.Value = _gameSessionData.CurrentScore;
        }

        /// <summary>
        /// スコアを加算する.
        /// </summary>
        /// <param name="value">加算するスコア</param>
        public void AddScore(int value)
        {
            if (_gameSessionData == null) { return; }

            var nextScore = _gameSessionData.CurrentScore + Mathf.Max(0, value);
            _gameSessionData.SetScore(nextScore);
            _score.Value = _gameSessionData.CurrentScore;
        }

        /// <summary>
        /// セッションデータを取得する.
        /// </summary>
        /// <returns>ゲームセッションデータ</returns>
        public GameSessionData GetSessionData()
        {
            return _gameSessionData;
        }

        #endregion
    }
}
