using UnityEngine;

using System.Collections;

using CarTrickRush.Characters.Player;
using CarTrickRush.Core;
using CarTrickRush.Data;

namespace CarTrickRush.Managers
{
    /// =========================================================================================
    /// <summary>
    /// ゲーム全体の進行状態管理Manager.
    /// </summary>
    /// =========================================================================================
    public sealed class GameManager : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// インスタンス.
        /// </summary>
        private static GameManager _instance;

        /// <summary>
        /// ゴール後待機時間.
        /// </summary>
        [SerializeField] private float _resultOverlayDelay = 1.5f;

        /// <summary>
        /// ゴール時プレイヤー参照.
        /// </summary>
        private PlayerController _playerController;

        /// <summary>
        /// ゴール演出実行中か.
        /// </summary>
        private bool _isGoalSequenceRunning;

        /// <summary>
        /// タイトルシーン名.
        /// </summary>
        private string _titleSceneName = "TitleScene";

        /// <summary>
        /// ゲームシーン名.
        /// </summary>
        private string _gameSceneName = "GameScene";

        /// <summary>
        /// リザルト加算シーン名.
        /// </summary>
        private string _resultOverlaySceneName = "ResultScene";

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// インスタンス.
        /// </summary>
        public static GameManager Instance => _instance;

        /// <summary>
        /// 現在のリザルトデータ.
        /// </summary>
        public ResultData CurrentResultData { get; private set; }

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            ManagerLocator.Register(this);
        }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// プレイヤー参照を登録する.
        /// </summary>
        /// <param name="playerController">PlayerController.</param>
        public void RegisterPlayer(PlayerController playerController)
        {
            if (playerController == null) { return; }

            _playerController = playerController;
        }

        /// <summary>
        /// ゴール到達時の処理を開始する.
        /// </summary>
        public void OnGoalReached()
        {
            if (_isGoalSequenceRunning) { return; }

            StartCoroutine(GoalSequenceCoroutine());
        }

        /// <summary>
        /// リプレイする.
        /// </summary>
        public void Replay()
        {
            // InputManager.Instance?.SetInputEnabled(true);
            CurrentResultData = null;
            SceneLoadManager.LoadScene(_gameSceneName);
        }

        /// <summary>
        /// タイトルへ戻る.
        /// </summary>
        public void ReturnToTitle()
        {
            // InputManager.Instance?.SetInputEnabled(true);
            CurrentResultData = null;
            SceneLoadManager.LoadScene(_titleSceneName);
        }

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// ゴール後シーケンス.
        /// </summary>
        /// <returns>コルーチン.</returns>
        private IEnumerator GoalSequenceCoroutine()
        {
            _isGoalSequenceRunning = true;

            // InputManager.Instance?.SetInputEnabled(false);

            _playerController?.EnterGoalSequence();

            BuildResultData();

            yield return new WaitForSeconds(_resultOverlayDelay);

            SceneLoadManager.LoadSceneAdditive(_resultOverlaySceneName);
        }

        /// <summary>
        /// リザルトデータを構築する.
        /// </summary>
        private void BuildResultData()
        {
            var scoreManager = ManagerLocator.ScoreManager;
            var saveManager = ManagerLocator.SaveManager;

            var currentScore = scoreManager != null ? scoreManager.CurrentScore : 0;
            var previousBestScore = saveManager != null ? saveManager.BestScore : 0;
            var isNewRecord = currentScore > previousBestScore;
            var resolvedBestScore = isNewRecord ? currentScore : previousBestScore;

            saveManager?.UpdateBestScore(currentScore);

            CurrentResultData = new ResultData(
                currentScore,
                resolvedBestScore,
                isNewRecord
            );
        }

        #endregion
    }
}