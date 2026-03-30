using UnityEngine;

using CarTrickRush.Core.Interface;

namespace CarTrickRush.Characters.Player
{
    /// =========================================================================================
    /// <summary>
    /// Player向けVFX再生ハンドラー.
    /// </summary>
    /// =========================================================================================
    public sealed class PlayerVFXHandler : MonoBehaviour, IVFXHandler
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// 通常回転時のVFXプレハブ.
        /// </summary>
        [SerializeField] private GameObject _normalRotationVfxPrefab = default;

        /// <summary>
        /// ボーナス回転時のVFXプレハブ.
        /// </summary>
        [SerializeField] private GameObject _bonusRotationVfxPrefab = default;

        /// <summary>
        /// VFX生成先の親Transform.
        /// </summary>
        [SerializeField] private Transform _spawnParent = default;

        /// <summary>
        /// 常時再生する煙VFX.
        /// </summary>
        [SerializeField] private ParticleSystem _smokeLoopVfx = default;

        /// <summary>
        /// 1回再生VFXの破棄までの秒数.
        /// </summary>
        [SerializeField] private float _oneShotDestroyDelay = 3.0f;

        #endregion

        #region ------------------ Interface Methods ------------------

        public void PlayOneShot(GameObject prefab, Vector3 position, Transform parent = null, float destroyDelay = 3.0f)
        {
            if (prefab == null) { return; }

            var instance = Instantiate(prefab, position, Quaternion.identity, parent);

            if (destroyDelay > 0f)
            {
                Destroy(instance, destroyDelay);
            }
        }

        public void StopLoop(ParticleSystem vfx)
        {
            if (vfx == null) { return; }
            if (!vfx.isPlaying) { return; }
            vfx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        public void SetLoopActive(ParticleSystem vfx, bool isActive)
        {
            if (vfx == null) { return; }

            if (isActive)
            {
                if (!vfx.isPlaying)
                {
                    vfx.Play();
                }
                return;
            }

            StopLoop(vfx);
        }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// 回転VFXを再生する.
        /// </summary>
        /// <param name="position">再生座標.</param>
        /// <param name="isBonus">ボーナス演出か.</param>
        public void PlayRotationVfx(Vector3 position, bool isBonus)
        {
            GameObject prefab = isBonus ? _bonusRotationVfxPrefab : _normalRotationVfxPrefab;
            Transform parent = _spawnParent != null ? _spawnParent : null;
            PlayOneShot(prefab, position, parent, _oneShotDestroyDelay);
        }

        /// <summary>
        /// 常時煙VFXの有効/無効を切り替える.
        /// </summary>
        /// <param name="isActive">有効化する場合はtrue.</param>
        public void SetSmokeActive(bool isActive)
        {
            SetLoopActive(_smokeLoopVfx, isActive);
        }

        #endregion
    }
}
