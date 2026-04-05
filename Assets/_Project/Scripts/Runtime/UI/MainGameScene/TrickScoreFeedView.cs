using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

using CarTrickRush.Definitions;

namespace CarTrickRush.UI
{
    /// =========================================================================================
    /// <summary>
    /// トリックスコアのフィード表示.
    /// </summary>
    /// =========================================================================================
    public sealed class TrickScoreFeedView : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// ボーナス行を表示する親のRectTransform.
        /// </summary>
        [SerializeField] private RectTransform _rowParent = default;

        /// <summary>
        /// 通常スコア行プレハブ.
        /// </summary>
        [FormerlySerializedAs("_rowPrefab")]
        [SerializeField] private TrickScoreRowView _normalRowPrefab = default;

        /// <summary>
        /// ボーナススコア行プレハブ (未設定時は通常プレハブを使う).
        /// </summary>
        [SerializeField] private TrickScoreRowView _bonusRowPrefab = default;

        /// <summary>
        /// 最大表示件数.
        /// </summary>
        [SerializeField] private int _maxRows = 3;

        /// <summary>
        /// 表示時間.
        /// </summary>
        [SerializeField] private float _visibleDuration = 2f;

        /// <summary>
        /// 前の行に適用する幅・高さの倍率.
        /// </summary>
        [SerializeField] private float _previousSetRowFactor = 0.8f;

        /// <summary>
        /// 現在のセットで追加した行数.
        /// </summary>
        private int _setRowCount = default;

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// トリックスコアをフィードに追加する.
        /// </summary>
        /// <param name="displayName">表示名.</param>
        /// <param name="addValue">加算値.</param>
        /// <param name="kind">スコア行の種別.</param>
        /// <param name="endGroup">同一セットの最後かどうか.</param>
        public void Push(string displayName, int addValue, TrickScoreRowKind kind, bool endGroup = true)
        {
            var prefab = GetRowPrefab(kind);
            if (_rowParent == null || prefab == null) { return; }

            var limit = Mathf.Max(1, _maxRows);
            var maxBeforeAdd = limit - 1;
            var removeCount = Mathf.Max(0, _rowParent.childCount - maxBeforeAdd);
            for (var i = 0; i < removeCount; i++)
            {
                if (_rowParent.childCount == 0) { break; }

                var oldest = _rowParent.GetChild(_rowParent.childCount - 1);
                oldest.SetParent(null);
                Destroy(oldest.gameObject);
            }

            var row = Instantiate(prefab, _rowParent);
            row.transform.SetAsFirstSibling();
            row.Setup(displayName, addValue, _visibleDuration);
            row.Show();

            _setRowCount++;

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rowParent);
            ApplySizeFactors(_setRowCount);

            if (endGroup)
            {
                _setRowCount = 0;
            }
        }

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// スコア行のプレハブを取得する.
        /// </summary>
        /// <param name="kind">スコア行の種別.</param>
        /// <returns>解決したプレハブ.</returns>
        private TrickScoreRowView GetRowPrefab(TrickScoreRowKind kind)
        {
            if (kind == TrickScoreRowKind.Bonus && _bonusRowPrefab != null)
            {
                return _bonusRowPrefab;
            }

            return _normalRowPrefab;
        }

        /// <summary>
        /// 行のレイアウトサイズの倍率を更新する.
        /// </summary>
        /// <param name="setRowCount">先頭から等倍表示する行数.</param>
        private void ApplySizeFactors(int setRowCount)
        {
            var compact = Mathf.Max(0.01f, _previousSetRowFactor);
            var n = _rowParent.childCount;
            var fullCount = Mathf.Clamp(setRowCount, 0, n);
            for (var i = 0; i < n; i++)
            {
                var rowView = _rowParent.GetChild(i).GetComponent<TrickScoreRowView>();
                if (rowView == null) { continue; }

                var factor = i < fullCount ? 1f : compact;
                rowView.ApplySizeFactor(factor);
            }
        }

        #endregion
    }
}
