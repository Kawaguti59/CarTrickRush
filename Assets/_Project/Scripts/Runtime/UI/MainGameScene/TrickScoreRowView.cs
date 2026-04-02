using System.Collections;

using UnityEngine;

using TMPro;

namespace CarTrickRush.UI
{
    /// =========================================================================================
    /// <summary>
    /// トリックスコアの行表示.
    /// </summary>
    /// =========================================================================================
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class TrickScoreRowView : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// サイズ調整対象のオブジェクト.
        /// </summary>
        [SerializeField] private GameObject[] _layoutSizeTargetObjects = default;

        /// <summary>
        /// ボーナス名のテキスト.
        /// </summary>
        [SerializeField] private TMP_Text _nameText = default;

        /// <summary>
        /// ボーナス値のテキスト.
        /// </summary>
        [SerializeField] private TMP_Text _valueText = default;

        /// <summary>
        /// CanvasGroup.
        /// </summary>
        [SerializeField] private CanvasGroup _canvasGroup = default;

        /// <summary>
        /// ライフサイクル用のコルーチン.
        /// </summary>
        private Coroutine _lifecycleRoutine = default;

        /// <summary>
        /// レイアウトサイズの対象RectTransform.
        /// </summary>
        private RectTransform[] _layoutSizeTargetRects = default;

        /// <summary>
        /// レイアウトサイズの基準値.
        /// </summary>
        private Vector2[] _baseSizeDeltas = default;

        /// <summary>
        /// レイアウトサイズの基準値が取得されたかどうか.
        /// </summary>
        private bool _baseSizesCaptured = default;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            RebuildTargets();
        }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// 対象 RectTransform の sizeDelta を, Awake 時点の基準に対して factor 倍する (1 = 最新行想定).
        /// </summary>
        public void ApplySizeFactor(float factor)
        {
            RebuildTargets();
            if (!_baseSizesCaptured)
            {
                SnapshotBaseSizes();
            }

            factor = Mathf.Max(0.01f, factor);
            if (_layoutSizeTargetRects == null) { return; }

            for (var i = 0; i < _layoutSizeTargetRects.Length; i++)
            {
                var rt = _layoutSizeTargetRects[i];
                if (rt == null) { continue; }

                if (_baseSizeDeltas == null || i >= _baseSizeDeltas.Length) { continue; }

                rt.sizeDelta = _baseSizeDeltas[i] * factor;
            }
        }

        /// <summary>
        /// 表示内容を設定し, 一定時間後にフェードアウトして破棄する.
        /// </summary>
        public void Show(string displayName, int addValue, float visibleDuration, float fadeDuration)
        {
            if (_lifecycleRoutine != null)
            {
                StopCoroutine(_lifecycleRoutine);
                _lifecycleRoutine = null;
            }

            _nameText.text = displayName;
            _valueText.text = $"+{Mathf.Max(0, addValue):N0}";

            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;

            _lifecycleRoutine = StartCoroutine(LifecycleRoutine(visibleDuration, fadeDuration));
        }

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// 対象のオブジェクトを取得する.
        /// </summary>
        private void RebuildTargets()
        {
            if (_layoutSizeTargetObjects == null || _layoutSizeTargetObjects.Length == 0)
            {
                _layoutSizeTargetRects = System.Array.Empty<RectTransform>();
                return;
            }

            _layoutSizeTargetRects = new RectTransform[_layoutSizeTargetObjects.Length];
            for (var i = 0; i < _layoutSizeTargetObjects.Length; i++)
            {
                var go = _layoutSizeTargetObjects[i];
                _layoutSizeTargetRects[i] = go != null ? go.GetComponent<RectTransform>() : null;
            }
        }

        /// <summary>
        /// 基準となるサイズを取得する.
        /// </summary>
        private void SnapshotBaseSizes()
        {
            if (_layoutSizeTargetRects == null || _layoutSizeTargetRects.Length == 0)
            {
                _baseSizeDeltas = System.Array.Empty<Vector2>();
                _baseSizesCaptured = true;
                return;
            }

            _baseSizeDeltas = new Vector2[_layoutSizeTargetRects.Length];
            for (var i = 0; i < _layoutSizeTargetRects.Length; i++)
            {
                var rt = _layoutSizeTargetRects[i];
                _baseSizeDeltas[i] = rt != null ? rt.sizeDelta : Vector2.zero;
            }

            _baseSizesCaptured = true;
        }

        /// <summary>
        /// ライフサイクル用のコルーチン.
        /// </summary>
        /// <param name="visibleDuration">表示時間.</param>
        /// <param name="fadeDuration">フェード時間.</param>
        /// <returns>コルーチン.</returns>
        private IEnumerator LifecycleRoutine(float visibleDuration, float fadeDuration)
        {
            if (visibleDuration > 0f)
            {
                yield return new WaitForSeconds(visibleDuration);
            }

            fadeDuration = Mathf.Max(0.01f, fadeDuration);
            var elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
                yield return null;
            }

            _canvasGroup.alpha = 0f;
            Destroy(gameObject);
        }

        #endregion
    }
}
