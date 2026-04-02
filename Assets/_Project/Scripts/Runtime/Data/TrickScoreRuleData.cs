using UnityEngine;

namespace CarTrickRush.Data
{
    /// =========================================================================================
    /// <summary>
    /// トリックスコアルール定義.
    /// </summary>
    /// =========================================================================================
    [System.Serializable]
    public sealed class TrickScoreRuleData
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// ルール名.
        /// </summary>
        [SerializeField] private string _ruleName;

        /// <summary>
        /// スコア値.
        /// </summary>
        [SerializeField] private int _score;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// ルール名.
        /// </summary>
        public string RuleName => _ruleName;

        /// <summary>
        /// スコア値.
        /// </summary>
        public int Score => _score;

        #endregion
    }
}
