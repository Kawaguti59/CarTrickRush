using System.Collections;

using UnityEngine;

using TMPro;

using CarTrickRush.Core.View;

namespace CarTrickRush.UI
{
    /// =========================================================================================
    /// <summary>
    /// トリックスコアの行表示.
    /// </summary>
    /// =========================================================================================
    public sealed class TrickScoreRowView : MonoBehaviour, IView
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
        /// アニメーター.
        /// </summary>
        [SerializeField] private Animator _animator = default;

        /// <summary>
        /// ライフサイクル用のコルーチン.
        /// </summary>
        private Coroutine _lifecycleRoutine = default;

        /// <summary>
        /// 表示が続く時間.
        /// </summary>
        private float _visibleDuration = default;

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
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }

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
        /// 表示内容と表示時間を設定する.
        /// </summary>
        /// <param name="displayName">表示名.</param>
        /// <param name="addValue">加算値.</param>
        /// <param name="visibleDuration">表示時間.</param>
        public void Setup(string displayName, int addValue, float visibleDuration)
        {
            _nameText.text = displayName;
            _valueText.text = $"+{Mathf.Max(0, addValue):N0}";
            _visibleDuration = visibleDuration;
        }

        #endregion

        #region ------------------ Interface Methods ------------------

        public void Initialize()
        {
        }

        public void Show()
        {
            if (_lifecycleRoutine != null)
            {
                StopCoroutine(_lifecycleRoutine);
                _lifecycleRoutine = null;
            }

            if (_animator != null)
            {
                _animator.Play("Show", layer: 0, normalizedTime: 0f);
            }

            _lifecycleRoutine = StartCoroutine(LifecycleRoutine());
        }

        public void Hide()
        {
            if (_animator == null) { return; }
            _animator.Play("Hide", layer: 0, normalizedTime: 0f);
        }

        public bool IsPlaying()
        {
            if (_animator == null) { return false; }
            return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
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
        /// <returns>コルーチン.</returns>
        private IEnumerator LifecycleRoutine()
        {
            if (_visibleDuration > 0f)
            {
                yield return new WaitForSeconds(_visibleDuration);
            }

            if (_animator != null)
            {
                Hide();
                yield return null;
                while (IsPlaying())
                {
                    yield return null;
                }
            }

            Destroy(gameObject);
        }

        #endregion
    }
}
