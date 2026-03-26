using System.Collections.Generic;
using System.Linq;

using CarTrickRush.Data;
using CarTrickRush.Definitions;

namespace CarTrickRush.Characters.Player
{
    /// =========================================================================================
    /// <summary>
    /// プレイヤー状態データを保持するモデル.
    /// </summary>
    /// =========================================================================================
    public sealed class PlayerModel
    {
        #region ------------------ Fields ------------------
        
        /// <summary>
        /// トリック入力キュー最大件数.
        /// </summary>
        private const int MaxTrickQueueSize = 5;

        /// <summary>
        /// トリック入力リスト.
        /// </summary>
        private readonly List<TrickInputType> _trickInputs = new();

        #endregion

        #region ------------------ Properties ------------------
        
        /// <summary>
        /// 現在の状態種別.
        /// </summary>
        public PlayerStateType CurrentStateType { get; private set; }

        /// <summary>
        /// ペナルティ中か.
        /// </summary>
        public bool IsPenalty { get; private set; }

        /// <summary>
        /// 空中にいるか.
        /// </summary>
        public bool IsAirborne { get; private set; }    
        
        #endregion
        
        #region ------------------ Public Methods ------------------
        
        /// <summary>
        /// モデルを初期化する.
        /// </summary>
        public void Initialize()
        {
            CurrentStateType = PlayerStateType.Ground;
            IsPenalty = false;
            IsAirborne = false;
            _trickInputs.Clear();
        }

        /// <summary>
        /// 状態を更新する.
        /// </summary>
        /// <param name="stateType">状態種別.</param>
        public void ChangeState(PlayerStateType stateType)
        {
            CurrentStateType = stateType;
            IsAirborne = stateType == PlayerStateType.Air;
            IsPenalty = stateType == PlayerStateType.Penalty;
        }

        /// <summary>
        /// トリック入力をキューに追加する.
        /// </summary>
        /// <param name="input"></param>
        public void EnqueueTrickInput(TrickInputType input)
        {
            // キュー方式: 最大5件を超えたら古い入力から破棄する.
            if (_trickInputs.Count >= MaxTrickQueueSize)
            {
                _trickInputs.RemoveAt(0);
            }

            _trickInputs.Add(input);
        }

        /// <summary>
        /// トリック入力キューをクリアする.
        /// </summary>
        public void ClearTrickInputs()
        {
            _trickInputs.Clear();
        }

        /// <summary>
        /// 現在のトリック入力キューのスナップショットを取得する.
        /// </summary>
        public IReadOnlyList<TrickInputType> GetTrickInputsSnapshot()
        {
            return new List<TrickInputType>(_trickInputs);
        }

        /// <summary>
        /// トリックボーナスを評価する.
        /// 一致した場合はキューをクリアして一致したボーナスを返す.
        /// </summary>
        /// <param name="bonusList">評価対象のボーナスリスト.</param>
        /// <returns>一致したボーナス / 一致なしはnull.</returns>
        public TrickBonusData EvaluateTrick(IReadOnlyList<TrickBonusData> bonusList)
        {
            if (bonusList == null)
            {
                return null;
            }

            // コンボ数の大きい順で評価する.
            // 一致した時点で1件のみ採用し同時に複数ボーナスは発火させない.
            foreach (var bonus in bonusList
                         .Where(b => (b?.Sequence?.Count ?? 0) > 0)
                         .OrderByDescending(b => b.Sequence.Count))
            {
                if (IsMatch(bonus.Sequence))
                {
                    // 一致したらキューを空にする.
                    _trickInputs.Clear();
                    return bonus;
                }
            }

            return null;
        }
        
        #endregion
        
        #region ------------------ Private Methods ------------------
        
       /// <summary>
        /// トリック入力シーケンス一致判定.
        /// </summary>
        private bool IsMatch(IReadOnlyList<TrickInputType> sequence)
        {
            int requiredLength = sequence.Count;

            if (_trickInputs.Count < requiredLength)
            {
                return false;
            }

            int startIndex = _trickInputs.Count - requiredLength;

            for (int i = 0; i < requiredLength; i++)
            {
                if (_trickInputs[startIndex + i] != sequence[i])
                {
                    return false;
                }
            }

            return true;
        }
        
        #endregion
    }
}