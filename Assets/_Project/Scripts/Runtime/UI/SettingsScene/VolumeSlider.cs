using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using CarTrickRush.Core;
using CarTrickRush.Definitions;

namespace CarTrickRush.UI.Settings
{
    /// =========================================================================================
    /// <summary>
    /// 音量調整用スライダー.
    /// </summary>
    /// =========================================================================================
    public sealed class VolumeSlider : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// スライダー.
        /// </summary>
        [SerializeField] private Slider _slider = default;

        /// <summary>
        /// 音量チャンネル.
        /// </summary>
        [SerializeField] private AudioVolumeKind _volumeKind = AudioVolumeKind.Master;

        /// <summary>
        /// スライダーが選択中のときだけ表示するハイライト用 Image（子オブジェクトなどに付与）.
        /// </summary>
        [SerializeField] private Image _highlightImage = default;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// UI ナビゲーション用.
        /// </summary>
        public Selectable AsSelectable => _slider;

        /// <summary>
        /// このスライダーが操作する音量チャンネル.
        /// </summary>
        public AudioVolumeKind VolumeKind => _volumeKind;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            if (_slider == null)
            {
                _slider = GetComponentInChildren<Slider>(includeInactive: true);
            }

            if (_slider == null) { return; }

            _slider.minValue = 0f;
            _slider.maxValue = 100f;
            _slider.wholeNumbers = true;
            SyncFromAudioManager();
            _slider.onValueChanged.AddListener(OnSliderValueChanged);
            RefreshSelectionHighlight(isInitial: true);
        }

        private void LateUpdate()
        {
            RefreshSelectionHighlight(isInitial: false);
        }

        private void OnDestroy()
        {
            if (_slider != null)
            {
                _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
            }
        }

        #endregion

        #region ------------------ Interface Methods ------------------

        public void OnPointerEnter(PointerEventData eventData)
        {
            SelectSelf();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            SelectSelf();
        }

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// スライダーを選択する.
        /// </summary>
        private void SelectSelf()
        {
            if (_slider == null) { return; }

            var eventSystem = EventSystem.current;
            if (eventSystem == null) { return; }

            eventSystem.SetSelectedGameObject(_slider.gameObject);
        }

        /// <summary>
        /// 音量マネージャーからスライダーの値を同期する.
        /// </summary>
        private void SyncFromAudioManager()
        {
            var audio = ManagerLocator.AudioManager;
            if (audio == null || _slider == null) { return; }

            var normalized = audio.GetVolume(_volumeKind);
            _slider.SetValueWithoutNotify(Mathf.Round(normalized * 100f));
        }

        /// <summary>
        /// スライダーの値が変更されたときの処理.
        /// </summary>
        /// <param name="value">スライダーの値.</param>
        private void OnSliderValueChanged(float value)
        {
            var audio = ManagerLocator.AudioManager;
            if (audio == null) { return; }

            audio.SetVolume(_volumeKind, Mathf.Clamp01(value / 100f));
        }

        /// <summary>
        /// スライダーが選択中のときだけ表示するハイライト用 Imageを更新する.
        /// </summary>
        /// <param name="isInitial">初期化かどうか.</param> 
        private void RefreshSelectionHighlight(bool isInitial)
        {
            var eventSystem = EventSystem.current;
            if (eventSystem == null) { return; }
            var selected = eventSystem != null && eventSystem.currentSelectedGameObject == _slider.gameObject;
            if (!isInitial) { return; }

            _highlightImage?.gameObject.SetActive(selected);
        }

        #endregion
    }
}
