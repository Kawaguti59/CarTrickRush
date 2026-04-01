using UnityEngine;

using System.Collections.Generic;

using CarTrickRush.Characters.Player.States;
using CarTrickRush.Characters.Player.Interfaces;
using CarTrickRush.Data;
using CarTrickRush.Debugging;
using CarTrickRush.Definitions;
using CarTrickRush.GameScene;
using CarTrickRush.Managers;

namespace CarTrickRush.Characters.Player
{
    /// =========================================================================================
    /// <summary>
    /// プレイヤー本体制御クラス.
    /// </summary>
    /// =========================================================================================
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PlayerController : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// Rigidbody参照.
        /// </summary>
        [SerializeField] private Rigidbody _rigidbody = default;

        /// <summary>
        /// 着地判定位置.
        /// </summary>
        [SerializeField] private Transform _groundCheckPoint = default;

        /// <summary>
        /// 自動前進速度.
        /// </summary>
        [SerializeField] private float _autoMoveSpeed = 8.0f;

        /// <summary>
        /// 着地判定レイヤー.
        /// </summary>
        [SerializeField] private LayerMask _groundLayer = default;

        /// <summary>
        /// 着地判定距離.
        /// </summary>
        [SerializeField] private float _groundCheckDistance = 0.6f;

        /// <summary>
        /// ペナルティ継続時間.
        /// </summary>
        [SerializeField] private float _penaltyHideTime = 1.0f;

        /// <summary>
        /// 車体再表示後の点滅時間（秒）. 終了後に地上状態へ戻る.
        /// </summary>
        [SerializeField] private float _penaltyBlinkTime = 0.5f;

        /// <summary>
        /// トリックボーナスマスタ.
        /// </summary>
        [SerializeField] private TrickBonusMaster _bonusMaster = default;

        /// <summary>
        /// プレイヤー見た目制御.
        /// </summary>
        [SerializeField] private PlayerView _playerView = default;

        /// <summary>
        /// 現在状態.
        /// </summary>
        private IPlayerState _currentState = default;

        /// <summary>
        /// プレイヤーモデル.
        /// </summary>
        private PlayerModel _playerModel = default;

        /// <summary>
        /// 地上状態インスタンス.
        /// </summary>
        private GroundState _groundState = default;

        /// <summary>
        /// 空中状態インスタンス.
        /// </summary>
        private AirState _airState = default;

        /// <summary>
        /// ペナルティ状態インスタンス.
        /// </summary>
        private PenaltyState _penaltyState = default;
    
        /// <summary>
        /// ゴール中か.
        /// </summary>
        private bool _isGoal = default;

        /// <summary>
        /// トリック失敗ヒットVFXを再生するか.
        /// </summary>
        private bool _isTrickFailImpactVfx = default;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// 現在状態種別.
        /// </summary>
        public PlayerStateType CurrentStateType => _currentState?.StateType ?? PlayerStateType.None;

        /// <summary>
        /// 地上状態参照.
        /// </summary>
        public GroundState GroundState => _groundState;

        /// <summary>
        /// 空中状態参照.
        /// </summary>
        public AirState AirState => _airState;

        /// <summary>
        /// ペナルティ状態参照.
        /// </summary>
        public PenaltyState PenaltyState => _penaltyState;

        /// <summary>
        /// ペナルティ中か.
        /// </summary>
        public bool IsPenalty => CurrentStateType != PlayerStateType.Penalty;

        /// <summary>
        /// 車体非表示フェーズの長さ（秒）.
        /// </summary>
        public float PenaltyHideTime => _penaltyHideTime;

        /// <summary>
        /// 復帰点滅フェーズの長さ（秒）.
        /// </summary>
        public float PenaltyBlinkTime => _penaltyBlinkTime;

        /// <summary>
        /// ゴール演出中か.
        /// </summary>
        public bool IsGoal => _isGoal;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Reset()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Awake()
        {
            // プレイヤーモデルを初期化.
            _playerModel = new PlayerModel();
            _playerModel.Initialize();
            // プレイヤービューを初期化.
            _playerView.Initialize();

            // 地上状態インスタンスを初期化.
            _groundState = new GroundState(this);
            // 空中状態インスタンスを初期化.
            _airState = new AirState(this);
            // ペナルティ状態インスタンスを初期化.
            _penaltyState = new PenaltyState(this);
        }

        private void Start()
        {
            // 初期状態に遷移.
            ChangeState(_groundState);
            // ゲームマネージャーにプレイヤーを登録する.
            MainProcess.Instance?.RegisterPlayer(this);
        }

        private void Update()
        {
            if (IsGoal) { return; }

            _currentState?.HandleInput();
            _currentState?.Update();
        }

        private void FixedUpdate()
        {
            if (IsGoal) { return; }

            _currentState?.FixedUpdate();
        }

        private void OnEnable()
        {
            InputManager.Instance.RotateRightPerformed += OnRotateRight;
            InputManager.Instance.RotateLeftPerformed += OnRotateLeft;
            InputManager.Instance.RotateUpPerformed += OnRotateUp;
            InputManager.Instance.RotateDownPerformed += OnRotateDown;
        }

        private void OnDisable()
        {
            InputManager.Instance.RotateRightPerformed -= OnRotateRight;
            InputManager.Instance.RotateLeftPerformed -= OnRotateLeft;
            InputManager.Instance.RotateUpPerformed -= OnRotateUp;
            InputManager.Instance.RotateDownPerformed -= OnRotateDown;
        }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// 着地処理.
        /// </summary>
        /// <returns>着地したか.</returns>
        public bool OnLanding()
        {
            if (_playerView != null && _playerView.IsTrickRotationAnimationPlaying())
            {
                _playerModel.ClearTrickInputs();
                _isTrickFailImpactVfx = true;
                ChangeState(_penaltyState);
                return false;
            }

            _playerModel.ClearTrickInputs();
            _playerView.PlayLand();
            return true;
        }

        /// <summary>
        /// 状態切り替え処理を行う.
        /// </summary>
        public void ChangeState(IPlayerState nextState)
        {
            if (nextState == null) { return; }

            var prevStateType = CurrentStateType;

            _currentState?.Exit();
            _currentState = nextState;
            _currentState.Enter();

            _playerModel.ChangeState(nextState.StateType);

            if (nextState.StateType == PlayerStateType.Ground)
            {
                if (prevStateType != PlayerStateType.Air)
                {
                    _playerView.PlayRun();
                }

                if (prevStateType == PlayerStateType.Penalty)
                {
                    // ペナルティ終了時の点滅を止める.
                    _playerView.StopBlink();
                }
            }
            else if (nextState.StateType == PlayerStateType.Air)
            {
                if (prevStateType == PlayerStateType.Penalty)
                {
                    // ペナルティ中→空中の遷移は点滅を止める.
                    _playerView.StopBlink();
                }
            }
        }

        /// <summary>
        /// 前進処理.
        /// </summary>
        public void MoveForward()
        {
            Vector3 velocity = _rigidbody.linearVelocity;
            velocity.x = _autoMoveSpeed;
            _rigidbody.linearVelocity = velocity;
        }

        /// <summary>
        /// 着地判定を行う.
        /// </summary>
        public bool IsGrounded()
        {
            return Physics.Raycast(
                _groundCheckPoint.position,
                Vector3.down,
                _groundCheckDistance,
                _groundLayer
            );
        }

        /// <summary>
        /// ジャンプ台接触通知.
        /// </summary>
        /// <param name="jumpPower">ジャンプ力.</param>
        public void OnJumpPadTriggered(float jumpPower)
        {
            if (CurrentStateType != PlayerStateType.Ground) { return; }

            // ジャンプ処理.
            Jump(jumpPower);
            _playerView.PlayJump();
            // 空中状態に遷移.
            ChangeState(_airState);
        }

        /// <summary>
        /// ペナルティ遷移.
        /// </summary>
        public void EnterPenalty()
        {
            _isTrickFailImpactVfx = false;
            ChangeState(_penaltyState);
        }

        /// <summary>
        /// ペナルティ状態に遷移した直後の処理を行う.
        /// </summary>
        internal void StartPenalty()
        {
            PlayTrickFailVfx();
            _playerView?.PlayPenalty();
            _playerView?.SetCarVisualActive(false);
        }

        /// <summary>
        /// ペナルティ復帰の点滅演出を開始する.
        /// </summary>
        internal void StartPenaltyBlink()
        {
            _playerView?.SetCarVisualActive(true);
            _playerView?.StartBlink();
        }

        private void PlayTrickFailVfx()
        {
            if (!_isTrickFailImpactVfx) { return; }

            _isTrickFailImpactVfx = false;
            _playerView?.PlayTrickFailImpactVfx();
        }

        /// <summary>
        /// ゴール演出状態を開始する.
        /// </summary>
        public void StartGoal()
        {
            _isGoal = true;

            _playerView.PlayRun();
        }

        /// <summary>
        /// 前進速度を設定する.
        /// </summary>
        /// <param name="speed">前進速度.</param>
        public void SetAutoMoveSpeed(float speed)
        {
            _autoMoveSpeed = Mathf.Max(0f, speed);
        }

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// ジャンプ処理.
        /// </summary>
        /// <param name="jumpPower">ジャンプ力.</param>
        private void Jump(float jumpPower)
        {
            Vector3 velocity = _rigidbody.linearVelocity;
            velocity.y = 0f;
            _rigidbody.linearVelocity = velocity;
            _rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
        }

        /// <summary>
        /// 右回転入力通知.
        /// </summary>
        private void OnRotateRight() => RequestTrick(TrickInputType.RotateRight);

        /// <summary>
        /// 左回転入力通知.
        /// </summary>
        private void OnRotateLeft() => RequestTrick(TrickInputType.RotateLeft);   

        /// <summary>
        /// 上回転入力通知.
        /// </summary>
        private void OnRotateUp() => RequestTrick(TrickInputType.RotateUp);

        /// <summary>
        /// 下回転入力通知.
        /// </summary>
        private void OnRotateDown() => RequestTrick(TrickInputType.RotateDown);

        /// <summary>
        /// トリック入力リクエスト.
        /// </summary>
        /// <param name="input">トリック入力種別.</param>
        private void RequestTrick(TrickInputType input)
        {
            if (CurrentStateType != PlayerStateType.Air) { return; }
            if (_playerView != null && _playerView.IsTrickRotationAnimationPlaying()) { return; }

            _playerModel.EnqueueTrickInput(input);
            bool isBonus = TryMatchTrickBonus();

            DebugOverlay.ShowRotationLog(GetRotationLogMessage(input));
            _playerView.ApplyTrickRotation(input);
            _playerView.PlayRotationVfx(isBonus);
        }

        /// <summary>
        /// 回転入力の瞬間にトリックボーナス判定を行う.
        /// 将来的にスコア加算/エフェクト表示/スコア表示の起点にする.
        /// </summary>
        private bool TryMatchTrickBonus()
        {
            IReadOnlyList<TrickInputType> queueSnapshot = _playerModel.GetTrickInputsSnapshot();
            var matchedBonus = _playerModel.EvaluateTrick(_bonusMaster.BonusList);

            if (matchedBonus == null) { return false; }

            DebugOverlay.ShowBonusLog(matchedBonus.BonusName, matchedBonus.Score, queueSnapshot);
            return true;
        }

        /// <summary>
        /// 回転入力をHUD表示用メッセージへ変換する.
        /// </summary>
        /// <param name="input">トリック入力種別.</param>
        /// <returns>表示メッセージ.</returns>
        private static string GetRotationLogMessage(TrickInputType input)
        {
            return input switch
            {
                TrickInputType.RotateUp => "[TrickInput] 上回転を実行",
                TrickInputType.RotateDown => "[TrickInput] 下回転を実行",
                TrickInputType.RotateLeft => "[TrickInput] 左回転を実行",
                TrickInputType.RotateRight => "[TrickInput] 右回転を実行",
                _ => null
            };
        }

        private void OnDrawGizmosSelected()
        {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(
                _groundCheckPoint.position,
                _groundCheckPoint.position + Vector3.down * _groundCheckDistance);
            #endif
        }

        #endregion
    }
}