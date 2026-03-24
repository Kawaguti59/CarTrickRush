using UnityEngine;
using System.Collections.Generic;

namespace CarTrickRush.Data
{
    /// =========================================================================================
    /// <summary>
    /// トリックボーナスマスタ.
    /// </summary>
    /// =========================================================================================
    [CreateAssetMenu(
        fileName = "TrickBonusMaster",
        menuName = "CarTrickRush/Data/Trick Bonus Master")]
    public sealed class TrickBonusMaster : ScriptableObject
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// トリックボーナスデータリスト.
        /// </summary>
        [SerializeField] private List<TrickBonusData> _bonusList = new();

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// トリックボーナスデータリスト.
        /// </summary>
        public IReadOnlyList<TrickBonusData> BonusList => _bonusList;

        #endregion
    }
}