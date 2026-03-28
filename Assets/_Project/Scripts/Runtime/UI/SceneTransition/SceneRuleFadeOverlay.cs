using UnityEngine;
using UnityEngine.UI;

using System.Collections;

namespace CarTrickRush.UI.SceneTransition
{
    /// =========================================================================================
    /// <summary>
    /// ルールフェード用オーバーレイ.
    /// </summary>
    /// =========================================================================================
    public sealed class SceneRuleFadeOverlay : MonoBehaviour
    {
        #region ------------------ Constants ------------------

        /// <summary>
        /// シェーダー _Progress の Property ID.
        /// </summary>
        private static readonly int ProgressId = Shader.PropertyToID("_Progress");

        /// <summary>
        /// シェーダー _Softness の Property ID.
        /// </summary>
        private static readonly int SoftnessId = Shader.PropertyToID("_Softness");

        /// <summary>
        /// シェーダー _Color の Property ID.
        /// </summary>
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        /// <summary>
        /// シェーダー _UseSolid の Property ID.
        /// </summary>
        private static readonly int UseSolidId = Shader.PropertyToID("_UseSolid");

        /// <summary>
        /// シェーダー名.
        /// </summary>
        private const string ShaderName = "CarTrickRush/UI/RuleFade";

        #endregion

        #region ------------------ Fields ------------------

        /// <summary>
        /// マスク表示用 RawImage.
        /// </summary>
        private RawImage _rawImage;

        /// <summary>
        /// マテリアルインスタンス.
        /// </summary>
        private Material _materialInstance;

        /// <summary>
        /// オーバーレイ用 Canvas.
        /// </summary>
        private Canvas _canvas;

        /// <summary>
        /// CanvasGroup.
        /// </summary>
        private CanvasGroup _canvasGroup;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// マテリアルが有効か.
        /// </summary>
        public bool IsReady => _materialInstance != null;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void OnDestroy()
        {
            if (_materialInstance != null)
            {
                Destroy(_materialInstance);
                _materialInstance = null;
            }
        }

        #endregion
        
        #region ------------------ Public Methods ------------------

        /// <summary>
        /// 子階層に Canvas / RawImage を生成して初期化する.
        /// </summary>
        /// <param name="overlayColor">オーバーレイの色（通常は黒系）.</param>
        public void EnsureBuilt(Color overlayColor)
        {
            if (_rawImage != null && _materialInstance != null)
            { 
                _materialInstance.SetColor(ColorId, overlayColor);
                return;
            }

            _canvas = gameObject.GetComponent<Canvas>();
            if (_canvas == null)
            {
                _canvas = gameObject.AddComponent<Canvas>();
            }

            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 32760;
            _canvas.overrideSorting = true;

            if (gameObject.GetComponent<CanvasScaler>() == null)
            {
                var scaler = gameObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920f, 1080f);
                scaler.matchWidthOrHeight = 0.5f;
            }

            if (gameObject.GetComponent<GraphicRaycaster>() == null)
            {
                gameObject.AddComponent<GraphicRaycaster>();
            }

            _canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = false;
            _canvasGroup.alpha = 1f;

            var imageGo = new GameObject("RuleFadeRawImage", typeof(RectTransform));
            imageGo.transform.SetParent(transform, false);
            _rawImage = imageGo.AddComponent<RawImage>();
            _rawImage.raycastTarget = true;

            var rt = _rawImage.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var shader = Shader.Find(ShaderName);
            if (shader == null)
            { 
                Debug.LogError($"{nameof(SceneRuleFadeOverlay)}: Shader '{ShaderName}' が見つかりません.");
                return;
            }

            _materialInstance = new Material(shader);
            _rawImage.texture = Texture2D.whiteTexture;
            _rawImage.material = _materialInstance;
            _materialInstance.SetColor(ColorId, overlayColor);
            _materialInstance.SetFloat(SoftnessId, 0.04f);
            _materialInstance.SetFloat(ProgressId, 0f);
            _materialInstance.SetFloat(UseSolidId, 0f);

            gameObject.SetActive(false);
        }

        /// <summary>
        /// マスクテクスチャとソリッド／ルールモードを設定する.
        /// </summary>
        /// <param name="mask">null のときソリッドフェード.</param>
        /// <param name="softness">ルール境界のぼかし.</param>
        public void Configure(Texture2D mask, float softness)
        {
            if (_materialInstance == null) { return; }

            var useSolid = mask == null ? 1f : 0f;
            _materialInstance.SetFloat(UseSolidId, useSolid);
            _materialInstance.SetFloat(SoftnessId, softness);

            if (mask != null)
            {
                _rawImage.texture = mask;
            }
            else
            {
                _rawImage.texture = Texture2D.whiteTexture;
            }
        }

        /// <summary>
        /// 進捗を即時設定する.
        /// </summary>
        /// <param name="progress">0〜1.</param>
        public void SetProgress(float progress)
        {
            if (_materialInstance == null) { return; }

            _materialInstance.SetFloat(ProgressId, Mathf.Clamp01(progress));
        }

        /// <summary>
        /// オーバーレイを表示する.
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
                _canvasGroup.blocksRaycasts = true;
            }
        }

        /// <summary>
        /// オーバーレイを非表示にする.
        /// </summary>
        public void Hide()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.blocksRaycasts = false;
            }

            gameObject.SetActive(false);
        }

        /// <summary>
        /// 進捗を線形にアニメする.
        /// </summary>
        /// <param name="from">開始値.</param>
        /// <param name="to">終了値.</param>
        /// <param name="duration">秒（unscaled）.</param>
        /// <returns>コルーチン.</returns>
        public IEnumerator AnimateProgress(float from, float to, float duration)
        {
            if (_materialInstance == null) { yield break; }

            duration = Mathf.Max(0.0001f, duration);
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var p = Mathf.Lerp(from, to, t);
                _materialInstance.SetFloat(ProgressId, p);
                yield return null;
            }

            _materialInstance.SetFloat(ProgressId, Mathf.Clamp01(to));
        }

        #endregion
    }
}
