using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using System.Collections.Generic;

using CarTrickRush.Core;
using CarTrickRush.Managers;

namespace CarTrickRush.UI.Pause
{
    /// =========================================================================================
    /// <summary>
    /// ポーズオーバーレイのUIを制御するクラス.
    /// </summary>
    /// =========================================================================================
    public sealed class PauseUIPresenter : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// 選択操作の対象ボタンリスト.
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
        /// オーバーレイ表示前の timeScale.
        /// </summary>
        private float _timeScaleBeforePause = 1f;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// 現在の選択ボタン.
        /// </summary>
        public Button CurrentButton => _currentButton;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void OnEnable()
        {
            _timeScaleBeforePause = Time.timeScale;
            Time.timeScale = 0f;
        }

        private void OnDisable()
        {
            Time.timeScale = _timeScaleBeforePause;
        }

        private void Awake()
        {
            if (_selectableButtons == null)
            {
                return;
            }

            SetInteractions(true);
            foreach (var button in _selectableButtons)
            {
                BindPointerSelect(button);
            }
        }

        private void Start()
        {
            if (_selectableButtons == null || _selectableButtons.Count == 0) { return; }
            var eventSystem = EventSystem.current;
            if (eventSystem == null) { return; }

            eventSystem.SetSelectedGameObject(_selectableButtons[0].gameObject);
            _currentButton = _selectableButtons[0];
        }

        private void LateUpdate()
        {
            if (!_interactionsEnabled) { return; }
            if (_selectableButtons == null || _selectableButtons.Count == 0) { return; }
            var eventSystem = EventSystem.current;
            if (eventSystem == null) { return; }

            var selectedGameObject = eventSystem.currentSelectedGameObject;
            for (var i = 0; i < _selectableButtons.Count; i++)
            {
                var button = _selectableButtons[i];
                if (button == null)
                {
                    continue;
                }

                if (selectedGameObject == button.gameObject)
                {
                    _currentButton = button;
                    return;
                }
            }

            if (selectedGameObject == null && _currentButton != null)
            {
                eventSystem.SetSelectedGameObject(_currentButton.gameObject);
            }
        }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// ポーズシーンを閉じる.
        /// </summary>
        public void OnClickClosePause()
        {
            ManagerLocator.AudioManager?.PlaySe("ButtonClick");
            SceneLoadManager.UnloadScene("PauseScene");
        }

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// インタラクション有効かを設定する.
        /// </summary>
        /// <param name="enabled">有効か.</param>
        private void SetInteractions(bool enabled)
        {
            _interactionsEnabled = enabled;

            if (_selectableButtons == null)
            {
                return;
            }

            foreach (var button in _selectableButtons)
            {
                if (button != null)
                {
                    button.interactable = enabled;
                }
            }
        }

        /// <summary>
        /// ホバー時に選択を切り替える.
        /// </summary>
        private void BindPointerSelect(Button button)
        {
            if (button == null)
            {
                return;
            }

            var eventTrigger = button.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                return;
            }

            var pointerEnterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            pointerEnterEntry.callback.AddListener(_ => SelectButton(button));
            eventTrigger.triggers.Add(pointerEnterEntry);

            var pointerDownEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            pointerDownEntry.callback.AddListener(_ => SelectButton(button));
            eventTrigger.triggers.Add(pointerDownEntry);
        }

        /// <summary>
        /// ボタンを選択する.
        /// </summary>
        /// <param name="button">選択するボタン.</param>
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
