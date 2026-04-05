using UnityEngine;

using CarTrickRush.Core.View;

namespace CarTrickRush.UI.Settings
{
    /// =========================================================================================
    /// <summary>
    /// 設定画面の表示アニメーションを制御するクラス.
    /// </summary>
    /// =========================================================================================
    public sealed class SettingsUIPresenterView : MonoBehaviour, IView
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// アニメーター.
        /// </summary>
        [SerializeField] private Animator _animator = default;

        #endregion

        #region ------------------ Interface Methods ------------------

        public void Initialize()
        {
        }

        /// <summary>
        /// 開始アニメーションを再生する.
        /// </summary>
        public void Show()
        {
            if (_animator == null) { return; }

            _animator.Play("Show", layer: 0, normalizedTime: 0f);
        }

        /// <summary>
        /// 終了アニメーションを再生する.
        /// </summary>
        public void Hide()
        {
            if (_animator == null) { return; }

            _animator.Play("Hide", layer: 0, normalizedTime: 0f);
        }

        public bool IsPlaying()
        {
            if (_animator == null)
            {
                return false;
            }

            var state = _animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("Hide"))
            {
                return state.normalizedTime < 1f || _animator.IsInTransition(0);
            }

            if (state.IsName("Show"))
            {
                return state.normalizedTime < 1f || _animator.IsInTransition(0);
            }

            return false;
        }

        #endregion
    }
}
