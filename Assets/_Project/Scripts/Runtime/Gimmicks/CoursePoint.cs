using UnityEngine;

using CarTrickRush.Definitions;
using CarTrickRush.Managers;
using CarTrickRush.Characters.Player;

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

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// ポイント種別.
        /// </summary>
        public CoursePointType PointType => _pointType;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void OnTriggerEnter(Collider other)
        {
            if (_isTriggered)
            {
                return;
            }

            if (other.TryGetComponent<PlayerController>(out var playerController))
            {
                _isTriggered = true;
                switch (_pointType)
                {
                    case CoursePointType.Start:
                        break;
                    case CoursePointType.Goal:
                        GameManager.Instance?.OnGoalReached();
                        break;
                    default:
                        break;
                }
                return;
            }
        }

        #endregion
    }
}