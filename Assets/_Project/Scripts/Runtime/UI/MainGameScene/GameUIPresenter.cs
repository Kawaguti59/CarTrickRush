using UnityEngine;

using CarTrickRush.Core;
using CarTrickRush.Managers;

namespace CarTrickRush.UI
{
    /// =========================================================================================
    /// <summary>
    /// ゲームUIへの値反映を仲介するクラス.
    /// </summary>
    /// =========================================================================================
    public sealed class GameUIPresenter : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// スコア表示用のビュー.
        /// </summary>
        [SerializeField] private ScoreView _scoreView = default;

        /// <summary>
        /// 進捗表示用のビュー.
        /// </summary>
        [SerializeField] private ProgressView _progressView = default;

        /// <summary>
        /// ボーナススコアのキュー表示.
        /// </summary>
        [SerializeField] private BonusScoreFeedView _bonusScoreFeedView = default;

        /// <summary>
        /// プレイヤーのTransform.
        /// </summary>
        [SerializeField] private Transform _playerTransform = default;

        /// <summary>
        /// スタート地点のTransform.
        /// </summary>
        [SerializeField] private Transform _startPoint = default;

        /// <summary>
        /// ゴール地点のTransform.
        /// </summary>
        [SerializeField] private Transform _goalPoint = default;

        #endregion

        #region ------------------ Properties ------------------
        
        /// <summary>
        /// スコア管理クラス.
        /// </summary>
        private ScoreManager ScoreManager => ManagerLocator.ScoreManager;

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// ボーナス名と加算値をキュー表示に追加する (ゲームロジック側は後から接続).
        /// </summary>
        public void PushBonusScore(string bonusDisplayName, int addValue)
        {
            if (_bonusScoreFeedView == null) { return; }

            _bonusScoreFeedView.Push(bonusDisplayName, addValue);
        }

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void Start()
        {
            SyncViews();
        }

        private void Update()
        {
            RefreshProgressView();
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// イベント購読を行う.
        /// </summary>
        private void SubscribeEvents()
        {
            if (ScoreManager == null) { return; }

            ScoreManager.ScoreChanged += OnScoreChanged;
        }

        /// <summary>
        /// イベント購読を解除する.
        /// </summary>
        private void UnsubscribeEvents()
        {
            if (ScoreManager == null) { return; }

            ScoreManager.ScoreChanged -= OnScoreChanged;
        }

        /// <summary>
        /// スコア変更時の表示更新を行う.
        /// </summary>
        private void OnScoreChanged(int score)
        {
            _scoreView.SetScore(score);
        }

        /// <summary>
        /// ビューの初期同期を行う.
        /// </summary>
        private void SyncViews()
        {
            RefreshScoreView();
            RefreshProgressView();
        }

        /// <summary>
        /// スコア表示を更新する.
        /// </summary>
        private void RefreshScoreView()
        {
            if (ScoreManager == null) { return; }

            _scoreView.SetScore(ScoreManager.CurrentScore);
        }

        /// <summary>
        /// 進捗表示を更新する.
        /// </summary>
        private void RefreshProgressView()
        {
            if (!TryCalculateProgressRate(out var progressRate)) { return; }

            _progressView.SetProgress(progressRate);
        }

        /// <summary>
        /// 進捗率を計算する.
        /// </summary>
        private bool TryCalculateProgressRate(out float progressRate)
        {
            progressRate = 0f;

            var startX = _startPoint.position.x;
            var goalX = _goalPoint.position.x;

            if (Mathf.Approximately(startX, goalX))
            {
                progressRate = 0f;
                return true;
            }

            var playerX = _playerTransform.position.x;
            progressRate = Mathf.InverseLerp(startX, goalX, playerX);

            return true;
        }

        #endregion
    }
}