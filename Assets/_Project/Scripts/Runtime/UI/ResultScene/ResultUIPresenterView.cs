using UnityEngine;

using CarTrickRush.Core.View;

namespace CarTrickRush.UI.Result
{
    /// =========================================================================================
    /// <summary>
    /// リザルト画面の表示を扱うView.
    /// </summary>
    /// =========================================================================================
    public sealed class ResultUIPresenterView : MonoBehaviour, IView
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// アニメーター.
        /// </summary>
        [SerializeField] private Animator _animator = default;

        /// <summary>
        /// 新記録かどうか.
        /// </summary>
        private bool _isNewRecord;

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// 新記録かどうかを設定する.
        /// </summary>
        /// <param name="isNewRecord">新記録かどうか.</param>
        public void SetIsNewRecord(bool isNewRecord)
        {
            _isNewRecord = isNewRecord;
        }

        #endregion

        #region ------------------ Interface Methods ------------------

        public void Initialize()
        {
        }

        public void Show()
        {
            var animationName = _isNewRecord ? "ShowNewRecord" : "Show";
            _animator?.Play(animationName, layer: 0, normalizedTime: 0.0f);
        }

        public void Hide()
        {
        }

        public bool IsPlaying()
        {
            if (_animator == null) { return false; }
            return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
        }

        #endregion
    }
}
