using Unity.Cinemachine;
using UnityEngine;

namespace CarTrickRush.GameScene
{
    /// =========================================================================================
    /// <summary>
    /// カメラの途切れを抑えるクラス.
    /// </summary>
    /// =========================================================================================
    [RequireComponent(typeof(CinemachineCamera))]
    [RequireComponent(typeof(CinemachineFollow))]
    public sealed class GameplayFollowVerticalDamping : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// カメラコンポーネント.
        /// </summary>
        [SerializeField] private CinemachineCamera _cinemachineCamera = default;
        
        /// <summary>
        /// フォローコンポーネント.
        /// </summary>
        [SerializeField] private CinemachineFollow _follow = default;

        /// <summary>
        /// ターゲットのRigidbody.
        /// </summary>
        [SerializeField] private Rigidbody _targetRigidbody = default;

        /// <summary>
        /// 中立時の PositionDamping.y.
        /// </summary>
        [SerializeField] private float _positionDampingYNeutral = 1.1f;

        /// <summary>
        /// 上昇時の PositionDamping.y.
        /// </summary>
        [SerializeField] private float _positionDampingYAscending = 2.75f;

        /// <summary>
        /// 落下時の PositionDamping.y.
        /// </summary>  
        [SerializeField] private float _positionDampingYFalling = 0.2f;

        /// <summary>
        /// この上昇速度 (m/s) で上昇用ダンピングが最大になる.
        /// </summary>
        [SerializeField] private float _riseFullAtSpeed = 3f;

        /// <summary>
        /// この落下速度 (m/s) で落下用ダンピングが最小になる.
        /// </summary>
        [SerializeField] private float _fallFullAtSpeed = 5.5f;

        /// <summary>
        /// PositionDamping.y を目標値へ近づけるスムージング時間 (秒).
        /// </summary>
        [SerializeField] private float _dampingSmoothTime = 0.2f;

        /// <summary>
        /// 高速落下時に FollowOffset.y に加える追加値 (負でカメラが下がり足元が見える).
        /// </summary>
        [SerializeField] private float _followYExtraWhenFalling = -0.85f;

        /// <summary>
        /// この落下速度で上記の追加オフセットが最大になる.
        /// </summary>
        [SerializeField] private float _followYExtraFullAtSpeed = 5.5f;

        /// <summary>
        /// FollowOffset.y の追加値を目標値へ近づけるスムージング時間 (秒).
        /// </summary>
        [SerializeField] private float _followYExtraSmoothTime = 0.14f;

        /// <summary>
        /// 現在の PositionDamping.y.
        /// </summary>
        private float _currentYDamping = default;

        /// <summary>
        /// PositionDamping.y を目標値へ近づけるスムージング速度.
        /// </summary>
        private float _dampingSmoothVelocity = default;

        /// <summary>
        /// 現在の FollowOffset.y の追加値.
        /// </summary>
        private float _currentFollowYExtra = default;

        /// <summary>
        /// FollowOffset.y の追加値を目標値へ近づけるスムージング速度.
        /// </summary>
        private float _followYExtraSmoothVelocity = default;

        /// <summary>
        /// 基準となる FollowOffset.y.
        /// </summary>
        private Vector3 _baseFollowOffset = default;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            if (_cinemachineCamera == null)
            {
                _cinemachineCamera = GetComponent<CinemachineCamera>();
            }

            if (_follow == null)
            {
                _follow = GetComponent<CinemachineFollow>();
            }

            if (_targetRigidbody == null && _cinemachineCamera != null
                                        && _cinemachineCamera.Target.TrackingTarget != null)
            {
                _targetRigidbody = _cinemachineCamera.Target.TrackingTarget.GetComponentInParent<Rigidbody>();
            }

            var ts = _follow.TrackerSettings;
            _currentYDamping = ts.PositionDamping.y;
            _baseFollowOffset = _follow.FollowOffset;
        }

        private void LateUpdate()
        {
            if (_follow == null || _targetRigidbody == null) { return; }

            // 縦速度を取得する.
            var vy = _targetRigidbody.linearVelocity.y;
            // 縦速度に応じて目標となる PositionDamping.y を計算する.
            var targetDamping = ComputeTargetDamping(vy);
            // PositionDamping.y を目標値へ近づけるスムージング処理.
            _currentYDamping = Mathf.SmoothDamp(
                _currentYDamping,
                targetDamping,
                ref _dampingSmoothVelocity,
                _dampingSmoothTime,
                Mathf.Infinity,
                Time.deltaTime);

            // 縦速度に応じて目標となる FollowOffset.y の追加値を計算する.  
            var targetFollowYExtra = vy < 0f
                ? Mathf.Lerp(0f, _followYExtraWhenFalling, Mathf.Clamp01(-vy / Mathf.Max(0.01f, _followYExtraFullAtSpeed)))
                : 0f;
            // FollowOffset.y の追加値を目標値へ近づけるスムージング処理を行う.
            _currentFollowYExtra = Mathf.SmoothDamp(
                _currentFollowYExtra,
                targetFollowYExtra,
                ref _followYExtraSmoothVelocity,
                _followYExtraSmoothTime,
                Mathf.Infinity,
                Time.deltaTime);

            // PositionDamping.y と FollowOffset.y の追加値を設定する.
            var ts = _follow.TrackerSettings;
            // PositionDamping.y を設定する.
            var pd = ts.PositionDamping;
            pd.y = _currentYDamping;
            // PositionDamping.y を設定する.
            ts.PositionDamping = pd;
            // TrackerSettings を設定する.
            _follow.TrackerSettings = ts;

            // FollowOffset.y の追加値を設定する.
            var offset = _baseFollowOffset;
            offset.y += _currentFollowYExtra;
            _follow.FollowOffset = offset;
        }

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// 縦速度に応じて目標となる PositionDamping.y を計算する.
        /// </summary>
        /// <param name="vy">縦速度 (m/s).</param>
        /// <returns>目標となる PositionDamping.y.</returns>
        private float ComputeTargetDamping(float vy)
        {
            if (vy >= 0f)
            {
                var riseT = Mathf.Clamp01(vy / Mathf.Max(0.01f, _riseFullAtSpeed));
                return Mathf.Lerp(_positionDampingYNeutral, _positionDampingYAscending, riseT);
            }

            var fallT = Mathf.Clamp01(-vy / Mathf.Max(0.01f, _fallFullAtSpeed));
            return Mathf.Lerp(_positionDampingYNeutral, _positionDampingYFalling, fallT);
        }

        #endregion
    }
}
