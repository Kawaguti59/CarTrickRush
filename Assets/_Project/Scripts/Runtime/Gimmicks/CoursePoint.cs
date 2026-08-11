using UnityEngine;

using VContainer;

using CarTrickRush.Characters.Player;
using CarTrickRush.Definitions;
using CarTrickRush.GameScene;

namespace CarTrickRush.Gimmicks
{
    /// =========================================================================================
    /// <summary>
    /// コース上の各種ポイント制御クラス.
    /// </summary>
    /// =========================================================================================
    [RequireComponent(typeof(Collider))]
    public sealed class CoursePoint : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// ポイント種別.
        /// </summary>
        [SerializeField] private CoursePointType _pointType = CoursePointType.None;

        /// <summary>
        /// 多重実行防止フラグ.
        /// </summary>
        private bool _isTriggered = default;

        private MainProcess _mainProcess = default;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// ポイント種別.
        /// </summary>
        public CoursePointType PointType => _pointType;

        #endregion

        #region ------------------ VContainer Methods ------------------

        [Inject]
        void Construct(MainProcess mainProcess)
        {
            _mainProcess = mainProcess;
        }

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void OnTriggerEnter(Collider other)
        {
            if (_isTriggered)
            {
                return;
            }

            if (other.TryGetComponent<PlayerController>(out _))
            {
                _isTriggered = true;
                switch (_pointType)
                {
                    case CoursePointType.Start:
                        break;
                    case CoursePointType.Goal:
                        _mainProcess?.OnGoalReached();
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion
    }
}
