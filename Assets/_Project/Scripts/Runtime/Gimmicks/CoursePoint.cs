using CarTrickRush.Definitions;
using CarTrickRush.Managers;
using UnityEngine;

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
        private bool _isTriggered;

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

            if (_pointType != CoursePointType.Goal)
            {
                return;
            }

            if (other.TryGetComponent<Player.PlayerController>(out _) == false)
            {
                return;
            }

            _isTriggered = true;
            SceneLoadManager.LoadScene("ResultScene");
        }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// 開始地点座標を返します.
        /// </summary>
        /// <returns>開始地点座標.</returns>
        public Vector3 GetPointPosition()
        {
            return transform.position;
        }

        #endregion
    }
}