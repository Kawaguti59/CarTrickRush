using UnityEngine;

using CarTrickRush.Character.Player.View;

namespace CarTrickRush.Character.Player
{
    /// <summary>
    /// プレイヤーの見た目制御を担当するView.
    /// </summary>
    public sealed class PlayerView : MonoBehaviour, IPlayerView
    {
        [SerializeField] private GameObject _visualRoot;
        [SerializeField] private Animator _animator;

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
    }
}