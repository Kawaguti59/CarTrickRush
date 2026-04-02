using System.Collections;

using UnityEngine;

using TMPro;

namespace CarTrickRush.UI
{
    /// =========================================================================================
    /// <summary>
    /// ボーナススコアの行表示.
    /// </summary>
    /// =========================================================================================
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class BonusScoreRowView : MonoBehaviour
    {
        #region ------------------ Fields ------------------

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

        #endregion

        #region ------------------ Public Methods ------------------

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
