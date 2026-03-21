using UnityEngine;

namespace CarTrickRush.Gimmicks.Interfaces
{
    /// =========================================================================================
    /// <summary>
    /// ギミック接触処理の共通インターフェース.
    /// </summary>
    /// =========================================================================================
    public interface IGimmick
    {
        #region ------------------ Interface Methods ------------------

        /// <summary>
        /// 接触開始時の処理.
        /// </summary>
        /// <param name="other">接触相手Collider.</param>
        void OnPlayerEnter(Collider other);

        /// <summary>
        /// 接触中の処理.
        /// </summary>
        /// <param name="other">接触相手Collider.</param>
        void OnPlayerStay(Collider other);

        /// <summary>
        /// 接触終了時の処理.
        /// </summary>
        /// <param name="other">接触相手Collider.</param>
        void OnPlayerExit(Collider other);

        #endregion
    }
}