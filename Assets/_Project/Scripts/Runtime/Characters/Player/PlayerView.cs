using UnityEngine;

using CarTrickRush.Core.View;
using CarTrickRush.Definitions;

namespace CarTrickRush.Character.Player.View
{
    /// =========================================================================================
    /// <summary>
    /// プレイヤーの見た目制御を担当するView.
    /// </summary>
    /// =========================================================================================
    public sealed class PlayerView : MonoBehaviour, IPlayerView
    {
        #region ------------------ Fields ------------------
        
        /// <summary>
        /// プレイヤー見た目ルート.
        /// </summary>
        [SerializeField] private GameObject _visualRoot;

        /// <summary>
        /// アニメーター参照.
        /// </summary>
        [SerializeField] private Animator _animator;

        [Header("Trick Animation Names")]
        [SerializeField] private string _rotateRightAnimationName = "TrickRotateRight";
        [SerializeField] private string _rotateLeftAnimationName = "TrickRotateLeft";
        [SerializeField] private string _rotateUpAnimationName = "TrickRotateUp";
        [SerializeField] private string _rotateDownAnimationName = "TrickRotateDown";
        
        #endregion

       #region ------------------ Interface Methods ------------------

        /// <summary>
        /// 走行演出を再生する.
        /// </summary>
        public void PlayRun()
        {
            if (_animator == null)
            {
                return;
            }

            _animator.Play("Run");
        }

        /// <summary>
        /// ジャンプ演出を再生する.
        /// </summary>
        public void PlayJump()
        {
            if (_animator == null)
            {
                return;
            }

            _animator.Play("Jump");
        }

        /// <summary>
        /// 着地演出を再生する.
        /// </summary>
        public void PlayLand()
        {
            if (_animator == null)
            {
                return;
            }

            _animator.Play("Land");
        }

        /// <summary>
        /// ペナルティ演出を再生する.
        /// </summary>
        public void PlayPenalty()
        {
            if (_animator == null)
            {
                return;
            }

            _animator.Play("Penalty");
        }

        /// <summary>
        /// 点滅演出を開始する.
        /// </summary>
        public void StartBlink()
        {
        }

        /// <summary>
        /// 点滅演出を停止する.
        /// </summary>
        public void StopBlink()
        {
        }

        /// <summary>
        /// 見た目の回転を更新する.
        /// </summary>
        /// <param name="rotationZ">Z回転値.</param>
        public void SetRotation(float rotationZ)
        {
            if (_visualRoot == null)
            {
                return;
            }

            _visualRoot.transform.localRotation = Quaternion.Euler(0f, 0f, rotationZ);
        }

        /// <summary>
        /// トリック入力に応じた演出を再生する.
        /// </summary>
        public void ApplyTrickRotation(TrickInputType input)
        {
            if (_animator == null)
            {
                return;
            }

            switch (input)
            {
                case TrickInputType.RotateRight:
                    PlayAnimation(_rotateRightAnimationName);
                    break;
                case TrickInputType.RotateLeft:
                    PlayAnimation(_rotateLeftAnimationName);
                    break;
                case TrickInputType.RotateUp:
                    PlayAnimation(_rotateUpAnimationName);
                    break;
                case TrickInputType.RotateDown:
                    PlayAnimation(_rotateDownAnimationName);
                    break;
                default:
#if UNITY_EDITOR
                    Debug.LogError($"Invalid input: {input}");
#endif
                    break;
            }
        }
        #endregion

        #region ------------------ Public Methods ------------------
        
        /// <summary>
        /// Viewを初期化する.
        /// </summary>
        public void Initialize()
        {
            if (_visualRoot != null)
            {
                _visualRoot.SetActive(true);
            }
        }

        /// <summary>
        /// Viewを表示する.
        /// </summary>
        public void Show()
        {
            if (_visualRoot != null)
            {
                _visualRoot.SetActive(true);
            }
        }

        /// <summary>
        /// Viewを非表示にする.
        /// </summary>
        public void Hide()
        {
            if (_visualRoot != null)
            {
                _visualRoot.SetActive(false);
            }
        }

        private void PlayAnimation(string animationName)
        {
            if (string.IsNullOrWhiteSpace(animationName))
            {
                return;
            }

            _animator.Play(animationName);
        }
        
        #endregion
    }
}