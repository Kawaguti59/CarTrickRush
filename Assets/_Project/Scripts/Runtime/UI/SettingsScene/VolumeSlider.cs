using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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

        /// <summary>
        /// 連続調整の単位.
        /// </summary>
        [SerializeField] private float _holdAdjustUnitsPerSecond = 100f;

        /// <summary>
        /// 左スティックの無効ゾーン（これ未満は無視）.
        /// </summary>
        [SerializeField] [Range(0f, 1f)] private float _gamepadStickDeadzone = 0.35f;

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

        private void Update()
        {
            ApplyHoldAdjustIfSelected();
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

            var stepped = Mathf.Round(value);
            audio.SetVolume(_volumeKind, Mathf.Clamp01(stepped / 100f));
        }

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// フォーカス中に矢印／A・D／パッドで押しっぱなし連続変化.
        /// </summary>
        private void ApplyHoldAdjustIfSelected()
        {
            if (_slider == null || !_slider.interactable || !_slider.IsActive()) { return; }

            if (!IsSliderSelectionFocused()) { return; }

            var axis = ReadHorizontalAdjustInput();
            if (Mathf.Approximately(axis, 0f)) { return; }

            var sign = SliderHorizontalDirectionSign(_slider);
            var delta = axis * sign * Mathf.Max(0f, _holdAdjustUnitsPerSecond) * Time.unscaledDeltaTime;
            var next = Mathf.Clamp(_slider.value + delta, _slider.minValue, _slider.maxValue);
            _slider.value = next;
        }

        private bool IsSliderSelectionFocused()
        {
            var eventSystem = EventSystem.current;
            if (eventSystem == null)
            {
                return false;
            }

            var current = eventSystem.currentSelectedGameObject;
            if (current == null)
            {
                return false;
            }

            if (current == _slider.gameObject)
            {
                return true;
            }

            var root = current.GetComponentInParent<Slider>();
            return root == _slider;
        }

        private static float SliderHorizontalDirectionSign(Slider slider)
        {
            var reverse = slider.direction == Slider.Direction.RightToLeft
                || slider.direction == Slider.Direction.TopToBottom;
            return reverse ? -1f : 1f;
        }

        private float ReadHorizontalAdjustInput()
        {
            var pad = Gamepad.current;
            if (pad != null)
            {
                if (pad.dpad.left.isPressed && !pad.dpad.right.isPressed)
                {
                    return -1f;
                }

                if (pad.dpad.right.isPressed && !pad.dpad.left.isPressed)
                {
                    return 1f;
                }
            }

            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                var left = keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed;
                var right = keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed;
                if (left && !right)
                {
                    return -1f;
                }

                if (right && !left)
                {
                    return 1f;
                }
            }

            if (pad != null)
            {
                var x = pad.leftStick.x.ReadValue();
                var dead = Mathf.Clamp01(_gamepadStickDeadzone);
                if (Mathf.Abs(x) > dead)
                {
                    return Mathf.Clamp(x, -1f, 1f);
                }
            }

            return 0f;
        }

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

            var current = eventSystem.currentSelectedGameObject;
            var selected = false;
            if (current != null)
            {
                if (current == _slider.gameObject)
                {
                    selected = true;
                }
                else
                {
                    var rootSelectable = current.GetComponentInParent<Selectable>();
                    selected = rootSelectable == _slider;
                }
            }

            if (_highlightImage.enabled == selected) { return; }

            _highlightImage.enabled = selected;
        }

        #endregion
    }
}
