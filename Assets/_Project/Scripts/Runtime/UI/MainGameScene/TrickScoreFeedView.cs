using UnityEngine;

namespace CarTrickRush.UI
{
    /// =========================================================================================
    /// <summary>
    /// トリックスコアのキュー表示.
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
        /// ボーナス行のプレハブ.
        /// </summary>
        [SerializeField] private TrickScoreRowView _rowPrefab = default;

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

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// ボーナス行を追加する (末尾が最新).
        /// </summary>
        public void Push(string bonusDisplayName, int addValue)
        {
            if (_rowParent == null || _rowPrefab == null)
            {
                return;
            }

            var limit = Mathf.Max(1, _maxRows);
            while (_rowParent.childCount >= limit)
            {
                var oldest = _rowParent.GetChild(0);
                Destroy(oldest.gameObject);
            }

            var row = Instantiate(_rowPrefab, _rowParent);
            row.transform.SetAsLastSibling();
            row.Show(bonusDisplayName, addValue, _visibleDuration, _fadeDuration);
        }

        #endregion

        #region ------------------ Editor / Debug ------------------

#if UNITY_EDITOR
        [ContextMenu("Debug/Push Sample Row")]
        private void DebugPushSampleRow()
        {
            Push("Back Flip", 100);
        }
#endif

        #endregion
    }
}
