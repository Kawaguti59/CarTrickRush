using UnityEngine;

using CarTrickRush.Core.View;

namespace CarTrickRush.UI.Result
{
    /// <summary>リザルト UI のイントロ再生だけを扱う View.</summary>
    public sealed class ResultUIPresenterView : MonoBehaviour, IView
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

        public void Show() 
        { 
        }

        public void Hide()
        { 
        }

        public void PlayIntro(bool newRecord)
        {
            var animationName = newRecord ? "IntroNewRecord" : "Intro";
            _animator?.Play(animationName, layer: 0, normalizedTime: 0.0f);
        }

        public bool IsPlaying()
        {
            if (_animator == null) { return false; }
            return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
        }

        #endregion
    }
}
