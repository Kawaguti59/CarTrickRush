using UnityEngine;

using TMPro;

namespace CarTrickRush.UI
{
    /// =========================================================================================
    /// <summary>
    /// スコア表示を担当するビュー.
    /// </summary>
    /// =========================================================================================
    public sealed class ScoreView : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// スコア表示用のテキスト.
        /// </summary>
        [SerializeField] private TMP_Text _scoreText;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            Validate();
        }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// スコア表示を更新する.
        /// </summary>
        /// <param name="score">スコア</param>
        public void SetScore(int score)
        {
            if (_scoreText == null)
            {
                return;
            }

            _scoreText.text = Mathf.Max(0, score).ToString("N0");
        }

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// バリデーションを行う.
        /// </summary>
        private void Validate()
        {
            if (_scoreText == null)
            {
                Debug.LogWarning($"{nameof(ScoreView)} : ScoreText is not assigned.", this);
            }
        }

        #endregion
    }
}