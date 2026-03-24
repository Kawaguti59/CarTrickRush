using UnityEngine;
using System.Collections.Generic;

using CarTrickRush.Definitions;

namespace CarTrickRush.Data
{
    /// =========================================================================================
    /// <summary>
    /// トリックボーナス定義.
    /// </summary>
    /// =========================================================================================
    [System.Serializable]
    public class TrickBonusData
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// トリックボーナス名.
        /// </summary>
        [SerializeField] private string _bonusName;

        /// <summary>
        /// トリックボーナススコア.
        /// </summary>
        [SerializeField] private int _score;

        /// <summary>
        /// トリックボーナス入力シーケンス.
        /// </summary>
        [SerializeField] private List<TrickInputType> _sequence = new();

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// トリックボーナス名.
        /// </summary>  
        public string BonusName => _bonusName;

        /// <summary>
        /// トリックボーナススコア.
        /// </summary>
        public int Score => _score;

        /// <summary>
        /// トリックボーナス入力シーケンス.
        /// </summary>
        public IReadOnlyList<TrickInputType> Sequence => _sequence;

        #endregion
    }
}