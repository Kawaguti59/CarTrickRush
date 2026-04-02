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
        /// フェード時間.
        /// </summary>
        [SerializeField] private float _fadeDuration = 0.35f;

        /// <summary>
        /// 前の行に適用する幅・高さの倍率.
        /// </summary>
        [SerializeField] private float _previousRowLayoutFactor = 0.8f;

        /// <summary>
        /// 現在のセットで追加した行数.
        /// </summary>
        private int _batchRowCount = default;

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// 行を追加する (先頭が最新, 下に古い行が並ぶ).
        /// </summary>
        /// <param name="endGroup">
        /// false のときは同一セットの続き (例: 回転スコアの次にボーナス行). 最後の1回だけ true にすると,
        /// そのセットで追加した全行が等倍, それより前の行だけが縮小倍率になる.
        /// </param>
        public void Push(string displayName, int addValue, TrickScoreRowKind kind, bool endGroup = true)
        {
            var prefab = ResolvePrefab(kind);
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
            row.Show(displayName, addValue, _visibleDuration, _fadeDuration);

            _batchRowCount++;

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rowParent);
            RefreshRowLayoutSizeFactors(_batchRowCount);

            if (endGroup)
            {
                _batchRowCount = 0;
            }
        }

        #endregion

        #region ------------------ Editor / Debug ------------------

#if UNITY_EDITOR
        [ContextMenu("Debug/Push Sample Normal")]
        private void DebugPushSampleNormal()
        {
            Push("Sample", 100, TrickScoreRowKind.Normal, endGroup: true);
        }

        [ContextMenu("Debug/Push Sample Bonus")]
        private void DebugPushSampleBonus()
        {
            Push("Bonus", 500, TrickScoreRowKind.Bonus, endGroup: true);
        }

        [ContextMenu("Debug/Push Sample Set (2 rows)")]
        private void DebugPushSampleSet()
        {
            Push("Row A", 10, TrickScoreRowKind.Normal, endGroup: false);
            Push("Row B", 90, TrickScoreRowKind.Bonus, endGroup: true);
        }
#endif

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// プレハブを解決する.
        /// </summary>
        /// <param name="kind">スコア行の種別.</param>
        /// <returns>解決したプレハブ.</returns>
        private TrickScoreRowView ResolvePrefab(TrickScoreRowKind kind)
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
        /// <param name="leadingFullScaleCount">先頭から等倍表示する行数.</param>
        private void RefreshRowLayoutSizeFactors(int leadingFullScaleCount)
        {
            var compact = Mathf.Max(0.01f, _previousRowLayoutFactor);
            var n = _rowParent.childCount;
            var fullCount = Mathf.Clamp(leadingFullScaleCount, 0, n);
            for (var i = 0; i < n; i++)
            {
                var rowView = _rowParent.GetChild(i).GetComponent<TrickScoreRowView>();
                if (rowView == null) { continue; }

                var factor = i < fullCount ? 1f : compact;
                rowView.SetRowLayoutSizeFactor(factor);
            }
        }

        #endregion
    }
}
