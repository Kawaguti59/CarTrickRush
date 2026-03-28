using UnityEngine;

using System;

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
        private GameSessionData _gameSessionData;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// 現在のスコア.
        /// </summary>
        public int CurrentScore => _gameSessionData?.CurrentScore ?? 0;

        #endregion

        #region ------------------ Events ------------------

        /// <summary>
        /// スコア変更時イベント.
        /// </summary>
        public event Action<int> ScoreChanged;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            ManagerLocator.Register(this);
            _gameSessionData = new GameSessionData();
            _gameSessionData.Reset();
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
            ScoreChanged?.Invoke(_gameSessionData.CurrentScore);
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

            ScoreChanged?.Invoke(_gameSessionData.CurrentScore);
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