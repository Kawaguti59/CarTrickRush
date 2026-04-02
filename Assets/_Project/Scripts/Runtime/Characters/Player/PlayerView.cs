using UnityEngine;

using System.Collections;

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

        /// <summary>
        /// 点滅の切り替え間隔（秒）.
        /// </summary>
        [SerializeField] private float _blinkInterval = 0.12f;

        /// <summary>
        /// 点滅演出のコルーチン.
        /// </summary>
        private Coroutine _blinkCoroutine = default;

        /// <summary>
        /// 点滅用Renderer.
        /// </summary>  
        private Renderer[] _blinkRenderers = default;

        /// <summary>
        /// PlayerCar.controller Base Layer (body)のインデックス.
        /// </summary>
        private const int BaseLayerIndex = 0;

        /// <summary>
        /// PlayerCar.controller Wheels Layerのインデックス.
        /// </summary>
        private const int WheelsLayerIndex = 1;

        #endregion

       #region ------------------ Interface Methods ------------------

        public void Initialize()
        {
            if (_visualRoot == null) { return; }
            _visualRoot.SetActive(true);
            CacheBlinkRenderers();
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
        /// トリック回転アニメが再生中かどうか.
        /// </summary>
        /// <returns>トリック回転アニメが再生中かどうか.</returns>
        public bool IsTrickRotationAnimationPlaying()
        {
            if (_animator == null || _trickAnimationNames == null) { return false; }

            // 遷移中かどうかを判定する.
            if (_animator.IsInTransition(0))
            {
                // 現在のアニメーションがトリック回転アニメかどうかを判定する.
                AnimatorStateInfo cur = _animator.GetCurrentAnimatorStateInfo(0);
                AnimatorStateInfo next = _animator.GetNextAnimatorStateInfo(0);
                if (IsTrickRotationState(cur) || IsTrickRotationState(next))
                {
                    return true;
                }
            }

            // 現在のアニメーションがトリック回転アニメかどうかを判定する.
            AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
            if (!IsTrickRotationState(info)) { return false; }

            if (info.loop) { return true; }

            // 再生中かどうかを判定する.
            return info.normalizedTime < 1f;
        }

        /// <summary>
        /// 走行演出を再生する.
        /// </summary>
        public void PlayRun()
        {
            PlayAnimatorState(BaseLayerIndex, "Idle");
            PlayAnimatorState(WheelsLayerIndex, "Drive");
        }

        /// <summary>
        /// ジャンプ演出を再生する.
        /// </summary>
        public void PlayJump()
        {
            // No Jump clip on PlayerCar; keep body/wheels consistent while airborne.
            PlayAnimatorState(BaseLayerIndex, "Idle");
            PlayAnimatorState(WheelsLayerIndex, "Drive");
        }

        /// <summary>
        /// 着地演出を再生する.
        /// </summary>
        public void PlayLand()
        {
            // No Land clip; match ground locomotion until ChangeState runs PlayRun.
            PlayAnimatorState(BaseLayerIndex, "Idle");
            PlayAnimatorState(WheelsLayerIndex, "Drive");
        }

        /// <summary>
        /// ペナルティ演出を再生する.
        /// </summary>
        public void PlayPenalty()
        {
            PlayAnimatorState(BaseLayerIndex, "None");
            PlayAnimatorState(WheelsLayerIndex, "Drive");
        }

        /// <summary>
        /// 点滅演出を開始する.
        /// </summary>
        public void StartBlink()
        {
            StopBlinkInternal();
            if (_visualRoot == null) { return; }

            CacheBlinkRenderers();
            _blinkCoroutine = StartCoroutine(BlinkRoutine());
        }

        /// <summary>
        /// 点滅演出を停止する.
        /// </summary>
        public void StopBlink()
        {
            StopBlinkInternal();
        }

        /// <summary>
        /// Free Racing Car（見た目ルート）の表示を切り替える.
        /// </summary>
        public void SetCarVisualActive(bool isActive)
        {
            if (_visualRoot == null) { return; }
            _visualRoot.SetActive(isActive);
            if (isActive)
            {
                CacheBlinkRenderers();
            }
        }

        /// <summary>
        /// 激突時のヒットVFXを再生する.
        /// </summary>
        public void PlayTrickFailImpactVfx()
        {
            if (_vfxHandler == null) { return; }

            Vector3 pos = _visualRoot != null ? _visualRoot.transform.position : transform.position;
            _vfxHandler.PlayTrickFailImpact(pos, new Vector3(2.0f, 2.0f, 2.0f));
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
                _animator.Play(_trickAnimationNames[input], BaseLayerIndex, 0f);
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
            _vfxHandler.PlayRotationVfx(origin, isBonus, new Vector3(2.0f, 2.0f, 2.0f));
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

        /// <summary>
        /// 指定レイヤーでアニメーションを再生する.
        /// </summary>
        /// <param name="layerIndex">レイヤーのインデックス.</param>
        /// <param name="stateName">アニメーションの名前.</param>
        private void PlayAnimatorState(int layerIndex, string stateName)
        {
            if (_animator == null || string.IsNullOrEmpty(stateName)) { return; }
            if (layerIndex < 0 || layerIndex >= _animator.layerCount) { return; }
            _animator.Play(stateName, layerIndex, 0f);
        }

        /// <summary>
        /// 点滅用Rendererをキャッシュする.
        /// </summary>
        private void CacheBlinkRenderers()
        {
            if (_visualRoot == null) { return; }
            _blinkRenderers = _visualRoot.GetComponentsInChildren<Renderer>(true);
        }

        /// <summary>
        /// 点滅演出を停止する.
        /// </summary>
        private void StopBlinkInternal()
        {
            if (_blinkCoroutine != null)
            {
                StopCoroutine(_blinkCoroutine);
                _blinkCoroutine = default;
            }

            SetAllBlinkRenderersEnabled(true);
        }

        /// <summary>
        /// 点滅用Rendererを有効/無効にする.
        /// </summary>
        /// <param name="enabled">有効化する場合はtrue.</param>
        private void SetAllBlinkRenderersEnabled(bool enabled)
        {
            if (_blinkRenderers == null) { return; }

            for (var i = 0; i < _blinkRenderers.Length; i++)
            {
                Renderer r = _blinkRenderers[i];
                if (r != null)
                {
                    r.enabled = enabled;
                }
            }
        }

        /// <summary>
        /// 点滅演出を実行する.
        /// </summary>
        private IEnumerator BlinkRoutine()
        {
            var wait = new WaitForSeconds(Mathf.Max(0.02f, _blinkInterval));
            var visible = true;

            while (true)
            {
                visible = !visible;
                if (_blinkRenderers != null)
                {
                    for (var i = 0; i < _blinkRenderers.Length; i++)
                    {
                        Renderer r = _blinkRenderers[i];
                        if (r != null)
                        {
                            r.enabled = visible;
                        }
                    }
                }

                yield return wait;
            }
        }

        /// <summary>
        /// トリック回転アニメが再生中かどうか.
        /// </summary>
        /// <param name="info">アニメーターStateInfo.</param>
        /// <returns>トリック回転アニメが再生中かどうか.</returns>
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