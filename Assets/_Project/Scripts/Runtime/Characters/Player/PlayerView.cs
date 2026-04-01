using UnityEngine;

using CarTrickRush.Characters.Player.Interfaces;
using CarTrickRush.Core;
using CarTrickRush.Definitions;

namespace CarTrickRush.Characters.Player
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
        [SerializeField] private GameObject _visualRoot = default;

        /// <summary>
        /// アニメーター参照.
        /// </summary>
        [SerializeField] private Animator _animator = default;

        /// <summary>
        /// 入力に応じたアニメーション名.
        /// </summary>
        [SerializeField] private InspectableMap<TrickInputType, string> _trickAnimationNames = default;

        /// <summary>
        /// Player用VFXハンドラー.
        /// </summary>
        [SerializeField] private PlayerVFXHandler _vfxHandler = default;

        #endregion

       #region ------------------ Interface Methods ------------------

        public void Initialize()
        {
            if (_visualRoot == null) { return; }
            _visualRoot.SetActive(true);
        }

        public void Show()
        {
            if (_visualRoot == null) { return; }
            _visualRoot.SetActive(true);
        }

        public void Hide()
        {
            if (_visualRoot == null) { return; }
            _visualRoot.SetActive(false);
        }

        public bool IsPlaying()
        {
            if (_animator == null) { return false; }
            return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f;
        }

        /// <summary>
        /// トリック回転アニメが再生中（遷移中含む）かどうか.
        /// </summary>
        public bool IsTrickRotationAnimationPlaying()
        {
            if (_animator == null || _trickAnimationNames == null) { return false; }

            if (_animator.IsInTransition(0))
            {
                AnimatorStateInfo cur = _animator.GetCurrentAnimatorStateInfo(0);
                AnimatorStateInfo next = _animator.GetNextAnimatorStateInfo(0);
                if (IsTrickRotationState(cur) || IsTrickRotationState(next))
                {
                    return true;
                }
            }

            AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
            if (!IsTrickRotationState(info))
            {
                return false;
            }

            if (info.loop)
            {
                return true;
            }

            return info.normalizedTime < 1f;
        }

        /// <summary>
        /// 走行演出を再生する.
        /// </summary>
        public void PlayRun()
        {
            if (_animator == null) { return; }
            _animator.Play("Run");
        }

        /// <summary>
        /// ジャンプ演出を再生する.
        /// </summary>
        public void PlayJump()
        {
            if (_animator == null) { return; }
            _animator.Play("Jump");
        }

        /// <summary>
        /// 着地演出を再生する.
        /// </summary>
        public void PlayLand()
        {
            if (_animator == null) { return; }
            _animator.Play("Land");
        }

        /// <summary>
        /// ペナルティ演出を再生する.
        /// </summary>
        public void PlayPenalty()
        {
            if (_animator == null) { return; }
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
            if (_visualRoot == null) { return; }
            _visualRoot.transform.localRotation = Quaternion.Euler(0f, 0f, rotationZ);
        }

        /// <summary>
        /// トリック入力に応じた演出を再生する.
        /// </summary>
        public void ApplyTrickRotation(TrickInputType input)
        {
            if (_animator == null) { return; }
            if (_trickAnimationNames != null)
            {
                _animator?.Play(_trickAnimationNames[input]);
                return;
            }
        }

        /// <summary>
        /// 回転VFXを再生する.
        /// </summary>
        /// <param name="isBonus">ボーナス演出か.</param>
        public void PlayRotationVfx(bool isBonus)
        {
            if (_vfxHandler == null) { return; }

            var origin = _visualRoot != null ? _visualRoot.transform.position : transform.position;
            _vfxHandler.PlayRotationVfx(origin, isBonus);
        }

        /// <summary>
        /// 常時煙VFXの有効/無効を切り替える.
        /// </summary>
        /// <param name="isActive">有効化する場合はtrue.</param>
        public void SetSmokeActive(bool isActive)
        {
            if (_vfxHandler == null) { return; }
            _vfxHandler.SetSmokeActive(isActive);
        }

        #endregion

        #region ------------------ Private Methods ------------------

        private bool IsTrickRotationState(AnimatorStateInfo info)
        {
            foreach (var pair in _trickAnimationNames)
            {
                if (info.IsName(pair.Value))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}