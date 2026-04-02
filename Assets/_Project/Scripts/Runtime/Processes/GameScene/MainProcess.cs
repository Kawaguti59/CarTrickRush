using UnityEngine;
using Unity.Cinemachine;

using System.Collections;

using CarTrickRush.Characters.Player;
using CarTrickRush.Core;
using CarTrickRush.Data;
using CarTrickRush.Managers;
using CarTrickRush.UI.Result;

namespace CarTrickRush.GameScene
{
    /// =========================================================================================
    /// <summary>
    /// メインプロセス.
    /// </summary>
    /// =========================================================================================
    public sealed class MainProcess : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// インスタンス.
        /// </summary>
        private static MainProcess _instance = default;

        /// <summary>
        /// リザルトオーバーレイ表示遅延.
        /// </summary>
        [SerializeField] private float _resultOverlayDelay = 1.5f;

        /// <summary>
        /// リザルトオーバーレイシーン名.
        /// </summary>
        [SerializeField] private string _resultOverlaySceneName = "ResultScene";

        /// <summary>
        /// ゲーム中のCinemachineカメラ.
        /// </summary>
        [SerializeField] private CinemachineCamera _gameplayCinemachineCamera = default;

        /// <summary>
        /// プレイヤー参照.
        /// </summary>
        private PlayerController _playerController = default;

        /// <summary>
        /// ゴール演出実行中か.
        /// </summary>
        private bool _isGoalSequenceRunning = default;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// インスタンス.
        /// </summary>
        public static MainProcess Instance => _instance;

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
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        private void Start()
        {
            ManagerLocator.ScoreManager?.ResetScore();
            ManagerLocator.AudioManager?.PlayBgm("GameBGM");
        }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// プレイヤー参照を登録する.
        /// </summary>
        /// <param name="playerController">プレイヤー参照.</param>
        public void RegisterPlayer(PlayerController playerController)
        {
            if (playerController == null) { return; }

            _playerController = playerController;
        }

        /// <summary>
        /// ゴール演出を開始する.
        /// </summary>
        public void OnGoalReached()
        {
            if (_isGoalSequenceRunning) { return; }

            StartCoroutine(GoalSequenceCoroutine());
        }

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// ゲームプレイ用カメラの追従・注視ターゲットを外す.
        /// </summary>
        private void StopGameplayCameraFollow()
        {
            if (_gameplayCinemachineCamera == null)
            {
                return;
            }

            _gameplayCinemachineCamera.Follow = null;
            _gameplayCinemachineCamera.LookAt = null;
        }

        /// <summary>
        /// ゴール演出シーケンス.
        /// </summary>
        /// <returns>コルーチン.</returns>
        private IEnumerator GoalSequenceCoroutine()
        {
            _isGoalSequenceRunning = true;

            StopGameplayCameraFollow();
            _playerController?.StartGoal();

            BuildResultData();

            yield return new WaitForSeconds(_resultOverlayDelay);

            SceneLoadManager.LoadSceneAdditive(_resultOverlaySceneName);

            _isGoalSequenceRunning = false;
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

            var data = new ResultData(currentScore, resolvedBestScore);
            ResultSceneSession.SetPending(data, isNewRecord);
        }

        #endregion
    }
}
