using UnityEngine;

using System.Collections.Generic;

namespace CarTrickRush.Data
{
    /// =========================================================================================
    /// <summary>
    /// トリックスコアルールマスタ.
    /// </summary>
    /// =========================================================================================
    [CreateAssetMenu(
        fileName = "TrickScoreRulesMaster",
        menuName = "CarTrickRush/Data/Trick Score Rules Master")]
    public sealed class TrickScoreRulesMaster : ScriptableObject
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// スコアルールデータリスト.
        /// </summary>
        [SerializeField] private List<TrickScoreRuleData> _ruleList = new();

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// スコアルールデータリスト.
        /// </summary>
        public IReadOnlyList<TrickScoreRuleData> RuleList => _ruleList;

        #endregion
    }
}
