using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using CarTrickRush.Core;
using CarTrickRush.Definitions;
using CarTrickRush.Managers;

namespace CarTrickRush.UI.Settings
{
    /// =========================================================================================
    /// <summary>
    /// 音量調整用スライダー管理クラス.
    /// </summary>
    /// =========================================================================================
    public sealed class VolumeSlider : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// 操作・フォーカス対象の Slider.
        /// </summary>
        [SerializeField] private Slider _slider = default;

        /// <summary>
        /// 音量チャンネル.
        /// </summary>
        [SerializeField] private AudioVolumeKind _volumeKind = AudioVolumeKind.Master;

        /// <summary>
        /// 選択中だけ見せたい装飾用 Image（Slider の親や行ルートを指定しないこと。非表示は <see cref="Image.enabled"/> のみ切り替える）.
        /// </summary>
        [SerializeField] private Image _highlightImage = default;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// 操作・フォーカス対象の Slider.
        /// </summary>
        public Slider Slider => _slider;

        /// <summary>
        /// UI ナビゲーション用 Selectable.
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
            SyncFromAudioManager();
            RefreshHighlightImage();
        }

        private void LateUpdate()
        {
            if (_slider == null) { return; }
            RefreshHighlightImage();
        }

        private void OnDestroy()
        {
            _slider?.onValueChanged.RemoveListener(OnSliderValueChanged);
        }

        #endregion

        #region ------------------ Public Methods ------------------
        
        /// <summary>
        /// Slider の値が変更された時の処理.
        /// </summary>
        /// <param name="value">Slider の値.</param>
        public void OnSliderValueChanged(float value)
        {
            if (_slider == null) { return; }

            var audio = ManagerLocator.AudioManager;
            if (audio == null) { return; }

            audio.SetVolume(_volumeKind, Mathf.Clamp01(value / 100f));
        }

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// AudioManager の音量を Slider に同期する.
        /// </summary>
        private void SyncFromAudioManager()
        {
            if (_slider == null) { return; }

            var audio = ManagerLocator.AudioManager;
            if (audio == null) { return; }

            var normalized = audio.GetVolume(_volumeKind);
            _slider.SetValueWithoutNotify(Mathf.Round(normalized * 100f));
        }

        /// <summary>
        /// Slider の選択状態に合わせてハイライト用 Image の表示を切り替える.
        /// </summary>
        private void RefreshHighlightImage()
        {
            if (_highlightImage == null || _slider == null) { return; }

            var eventSystem = EventSystem.current;
            if (eventSystem == null) { return; }

            var selected = SettingsUiSelection.Matches(_slider, eventSystem.currentSelectedGameObject);
            if (_highlightImage.enabled == selected) { return; }

            _highlightImage.enabled = selected;
        }

        #endregion
    }

    /// <summary>
    /// EventSystem の選択が指定 <see cref="Selectable"/>（Slider の子ハンドル等を含む）に属するか.
    /// </summary>
    internal static class SettingsUiSelection
    {
        internal static bool Matches(Selectable selectable, GameObject currentSelected)
        {
            if (selectable == null || currentSelected == null)
            {
                return false;
            }

            if (currentSelected == selectable.gameObject)
            {
                return true;
            }

            var rootSelectable = currentSelected.GetComponentInParent<Selectable>();
            return rootSelectable == selectable;
        }
    }
}
