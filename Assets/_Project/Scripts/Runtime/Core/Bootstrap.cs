using UnityEngine;

namespace CarTrickRush.Core
{
    /// =========================================================================================
    /// <summary>
    /// 通常起動用のBoot処理を行うクラス.
    /// </summary>
    /// =========================================================================================
    public sealed class Bootstrap : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// 最初に遷移するシーン名.
        /// </summary>
        [SerializeField] private string _firstSceneName = "TitleScene";

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            BootstrapUtility.InitializeManagers();
        }

        private void Start()
        {
            CarTrickRush.Managers.SceneLoadManager.LoadScene(_firstSceneName);
        }

        #endregion
    }
}