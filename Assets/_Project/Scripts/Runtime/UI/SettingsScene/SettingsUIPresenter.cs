using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using CarTrickRush.Core;
using CarTrickRush.Definitions;
using CarTrickRush.Managers;

namespace CarTrickRush.UI.Settings
{
    /// =========================================================================================
    /// <summary>
    /// 設定画面のUIを制御するクラス.
    /// </summary>
    /// =========================================================================================
    public sealed class SettingsUIPresenter : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// 音量チャンネルと <see cref="VolumeSlider"/> の対応（リスト順が上下ナビの順）.
        /// </summary>
        [SerializeField] private InspectableMap<AudioVolumeKind, VolumeSlider> _volumeSliders = new();

        /// <summary>
        /// 閉じるボタン.
        /// </summary>
        [SerializeField] private Button _closeButton = default;

        /// <summary>
        /// プレゼンターView.
        /// </summary>
        [SerializeField] private SettingsUIPresenterView _presenterView = default;

        /// <summary>
        /// 閉じているかどうか.
        /// </summary>
        private bool _closing = default;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            SetInteractions(true);
            ApplySelectableNavigationChain();
        }

        private void Start()
        {
            _presenterView?.PlayIntro();
        }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// 設定画面を閉じる.
        /// </summary>
        public void OnClickCloseSettings()
        {
            if (_closing)
            {
                return;
            }

            ManagerLocator.AudioManager?.PlaySe("ButtonClick");
            _closing = true;
            SetInteractions(false);
            StartCoroutine(CloseRoutine());
        }

        #endregion

        #region ------------------ Private Methods ------------------

        private IEnumerator CloseRoutine()
        {
            if (_presenterView != null)
            {
                yield return _presenterView.PlayExitRoutine();
            }

            SceneLoadManager.UnloadScene("SettingsScene");
        }

        private void SetInteractions(bool enabled)
        {
            foreach (var pair in _volumeSliders.Pairs)
            {
                if (!IsValidVolumePair(pair))
                {
                    continue;
                }

                SetInteractable(pair.Value.AsSelectable, enabled);
            }

            if (_closeButton != null)
            {
                _closeButton.interactable = enabled;
            }
        }

        private static void SetInteractable(Selectable selectable, bool enabled)
        {
            if (selectable != null)
            {
                selectable.interactable = enabled;
            }
        }

        private void ApplySelectableNavigationChain()
        {
            var chain = new List<Selectable>();
            foreach (var pair in _volumeSliders.Pairs)
            {
                if (!IsValidVolumePair(pair))
                {
                    continue;
                }

                var selectable = pair.Value.AsSelectable;
                if (selectable != null)
                {
                    chain.Add(selectable);
                }
            }

            var close = _closeButton;
            if (chain.Count > 0 && close != null)
            {
                chain.Add(close);
                ChainVertical(chain.ToArray());
                return;
            }

            if (close != null)
            {
                var navigation = new Navigation { mode = Navigation.Mode.Explicit };
                navigation.selectOnUp = close;
                navigation.selectOnDown = close;
                navigation.selectOnLeft = close;
                navigation.selectOnRight = close;
                close.navigation = navigation;
            }
        }

        private static void ChainVertical(params Selectable[] items)
        {
            for (var i = 0; i < items.Length; i++)
            {
                var navigation = new Navigation { mode = Navigation.Mode.Explicit };
                navigation.selectOnUp = i > 0 ? items[i - 1] : items[0];
                navigation.selectOnDown = i < items.Length - 1 ? items[i + 1] : items[^1];
                navigation.selectOnLeft = items[i];
                navigation.selectOnRight = items[i];
                items[i].navigation = navigation;
            }
        }

        private static bool IsValidVolumePair(InspectableMap<AudioVolumeKind, VolumeSlider>.InspectablePair pair)
        {
            return pair.Key != AudioVolumeKind.None
                && pair.Value != null
                && pair.Value.Slider != null;
        }

        #endregion
    }
}
