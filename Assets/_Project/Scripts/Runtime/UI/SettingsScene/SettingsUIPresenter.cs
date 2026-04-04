using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using System.Collections;

using CarTrickRush.Core;
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
        /// マスター音量スライダー.
        /// </summary>
        [SerializeField] private VolumeSlider _masterVolumeSlider = default;

        /// <summary>
        /// BGM音量スライダー.
        /// </summary>
        [SerializeField] private VolumeSlider _bgmVolumeSlider = default;

        /// <summary>
        /// SE音量スライダー.
        /// </summary>
        [SerializeField] private VolumeSlider _seVolumeSlider = default;

        /// <summary>
        /// 閉じるボタン.
        /// </summary>
        [SerializeField] private Button _closeButton = default;

        /// <summary>
        /// プレゼンターView.
        /// </summary>  
        [SerializeField] private SettingsUIPresenterView _presenterView = default;

        /// <summary>
        /// インタラクションが有効かどうか.
        /// </summary>
        private bool _interactionsEnabled = default;

        /// <summary>
        /// 閉じているかどうか.
        /// </summary>
        private bool _closing = default;

        /// <summary>
        /// 現在フォーカス中の選択 UI.
        /// </summary>
        private Selectable _currentSelectable = default;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// 現在フォーカス中の選択 UI.
        /// </summary>
        public Selectable CurrentSelectable => _currentSelectable;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            SetInteractions(true);
            BindPointerSelect(_masterVolumeSlider?.AsSelectable);
            BindPointerSelect(_bgmVolumeSlider?.AsSelectable);
            BindPointerSelect(_seVolumeSlider?.AsSelectable);
            BindPointerSelect(_closeButton);
            ApplySelectableNavigationChain();
        }

        private void Start()
        {
            var eventSystem = EventSystem.current;
            _currentSelectable = PickFirstSelectable();
            if (eventSystem != null && _currentSelectable != null)
            {
                eventSystem.SetSelectedGameObject(_currentSelectable.gameObject);
            }

            _presenterView?.PlayIntro();
        }

        private void LateUpdate()
        {
            if (!_interactionsEnabled || _closing) { return; }

            var eventSystem = EventSystem.current;
            if (eventSystem == null) { return; }

            var selectedGameObject = eventSystem.currentSelectedGameObject;
            if (TryMatchSelectable(selectedGameObject, _masterVolumeSlider?.AsSelectable)) { return; }
            if (TryMatchSelectable(selectedGameObject, _bgmVolumeSlider?.AsSelectable)) { return; }
            if (TryMatchSelectable(selectedGameObject, _seVolumeSlider?.AsSelectable)) { return; }

            if (_closeButton != null && selectedGameObject == _closeButton.gameObject)
            {
                _currentSelectable = _closeButton;
                return;
            }

            if (selectedGameObject == null && _currentSelectable != null)
            {
                eventSystem.SetSelectedGameObject(_currentSelectable.gameObject);
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

            ManagerLocator.AudioManager?.PlaySe("ButtonClick");
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
                yield return _presenterView.PlayExitRoutine();
            }

            SceneLoadManager.UnloadScene("SettingsScene");
        }

        /// <summary>
        /// インタラクションを有効/無効にする.
        /// </summary>
        /// <param name="enabled">有効かどうか.</param>
        private void SetInteractions(bool enabled)
        {
            _interactionsEnabled = enabled;

            SetInteractable(_masterVolumeSlider?.AsSelectable, enabled);
            SetInteractable(_bgmVolumeSlider?.AsSelectable, enabled);
            SetInteractable(_seVolumeSlider?.AsSelectable, enabled);

            if (_closeButton != null)
            {
                _closeButton.interactable = enabled;
            }
        }

        /// <summary>
        /// 選択可能なUIを有効/無効にする.
        /// </summary>
        /// <param name="selectable">選択可能なUI.</param>
        /// <param name="enabled">有効かどうか.</param>
        private static void SetInteractable(Selectable selectable, bool enabled)
        {
            if (selectable != null)
            {
                selectable.interactable = enabled;
            }
        }

        /// <summary>
        /// 最初の選択可能なUIを取得する.
        /// </summary>
        /// <returns>最初の選択可能なUI.</returns>
        private Selectable PickFirstSelectable()
        {
            if (_masterVolumeSlider != null)
            {
                return _masterVolumeSlider.AsSelectable;
            }

            if (_bgmVolumeSlider != null)
            {
                return _bgmVolumeSlider.AsSelectable;
            }

            if (_seVolumeSlider != null)
            {
                return _seVolumeSlider.AsSelectable;
            }

            return _closeButton;
        }

        private bool TryMatchSelectable(GameObject selected, Selectable candidate)
        {
            if (candidate == null || selected != candidate.gameObject)
            {
                return false;
            }

            _currentSelectable = candidate;
            return true;
        }

        /// <summary>
        /// 選択可能なUIのナビゲーションを設定する.
        /// </summary>
        private void ApplySelectableNavigationChain()
        {
            var master = _masterVolumeSlider?.AsSelectable;
            var bgm = _bgmVolumeSlider?.AsSelectable;
            var se = _seVolumeSlider?.AsSelectable;
            var close = _closeButton;

            if (master != null && bgm != null && se != null && close != null)
            {
                ChainVertical(master, bgm, se, close);
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

        /// <summary>
        /// 垂直方向のナビゲーションを設定する.
        /// </summary>
        /// <param name="items">選択可能なUI.</param>
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

        /// <summary>
        /// ホバー時に選択を切り替える.
        /// </summary>
        private void BindPointerSelect(Selectable selectable)
        {
            if (selectable == null) { return; }

            var eventTrigger = selectable.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null) { return; }

            var pointerEnterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            pointerEnterEntry.callback.AddListener(_ => SelectSelectable(selectable));
            eventTrigger.triggers.Add(pointerEnterEntry);

            var pointerDownEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            pointerDownEntry.callback.AddListener(_ => SelectSelectable(selectable));
            eventTrigger.triggers.Add(pointerDownEntry);
        }

        /// <summary>
        /// 選択可能なUIを選択する.
        /// </summary>
        /// <param name="selectable">選択可能なUI.</param>
        private void SelectSelectable(Selectable selectable)
        {
            if (!_interactionsEnabled || selectable == null || _closing) { return; }

            var eventSystem = EventSystem.current;
            if (eventSystem == null) { return; }

            eventSystem.SetSelectedGameObject(selectable.gameObject);
            _currentSelectable = selectable;
        }

        #endregion
    }
}
