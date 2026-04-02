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

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// 行を追加する (先頭が最新, 下に古い行が並ぶ).
        /// </summary>
        public void Push(string displayName, int addValue, TrickScoreRowKind kind)
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

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rowParent);
            RefreshRowLayoutSizeFactors();
        }

        #endregion

        #region ------------------ Editor / Debug ------------------

#if UNITY_EDITOR
        [ContextMenu("Debug/Push Sample Normal")]
        private void DebugPushSampleNormal()
        {
            Push("Sample", 100, TrickScoreRowKind.Normal);
        }

        [ContextMenu("Debug/Push Sample Bonus")]
        private void DebugPushSampleBonus()
        {
            Push("Bonus", 500, TrickScoreRowKind.Bonus);
        }
#endif

        #endregion

        #region ------------------ Private Methods ------------------

        private TrickScoreRowView ResolvePrefab(TrickScoreRowKind kind)
        {
            if (kind == TrickScoreRowKind.Bonus && _bonusRowPrefab != null)
            {
                return _bonusRowPrefab;
            }

            return _normalRowPrefab;
        }

        private void RefreshRowLayoutSizeFactors()
        {
            var compact = Mathf.Max(0.01f, _previousRowLayoutFactor);
            var n = _rowParent.childCount;
            for (var i = 0; i < n; i++)
            {
                var rowView = _rowParent.GetChild(i).GetComponent<TrickScoreRowView>();
                if (rowView == null) { continue; }

                var factor = i == 0 ? 1f : compact;
                rowView.SetRowLayoutSizeFactor(factor);
            }
        }

        #endregion
    }
}
