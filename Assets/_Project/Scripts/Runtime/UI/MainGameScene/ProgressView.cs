using UnityEngine;
using UnityEngine.UI;

namespace CarTrickRush.UI
{
    /// =========================================================================================
    /// <summary>
    /// 進捗表示を担当するビュー.
    /// </summary>
    /// =========================================================================================
    public sealed class ProgressView : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// 進捗表示用のスクロールバー.
        /// </summary>
        [SerializeField] private Scrollbar _progressBar = default;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------
        private void Awake()
        {
            SetupProgressBar();
        }
        
        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// 進捗表示を更新する.
        /// </summary>
        /// <param name="progressRate">進捗率</param>
        public void SetProgress(float progressRate)
        {
            _progressBar.size = 0f;
            _progressBar.value = Mathf.Clamp01(progressRate);
        }

        /// <summary>
        /// スクロールバーを初期化する.
        /// </summary>
        private void SetupProgressBar()
        {
            _progressBar.interactable = false;
            _progressBar.size = 0f;
            _progressBar.value = 0f;
        }
        #endregion
    }
}