using UnityEngine;
using Unity.Cinemachine;

using Cysharp.Threading.Tasks;
using R3;

using VContainer;

using CarTrickRush.Characters.Player;
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
        /// リザルトオーバーレイ表示遅延.
        /// </summary>
        [SerializeField] private float _resultOverlayDelay = 1.5f;

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

        /// <summary>
        /// リザルトオーバーレイシーン名.
        /// </summary>
        private string _resultOverlaySceneName = "ResultScene";

        /// <summary>
        /// ポーズオーバーレイシーン名.
        /// </summary>
        private string _pauseOverlaySceneName = "PauseScene";

        /// <summary>
        /// 入力購読の破棄管理.
        /// </summary>
        private CompositeDisposable _inputSubscriptions = default;

        private InputManager _inputManager = default;
        private ScoreManager _scoreManager = default;
        private SaveManager _saveManager = default;
        private AudioManager _audioManager = default;
        private SceneLoadManager _sceneLoadManager = default;

        #endregion

        #region ------------------ VContainer Methods ------------------

        [Inject]
        void Construct(
            InputManager inputManager,
            ScoreManager scoreManager,
            SaveManager saveManager,
            AudioManager audioManager,
            SceneLoadManager sceneLoadManager)
        {
            _inputManager = inputManager;
            _scoreManager = scoreManager;
            _saveManager = saveManager;
            _audioManager = audioManager;
            _sceneLoadManager = sceneLoadManager;
        }

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void OnEnable()
        {
            _inputSubscriptions?.Dispose();
            _inputSubscriptions = new CompositeDisposable();

            if (_inputManager == null) { return; }

            _inputManager.PausePerformed.Subscribe(_ => HandlePausePerformed()).AddTo(_inputSubscriptions);
        }

        private void OnDisable()
        {
            _inputSubscriptions?.Dispose();
            _inputSubscriptions = null;
        }

        private void Start()
        {
            _scoreManager?.ResetScore();
            _audioManager?.PlayBgm("GameBGM");
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
        /// ポーズシーンを開く.
        /// </summary>
        public void OpenPauseOverlay()
        {
            if (_isGoalSequenceRunning) { return; }

            if (SceneLoadManager.IsSceneLoaded(_resultOverlaySceneName)) { return; }

            if (SceneLoadManager.IsSceneLoaded(_pauseOverlaySceneName)) { return; }

            _sceneLoadManager.LoadSceneAdditive(_pauseOverlaySceneName);
        }

        /// <summary>
        /// ゴール演出を開始する.
        /// </summary>
        public void OnGoalReached()
        {
            if (_isGoalSequenceRunning) { return; }

            GoalSequenceAsync(destroyCancellationToken).Forget();
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
        /// <param name="cancellationToken">キャンセルトークン.</param>
        private async UniTaskVoid GoalSequenceAsync(System.Threading.CancellationToken cancellationToken)
        {
            // ゴール演出実行中フラグを設定する.
            _isGoalSequenceRunning = true;

            // ゴール効果音を再生する.
            _audioManager?.PlaySe("GoalCheer");

            // カメラの追従を解除する.
            StopGameplayCameraFollow();
            _playerController?.StartGoal();

            // リザルトデータを構築する.
            BuildResultData();

            // リザルトオーバーレイシーンを読み込む.
            await UniTask.Delay(
                System.TimeSpan.FromSeconds(_resultOverlayDelay),
                cancellationToken: cancellationToken);
            _sceneLoadManager.LoadSceneAdditive(_resultOverlaySceneName);

            // ゴール演出実行中フラグを解除する.
            _isGoalSequenceRunning = false;
        }

        /// <summary>
        /// ポーズ画面を開く／既に開いていれば閉じる.
        /// </summary>
        private void HandlePausePerformed()
        {
            if (SceneLoadManager.IsSceneLoaded(_pauseOverlaySceneName))
            {
                _sceneLoadManager.UnloadScene(_pauseOverlaySceneName);
                return;
            }

            OpenPauseOverlay();
        }

        /// <summary>
        /// リザルトデータを構築する.
        /// </summary>
        private void BuildResultData()
        {
            var currentScore = _scoreManager != null ? _scoreManager.CurrentScore : 0;
            var previousBestScore = _saveManager != null ? _saveManager.BestScore : 0;
            var isNewRecord = currentScore > previousBestScore;
            var resolvedBestScore = isNewRecord ? currentScore : previousBestScore;

            _saveManager?.UpdateBestScore(currentScore);

            var data = new ResultData(currentScore, resolvedBestScore);
            ResultSceneSession.SetPending(data, isNewRecord);
        }

        #endregion
    }
}
