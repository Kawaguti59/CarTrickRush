using UnityEngine;

using CarTrickRush.Core;
using CarTrickRush.Definitions;
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
        /// アクティブなインスタンス.
        /// </summary>
        private static GameUIPresenter _instance = default;

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
        [SerializeField] private TrickScoreFeedView _trickScoreFeedView = default;

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
        /// ゲームUI仲介のインスタンス.
        /// </summary>
        public static GameUIPresenter Instance => _instance;
        
        /// <summary>
        /// スコア管理クラス.
        /// </summary>
        private ScoreManager ScoreManager => ManagerLocator.ScoreManager;

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// トリックスコアをフィードに追加する.
        /// </summary>
        /// <param name="displayName">表示名.</param>
        /// <param name="addValue">加算値.</param>
        /// <param name="rowKind">行の種別.</param>
        /// <param name="endGroup">同一セットの最後かどうか.</param>
        public void PushTrickScore(string displayName, int addValue, TrickScoreRowKind rowKind, bool endGroup = true)
        {
            if (_trickScoreFeedView == null) { return; }

            _trickScoreFeedView.Push(displayName, addValue, rowKind, endGroup);
        }

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            _instance = this;
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

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

            progressRate = Mathf.InverseLerp(startX, goalX, _playerTransform.position.x);

            return true;
        }

        #endregion
    }
}