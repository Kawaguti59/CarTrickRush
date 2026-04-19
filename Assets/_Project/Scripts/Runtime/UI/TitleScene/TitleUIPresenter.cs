using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using System.Collections.Generic;

using CarTrickRush.Core;
using CarTrickRush.UI.Common;

namespace CarTrickRush.UI.Title
{
    /// =========================================================================================
    /// <summary>
    /// タイトル画面UIの制御クラス.
    /// </summary>
    /// =========================================================================================
    public sealed class TitleUIPresenter : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// 設定ボタン.
        /// </summary>
        [SerializeField] private List<Button> _selectableButtons = default;

        /// <summary>
        /// インタラクション有効か.
        /// </summary>
        private bool _interactionsEnabled = default;

        /// <summary>
        /// 現在の選択ボタン.
        /// </summary>
        private Button _currentButton = default;

        /// <summary>
        /// 選択が外れた検知時刻.
        /// </summary>
        private float _lostFocusSinceUnscaled = -1f;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// 現在の選択ボタン.
        /// </summary>
        public Button CurrentButton => _currentButton;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            SetInteractions(true);
            foreach (var button in _selectableButtons)
            {
                BindPointerSelect(button);
            }
        }

        private void Start()
        {
            ManagerLocator.AudioManager?.PlayBgm("TitleBGM");

            var eventSystem = EventSystem.current;
            if (eventSystem == null || _selectableButtons.Count == 0) { return; }

            eventSystem.SetSelectedGameObject(_selectableButtons[0].gameObject);
            _currentButton = _selectableButtons[0];
        }

        private void LateUpdate()
        {
            if (!_interactionsEnabled) { return; }
            if (_selectableButtons.Count == 0) { return; }

            var eventSystem = EventSystem.current;
            if (eventSystem == null) { return; }

            var selectedGameObject = eventSystem.currentSelectedGameObject;
            switch (selectedGameObject)
            {
                // ゲーム開始ボタン.
                case GameObject gameObject when gameObject == _selectableButtons[0].gameObject:
                    _lostFocusSinceUnscaled = -1f;
                    _currentButton = _selectableButtons[0];
                    return;
                // 設定ボタン.
                case GameObject gameObject when gameObject == _selectableButtons[1].gameObject:
                    _lostFocusSinceUnscaled = -1f;
                    _currentButton = _selectableButtons[1];
                    return;
                // 終了ボタン.
                case GameObject gameObject when gameObject == _selectableButtons[2].gameObject:
                    _lostFocusSinceUnscaled = -1f;
                    _currentButton = _selectableButtons[2];
                    return;
            }

            // 選択解除時は現在のボタンを選択する.
            if (selectedGameObject == null && _currentButton != null)
            {
                if (UISelectionDelay.ShouldRestoreNow(ref _lostFocusSinceUnscaled, hasValidFocus: false))
                {
                    eventSystem.SetSelectedGameObject(_currentButton.gameObject);
                }
            }
            else if (selectedGameObject != null)
            {
                _lostFocusSinceUnscaled = -1f;
            }
        }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// 終了ボタン用。Button の OnClick から割り当てる.
        /// </summary>
        public void OnClickQuit()
        {
            UIButtonClickSound.Play();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// インタラクション有効かを設定する.
        /// </summary>
        /// <param name="enabled">有効か</param>
        private void SetInteractions(bool enabled)
        {
            _interactionsEnabled = enabled;

            foreach (var button in _selectableButtons)
            {
                if (button != null) { button.interactable = enabled; }
            }
        }

        /// <summary>
        /// ホバー時に選択を切り替える.
        /// </summary>
        private void BindPointerSelect(Button button)
        {
            if (button == null) { return; }

            var eventTrigger = button.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null) { return; }

            var pointerEnterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            pointerEnterEntry.callback.AddListener(_ => SelectButton(button));
            eventTrigger.triggers.Add(pointerEnterEntry);

            var pointerDownEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            pointerDownEntry.callback.AddListener(_ => SelectButton(button));
            eventTrigger.triggers.Add(pointerDownEntry);
        }

        private void SelectButton(Button button)
        {
            if (!_interactionsEnabled || button == null) { return; }

            var eventSystem = EventSystem.current;
            if (eventSystem == null) { return; }

            eventSystem.SetSelectedGameObject(button.gameObject);
            _currentButton = button;
        }

        #endregion
    }
}
