using CarTrickRush.Core.View;
using CarTrickRush.Definitions;

namespace CarTrickRush.Characters.Player.Interfaces
{
    /// =========================================================================================
    /// <summary>
    /// プレイヤーViewの共通インターフェース.
    /// </summary>
    /// =========================================================================================
    public interface IPlayerView : IView
    {
        /// <summary>
        /// 走行演出を再生する.
        /// </summary>
        void PlayRun();

        /// <summary>
        /// ジャンプ演出を再生する.
        /// </summary>
        void PlayJump();

        /// <summary>
        /// 着地演出を再生する.
        /// </summary>
        void PlayLand();

        /// <summary>
        /// ペナルティ演出を再生する.
        /// </summary>
        void PlayPenalty();

        /// <summary>
        /// 点滅演出を開始する.
        /// </summary>
        void StartBlink();

        /// <summary>
        /// 点滅演出を停止する.
        /// </summary>
        void StopBlink();

        /// <summary>
        /// 見た目の回転を更新する.
        /// </summary>
        /// <param name="rotationZ">Z回転値.</param>
        void SetRotation(float rotationZ);

        /// <summary>
        /// トリック入力に応じた演出を再生する.
        /// </summary>
        void ApplyTrickRotation(TrickInputType input);
    }
}