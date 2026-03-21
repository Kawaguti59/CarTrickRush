using UnityEngine;

namespace CarTrickRush.Core
{
    /// =========================================================================================
    /// <summary>
    /// デバッグ起動用のBoot処理を行うクラス.
    /// </summary>
    /// =========================================================================================
    public sealed class BootstrapDebug : MonoBehaviour
    {
        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            BootstrapUtility.InitializeManagers();
        }

        #endregion
    }
}