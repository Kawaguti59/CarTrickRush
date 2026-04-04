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

        private void SelectSelf()
        {
            if (_slider == null) { return; }

            var eventSystem = EventSystem.current;
            if (eventSystem == null) { return; }

            eventSystem.SetSelectedGameObject(_slider.gameObject);
        }

        private void SyncFromAudioManager()
        {
            var audio = ManagerLocator.AudioManager;
            if (audio == null || _slider == null) { return; }

            var normalized = audio.GetVolume(_volumeKind);
            _slider.SetValueWithoutNotify(Mathf.Round(normalized * 100f));
        }

        private void OnSliderValueChanged(float value)
        {
            var audio = ManagerLocator.AudioManager;
            if (audio == null) { return; }

            audio.SetVolume(_volumeKind, Mathf.Clamp01(value / 100f));
        }

        #endregion
    }
}
