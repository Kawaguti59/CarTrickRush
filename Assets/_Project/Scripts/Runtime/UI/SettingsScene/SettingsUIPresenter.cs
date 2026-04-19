using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using CarTrickRush.Core;
using CarTrickRush.Definitions;
using CarTrickRush.Managers;
using CarTrickRush.UI.Common;

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
        /// 音量スライダーの対応.
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

        /// <summary>
        /// ボタン・スライダー操作が有効かどうか.
        /// </summary>
        private bool _canOperate = default;

        /// <summary>
        /// 現在のフォーカス対象.
        /// </summary>
        private Selectable _currentSelectable = default;

        /// <summary>
        /// 意図した UI から外れた検知時刻.
        /// </summary>
        private float _lostOwnedFocusSinceUnscaled = -1f;

        /// <summary>
        /// 所有している Selectable.
        /// </summary>
        private readonly List<Selectable> _ownedSelectables = new();

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            CollectOwnedSelectables();
            BindPointerSelects();
            SetInteractions(true);
        }

        private void Start()
        {
            _presenterView?.Show();
            StartCoroutine(InitializeSelectionCoroutine());
        }

        private void LateUpdate()
        {
            if (!_canOperate || _closing) { return; }
            if (_ownedSelectables.Count == 0) { return; }

            var eventSystem = EventSystem.current;
            if (eventSystem == null) { return; }

            var selectedGameObject = eventSystem.currentSelectedGameObject;
            if (TryResolveOwnedSelectable(selectedGameObject, out var owned))
            {
                _lostOwnedFocusSinceUnscaled = -1f;
                _currentSelectable = owned;
                return;
            }

            // 選択を失った場合は再選択.
            if (_currentSelectable != null
                && selectedGameObject != _currentSelectable.gameObject)
            {
                if (UISelectionDelay.ShouldRestoreNow(ref _lostOwnedFocusSinceUnscaled, hasValidFocus: false))
                {
                    eventSystem.SetSelectedGameObject(_currentSelectable.gameObject);
                }
            }
            else
            {
                _lostOwnedFocusSinceUnscaled = -1f;
            }
        }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// 設定画面を閉じる.
        /// </summary>
        public void OnClickCloseSettings()
        {
            if (_closing) { return; }

            UIButtonClickSound.Play();
            _closing = true;
            SetInteractions(false);
            StartCoroutine(CloseRoutine());
        }

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// 設定画面を閉じる.
        /// </summary>
        /// <returns>コルーチン.</returns>
        private IEnumerator CloseRoutine()
        {
            if (_presenterView != null)
            {
                _presenterView.Hide();
                yield return null;
                while (_presenterView.IsPlaying())
                {
                    yield return null;
                }
            }

            SceneLoadManager.UnloadScene("SettingsScene");
        }

        /// <summary>
        /// 初回フレーム後に EventSystem の選択を先頭に合わせる.
        /// </summary>
        private IEnumerator InitializeSelectionCoroutine()
        {
            yield return null;

            if (!_canOperate || _closing)
            {
                yield break;
            }

            var eventSystem = EventSystem.current;
            if (eventSystem == null || _ownedSelectables.Count == 0)
            {
                yield break;
            }

            var initial = _ownedSelectables[0];
            if (initial == null || !initial.interactable)
            {
                yield break;
            }

            eventSystem.SetSelectedGameObject(initial.gameObject);
            _currentSelectable = initial;
        }

        /// <summary>
        /// ボタン・スライダー操作が有効かどうかを設定する.
        /// </summary>
        /// <param name="enabled">有効かどうか.</param>
        private void SetInteractions(bool enabled)
        {
            _canOperate = enabled;

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

        /// <summary>
        /// Selectable の interactable を設定する.
        /// </summary>
        private static void SetInteractable(Selectable selectable, bool enabled)
        {
            if (selectable != null)
            {
                selectable.interactable = enabled;
            }
        }

        /// <summary>
        /// 所有している Selectable を取得する.
        /// </summary>
        private void CollectOwnedSelectables()
        {
            _ownedSelectables.Clear();

            foreach (var pair in _volumeSliders.Pairs)
            {
                if (!IsValidVolumePair(pair))
                {
                    continue;
                }

                var selectable = pair.Value.AsSelectable;
                if (selectable != null)
                {
                    _ownedSelectables.Add(selectable);
                }
            }

            if (_closeButton != null)
            {
                _ownedSelectables.Add(_closeButton);
            }
        }

        /// <summary>
        /// 有効な音量ペアかどうかを判定する.
        /// </summary>
        /// <param name="pair">対象の音量ペア.</param>
        /// <returns>有効な音量ペアかどうか.</returns>
        private static bool IsValidVolumePair(InspectableMap<AudioVolumeKind, VolumeSlider>.InspectablePair pair)
        {
            return pair.Key != AudioVolumeKind.None
                && pair.Value != null
                && pair.Value.Slider != null;
        }

        /// <summary>
        /// ポインタ選択を設定する.
        /// </summary>
        private void BindPointerSelects()
        {
            foreach (var selectable in _ownedSelectables)
            {
                BindPointerSelect(selectable);
            }
        }

        /// <summary>
        /// ホバー時に選択を切り替える.
        /// </summary>
        private void BindPointerSelect(Selectable selectable)
        {
            if (selectable == null) { return; }

            var eventTrigger = selectable.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = selectable.gameObject.AddComponent<EventTrigger>();
            }

            var pointerEnterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            pointerEnterEntry.callback.AddListener(_ => SelectSelectable(selectable));
            eventTrigger.triggers.Add(pointerEnterEntry);

            var pointerDownEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            pointerDownEntry.callback.AddListener(_ => SelectSelectable(selectable));
            eventTrigger.triggers.Add(pointerDownEntry);
        }

        /// <summary>
        /// 選択を設定する.
        /// </summary>
        /// <param name="selectable">対象の Selectable.</param>
        private void SelectSelectable(Selectable selectable)
        {
            if (!_canOperate || _closing || selectable == null) { return; }
            EventSystem.current?.SetSelectedGameObject(selectable.gameObject);
            _currentSelectable = selectable;
        }

        /// <summary>
        /// 所有している Selectable を取得する.
        /// </summary>
        /// <param name="selectedGameObject">選択された GameObject.</param>
        /// <param name="selectable">対象の Selectable.</param>
        /// <returns>所有している Selectable を取得できたかどうか.</returns>
        private bool TryResolveOwnedSelectable(GameObject selectedGameObject, out Selectable selectable)
        {
            selectable = null;
            if (selectedGameObject == null)
            {
                return false;
            }

            foreach (var owned in _ownedSelectables)
            {
                if (owned == null)
                {
                    continue;
                }

                if (selectedGameObject == owned.gameObject)
                {
                    selectable = owned;
                    return true;
                }

                var rootSelectable = selectedGameObject.GetComponentInParent<Selectable>();
                if (rootSelectable == owned)
                {
                    selectable = owned;
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
