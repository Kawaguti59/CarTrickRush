using UnityEngine;

using CarTrickRush.Definitions;
using CarTrickRush.Player.Interfaces;
using CarTrickRush.Player.States;

namespace CarTrickRush.Player
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
        /// 接地判定位置.
        /// </summary>
        [SerializeField] private Transform _groundCheckPoint;

        /// <summary>
        /// 自動前進速度.
        /// </summary>
        [SerializeField] private float _autoMoveSpeed = 8.0f;

        /// <summary>
        /// ジャンプ力.
        /// </summary>
        [SerializeField] private float _jumpForce = 10.0f;

        /// <summary>
        /// 接地判定レイヤー.
        /// </summary>
        [SerializeField] private LayerMask _groundLayer;

        /// <summary>
        /// 接地判定距離.
        /// </summary>
        [SerializeField] private float _groundCheckDistance = 0.6f;

        /// <summary>
        /// ペナルティ継続時間.
        /// </summary>
        [SerializeField] private float _penaltyDuration = 1.5f;

        /// <summary>
        /// 現在状態.
        /// </summary>
        private IPlayerState _currentState;

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

        #endregion

        #region ------------------ Public Methods ------------------

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

            _currentState?.Exit();
            _currentState = nextState;
            _currentState.Enter();
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
        /// 接地判定を行う.
        /// </summary>
        public bool IsGrounded()
        {
            return Physics.Raycast(
                _groundCheckPoint.position,
                Vector3.down,
                _groundCheckDistance,
                _groundLayer);
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

            Jump(jumpPower);
            ChangeState(_airState);
        }

        /// <summary>
        /// ペナルティ遷移.
        /// </summary>
        public void EnterPenalty()
        {
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