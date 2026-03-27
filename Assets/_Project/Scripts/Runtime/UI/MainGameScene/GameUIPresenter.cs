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
        [SerializeField] private ScoreView _scoreView;

        /// <summary>
        /// 進捗表示用のビュー.
        /// </summary>
        [SerializeField] private ProgressView _progressView;

        /// <summary>
        /// プレイヤーのTransform.
        /// </summary>
        [SerializeField] private Transform _playerTransform;

        /// <summary>
        /// スタート地点のTransform.
        /// </summary>
        [SerializeField] private Transform _startPoint;

        /// <summary>
        /// ゴール地点のTransform.
        /// </summary>
        [SerializeField] private Transform _goalPoint;

        #endregion

        #region ------------------ Properties ------------------
        
        /// <summary>
        /// スコア管理クラス.
        /// </summary>
        private ScoreManager ScoreManager => ManagerLocator.ScoreManager;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            Validate();
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
            if (ScoreManager == null)
            {
                return;
            }

            ScoreManager.ScoreChanged += OnScoreChanged;
        }

        /// <summary>
        /// イベント購読を解除する.
        /// </summary>
        private void UnsubscribeEvents()
        {
            if (ScoreManager == null)
            {
                return;
            }

            ScoreManager.ScoreChanged -= OnScoreChanged;
        }

        /// <summary>
        /// スコア変更時の表示更新を行う.
        /// </summary>
        private void OnScoreChanged(int score)
        {
            _scoreView?.SetScore(score);
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
            if (ScoreManager == null)
            {
                return;
            }

            _scoreView?.SetScore(ScoreManager.CurrentScore);
        }

        /// <summary>
        /// 進捗表示を更新する.
        /// </summary>
        private void RefreshProgressView()
        {
            if (!TryCalculateProgressRate(out var progressRate))
            {
                return;
            }

            _progressView?.SetProgress(progressRate);
        }

        /// <summary>
        /// 進捗率を計算する.
        /// </summary>
        private bool TryCalculateProgressRate(out float progressRate)
        {
            progressRate = 0f;

            if (_playerTransform == null || _startPoint == null || _goalPoint == null)
            {
                return false;
            }

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

        /// <summary>
        /// バリデーションを行う.
        /// </summary>
        private void Validate()
        {
            if (_scoreView == null)
            {
                Debug.LogWarning($"{nameof(GameUIPresenter)} : ScoreView is not assigned.", this);
            }

            if (_progressView == null)
            {
                Debug.LogWarning($"{nameof(GameUIPresenter)} : ProgressView is not assigned.", this);
            }

            if (ScoreManager == null)
            {
                Debug.LogWarning($"{nameof(GameUIPresenter)} : ScoreManager is not assigned.", this);
            }

            if (_playerTransform == null)
            {
                Debug.LogWarning($"{nameof(GameUIPresenter)} : PlayerTransform is not assigned.", this);
            }

            if (_startPoint == null)
            {
                Debug.LogWarning($"{nameof(GameUIPresenter)} : StartPoint is not assigned.", this);
            }

            if (_goalPoint == null)
            {
                Debug.LogWarning($"{nameof(GameUIPresenter)} : GoalPoint is not assigned.", this);
            }
        }

        #endregion
    }
}