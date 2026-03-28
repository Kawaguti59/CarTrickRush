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
        [SerializeField] private TMP_Text _scoreText = default;

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// スコア表示を更新する.
        /// </summary>
        /// <param name="score">スコア</param>
        public void SetScore(int score)
        {
            _scoreText.text = Mathf.Max(0, score).ToString("N0");
        }

        #endregion
    }
}