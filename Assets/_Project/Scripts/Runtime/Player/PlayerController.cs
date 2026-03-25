using UnityEngine;
using System.Collections.Generic;

using CarTrickRush.Data;
using CarTrickRush.Definitions;
using CarTrickRush.Managers;
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
        /// 回転速度.
        /// </summary>
        [SerializeField] private float _rotationSpeed = 360f;

        /// <summary>
        /// トリックボーナスマスタ.
        /// </summary>
        [SerializeField] private TrickBonusMaster _bonusMaster;

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

        /// <summary>
        /// トリック入力リスト.
        /// </summary>
        private readonly List<TrickInputType> _trickInputs = new();

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
            EvaluateTrick();
            _trickInputs.Clear();
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

            _currentState?.Exit();
            _currentState = nextState;
            _currentState.Enter();

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

            _trickInputs.Add(input);

#if UNITY_EDITOR
            Debug.Log($"[PlayerController] Rotation Input: {input}");
#endif
            ApplyRotation(input);
        }

        /// <summary>
        /// 回転処理.
        /// </summary>
        /// <param name="input">トリック入力種別.</param>
        private void ApplyRotation(TrickInputType input)
        {
            switch (input)
            {
                // 右回転.
                case TrickInputType.RotateRight:
                    {
                        Vector3 axis = Vector3.forward;
                        float angle = -_rotationSpeed * Time.deltaTime;
                        LogRotationApplied(input, axis, angle);
                        transform.Rotate(axis, angle);
                    }
                    break;
                // 左回転.
                case TrickInputType.RotateLeft:
                    {
                        Vector3 axis = Vector3.forward;
                        float angle = _rotationSpeed * Time.deltaTime;
                        LogRotationApplied(input, axis, angle);
                        transform.Rotate(axis, angle);
                    }
                    break;
                // 上回転.
                case TrickInputType.RotateUp:
                    {
                        Vector3 axis = Vector3.right;
                        float angle = _rotationSpeed * Time.deltaTime;
                        LogRotationApplied(input, axis, angle);
                        transform.Rotate(axis, angle);
                    }
                    break;
                // 下回転.
                case TrickInputType.RotateDown:
                    {
                        Vector3 axis = Vector3.right;
                        float angle = -_rotationSpeed * Time.deltaTime;
                        LogRotationApplied(input, axis, angle);
                        transform.Rotate(axis, angle);
                    }
                    break;
                // 未定義.
                default:
#if UNITY_EDITOR
                    Debug.LogError($"Invalid input: {input}");
#endif
                    break;
            }
        }

        /// <summary>
        /// 空中中に回転が適用された際のログ出力.
        /// </summary>
        private void LogRotationApplied(TrickInputType input, Vector3 axis, float angleDegrees)
        {
#if UNITY_EDITOR
            Debug.Log($"[PlayerController] Rotation Applied: {input}, axis={axis}, angle={angleDegrees}deg");
#endif
        }

        /// <summary>
        /// トリック評価.
        /// </summary>
        private void EvaluateTrick()
        {
            if (_bonusMaster == null)
            {
                return;
            }

            foreach (var bonus in _bonusMaster.BonusList)
            {
                if (IsMatch(bonus.Sequence))
                {
#if UNITY_EDITOR
                    Debug.Log($"[PlayerController] Bonus! {bonus.BonusName} : {bonus.Score}");
#endif
                    break;
                }
            }
        }

        /// <summary>
        /// トリック入力シーケンス一致判定.
        /// </summary>
        /// <param name="sequence">トリック入力シーケンス.</param>
        /// <returns>一致した場合はtrue, 一致しない場合はfalse.</returns>
        private bool IsMatch(IReadOnlyList<TrickInputType> sequence)
        {
            if (_trickInputs.Count != sequence.Count)
            {
                return false;
            }

            for (int i = 0; i < sequence.Count; i++)
            {
                if (_trickInputs[i] != sequence[i])
                {
                    return false;
                }
            }

            return true;
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