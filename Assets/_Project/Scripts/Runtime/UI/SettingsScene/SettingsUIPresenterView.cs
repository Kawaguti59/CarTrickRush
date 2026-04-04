using UnityEngine;

using System.Collections;

namespace CarTrickRush.UI.Settings
{
    /// =========================================================================================
    /// <summary>
    /// 設定画面の表示アニメーションを制御するクラス.
    /// </summary>
    /// =========================================================================================
    public sealed class SettingsUIPresenterView : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// アニメーター.
        /// </summary>
        [SerializeField] private Animator _animator = default;

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// 開始アニメーションを再生する.
        /// </summary>
        public void PlayIntro()
        {
            if (_animator == null || string.IsNullOrEmpty("Intro")) { return; }

            _animator.Play("Intro", layer: 0, normalizedTime: 0f);
        }

        /// <summary>
        /// 終了アニメーションを再生する.
        /// </summary>
        /// <returns>コルーチン.</returns>
        public IEnumerator PlayExitRoutine()
        {
            if (_animator == null || string.IsNullOrEmpty("Exit"))
            {
                yield break;
            }

            _animator.Play("Exit", layer: 0, normalizedTime: 0f);
            yield return null;
            yield return new WaitUntil(ExitAnimationFinished);
        }

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// 終了アニメーションが終了したか.
        /// </summary>
        /// <returns>終了アニメーションが終了したかどうか.</returns>
        private bool ExitAnimationFinished()
        {
            if (_animator == null)
            {
                return true;
            }

            var state = _animator.GetCurrentAnimatorStateInfo(0);
            return state.IsName("Exit") && state.normalizedTime >= 1f && !_animator.IsInTransition(0);
        }

        #endregion
    }
}
