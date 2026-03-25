using UnityEngine;

using CarTrickRush.Characters.Player.States;
using CarTrickRush.Characters.Player.Interfaces;
using CarTrickRush.Data;
using CarTrickRush.Definitions;
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
        [SerializeField] private Rigidbody _rigidbody;

        /// <summary>
        /// 着地判定位置.
        /// </summary>
        [SerializeField] private Transform _groundCheckPoint;

        /// <summary>
        /// 自動前進速度.
        /// </summary>
        [SerializeField] private float _autoMoveSpeed = 8.0f;

        /// <summary>
        /// 着地判定レイヤー.
        /// </summary>
        [SerializeField] private LayerMask _groundLayer;

        /// <summary>
        /// 着地判定距離.
        /// </summary>
        [SerializeField] private float _groundCheckDistance = 0.6f;

        /// <summary>
        /// ペナルティ継続時間.
        /// </summary>
        [SerializeField] private float _penaltyDuration = 1.5f;

        /// <summary>
        /// トリックボーナスマスタ.
        /// </summary>
        [SerializeField] private TrickBonusMaster _bonusMaster;

        /// <summary>
        /// プレイヤー見た目制御.
        /// </summary>
        [SerializeField] private PlayerView _playerView;

        /// <summary>
        /// 現在状態.
        /// </summary>
        private IPlayerState _currentState;

        /// <summary>
        /// プレイヤーモデル.
        /// </summary>
        private PlayerModel _playerModel;

        /// <summary>
        /// 地上状態インスタンス.
        /// </summary>
        private GroundState _groundState;

        /// <summary>
        /// 空中状態インスタンス.
        /// </summary>
        private AirState _airState;

        /// <summary>
        /// ペナルティ状態インスタンス.
        /// </summary>
        private PenaltyState _penaltyState;

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
        /// ペナルティ時間.
        /// </summary>
        public float PenaltyDuration => _penaltyDuration;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Reset()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Awake()
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }

            _playerModel = new PlayerModel();
            _playerModel.Initialize();

            if (_playerView == null)
            {
                _playerView = GetComponent<PlayerView>();
            }
            if (_playerView == null)
            {
                _playerView = GetComponentInChildren<PlayerView>(true);
            }
            _playerView?.Initialize();

            _groundState = new GroundState(this);
            _airState = new AirState(this);
            _penaltyState = new PenaltyState(this);
        }

        private void Start()
        {
            ChangeState(_groundState);
        }

        private void Update()
        {
            _currentState?.HandleInput();
            _currentState?.Update();
        }

        private void FixedUpdate()
        {
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
        public void OnLanding()
        {
            var matchedBonus = _playerModel.EvaluateTrick(
                _bonusMaster != null ? _bonusMaster.BonusList : null
            );

#if UNITY_EDITOR
            if (matchedBonus != null)
            {
                Debug.Log($"[PlayerController] Bonus! {matchedBonus.BonusName} : {matchedBonus.Score}");
            }
#endif
            _playerModel.ClearTrickInputs();
            _playerView?.PlayLand();
        }

        /// <summary>
        /// 状態切り替え処理を行う.
        /// </summary>
        public void ChangeState(IPlayerState nextState)
        {
            if (nextState == null)
            {
                Debug.LogError("ChangeState failed.");
                return;
            }

            var prevStateType = CurrentStateType;

            _currentState?.Exit();
            _currentState = nextState;
            _currentState.Enter();

            _playerModel.ChangeState(nextState.StateType);

            // 演出は「状態遷移」タイミングで呼び出す（Landing/Jump/Penalty は別箇所でも呼ぶため上書きに注意）
            if (_playerView != null)
            {
                if (nextState.StateType == PlayerStateType.Ground)
                {
                    // 空中→地上の遷移は OnLanding() で Land を再生するので、ここでは Run を抑制する。
                    if (prevStateType != PlayerStateType.Air)
                    {
                        _playerView.PlayRun();
                    }

                    // ペナルティ終了時の点滅を止める。
                    if (prevStateType == PlayerStateType.Penalty)
                    {
                        _playerView.StopBlink();
                    }
                }
                else if (nextState.StateType == PlayerStateType.Penalty)
                {
                    _playerView.PlayPenalty();
                    _playerView.StartBlink();
                }
                else if (nextState.StateType == PlayerStateType.Air)
                {
                    // ペナルティ中→空中の遷移は点滅を止める。
                    if (prevStateType == PlayerStateType.Penalty)
                    {
                        _playerView.StopBlink();
                    }
                }
            }

#if UNITY_EDITOR
            if (_currentState.StateType == PlayerStateType.Ground)
            {
                Debug.Log("[PlayerController] State changed to Ground.");
            }
            else if (_currentState.StateType == PlayerStateType.Air)
            {
                Debug.Log("[PlayerController] State changed to Air.");
            }
#endif
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
            if (CurrentStateType != PlayerStateType.Ground)
            {
                return;
            }

            // ジャンプ処理.
            Jump(jumpPower);
            _playerView?.PlayJump();
            // 空中状態に遷移.
            ChangeState(_airState);
        }

        /// <summary>
        /// ペナルティ遷移.
        /// </summary>
        public void EnterPenalty()
        {
            // ペナルティ状態に遷移.
            ChangeState(_penaltyState);
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
            if (CurrentStateType != PlayerStateType.Air)
            {
                return;
            }

            _playerModel.EnqueueTrickInput(input);

#if UNITY_EDITOR
            Debug.Log($"[PlayerController] Rotation Input: {input}");
#endif
            _playerView?.ApplyTrickRotation(input);
        }

        private void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            if (_groundCheckPoint == null) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(
                _groundCheckPoint.position,
                _groundCheckPoint.position + Vector3.down * _groundCheckDistance);
#endif
        }

        #endregion
    }
}