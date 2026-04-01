using UnityEngine;

namespace CarTrickRush.Core.Interface
{
    /// =========================================================================================
    /// <summary>
    /// VFX再生の共通インターフェース.
    /// </summary>
    /// =========================================================================================
    public interface IVFXHandler
    {
        /// <summary>
        /// VFXを1回再生する.
        /// </summary>
        /// <param name="prefab">再生するVFXプレハブ.</param>
        /// <param name="position">再生位置.</param>
        /// <param name="parent">生成先の親Transform.</param>
        /// <param name="destroyDelay">自動破棄までの秒数.</param>
        /// <param name="localScale">ルートのローカルスケール. null のときはプレハブのスケールを維持する.</param>
        void PlayOneShot(GameObject prefab, Vector3 position, Transform parent = null, float destroyDelay = 3.0f, Vector3? localScale = null);

        /// <summary>
        /// ループVFXの再生を停止する.
        /// </summary>
        /// <param name="vfx">停止対象のParticleSystem.</param>
        void StopLoop(ParticleSystem vfx);

        /// <summary>
        /// ループVFXの有効/無効を切り替える.
        /// </summary>
        /// <param name="vfx">対象のParticleSystem.</param>
        /// <param name="isActive">有効/無効.</param>
        void SetLoopActive(ParticleSystem vfx, bool isActive);
    }
}
