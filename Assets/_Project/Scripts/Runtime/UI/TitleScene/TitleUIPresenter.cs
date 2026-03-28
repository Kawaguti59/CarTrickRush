using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using System.Collections;

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
        /// ゲーム開始ボタン.
        /// </summary>
        [SerializeField] private Button _startGameButton;

        /// <summary>
        /// 設定ボタン.
        /// </summary>
        [SerializeField] private Button _openSettingsButton;

        /// <summary>
        /// 終了ボタン.
        /// </summary>
        [SerializeField] private Button _quitButton;

        /// <summary>
        /// 初期選択ボタン.
        /// </summary>  
        [SerializeField] private Button _initialSelectedButton;

        /// <summary>
        /// インタラクション有効か.
        /// </summary>
        private bool _interactionsEnabled;

        /// <summary>
        /// 現在の選択ボタン.
        /// </summary>
        private Button _currentButton;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// 現在のフォーカス対象ボタン。選択解除時はこの参照へ戻す.
        /// </summary>
        public Button CurrentButton => _currentButton;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            SetupButtonPointerSyncSelection(_startGameButton);
            SetupButtonPointerSyncSelection(_openSettingsButton);
            SetupButtonPointerSyncSelection(_quitButton);
            SetInteractionsEnabled(true);
        }

        private void Start()
        {
            StartCoroutine(SelectInitialButtonNextFrame());
        }

        private void LateUpdate()
        {
            if (!_interactionsEnabled) { return; }

            var eventSystem = EventSystem.current;
            if (eventSystem == null) { return; }

            var selectedGameObject = eventSystem.currentSelectedGameObject;
            switch (selectedGameObject)
            {
                // ゲーム開始ボタン.
                case GameObject gameObject when gameObject == _startGameButton.gameObject:
                    _currentButton = _startGameButton;
                    return;
                // 設定ボタン.
                case GameObject go when go == _openSettingsButton.gameObject:
                    _currentButton = _openSettingsButton;
                    return;
                // 終了ボタン.
                case GameObject go when go == _quitButton.gameObject:
                    _currentButton = _quitButton;
                    return;
            }

            // 選択解除時は現在のボタンを選択する.
            if (selectedGameObject == null && _currentButton != null)
            {
                eventSystem.SetSelectedGameObject(_currentButton.gameObject);
            }
        }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// 終了ボタン用。Button の OnClick から割り当てる.
        /// </summary>
        public void OnClickQuit()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// ボタンのポインター同期選択を設定する.
        /// </summary>
        /// <param name="button">ボタン</param>
        private void SetupButtonPointerSyncSelection(Button button)
        {
            var eventTrigger = button.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = button.gameObject.AddComponent<EventTrigger>();
            }

            var pointerEnterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            pointerEnterEntry.callback.AddListener(_ => SyncSelectionToButton(button));
            eventTrigger.triggers.Add(pointerEnterEntry);

            var pointerDownEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            pointerDownEntry.callback.AddListener(_ => SyncSelectionToButton(button));
            eventTrigger.triggers.Add(pointerDownEntry);
        }

        /// <summary>
        /// 選択ボタンを同期する.
        /// </summary>
        /// <param name="button">ボタン</param>
        private void SyncSelectionToButton(Button button)
        {
            if (!_interactionsEnabled) { return; }

            var eventSystem = EventSystem.current;
            if (eventSystem == null) { return; }

            eventSystem.SetSelectedGameObject(button.gameObject);
            _currentButton = button;
        }

        /// <summary>
        /// インタラクション有効かを設定する.
        /// </summary>
        /// <param name="enabled">有効か</param>
        private void SetInteractionsEnabled(bool enabled)
        {
            _interactionsEnabled = enabled;

            _startGameButton.interactable = enabled;
            _openSettingsButton.interactable = enabled;
            _quitButton.interactable = enabled;
        }

        /// <summary>
        /// 初期選択ボタンを選択する.
        /// </summary>
        /// <returns>コルーチン</returns>
        private IEnumerator SelectInitialButtonNextFrame()
        {
            yield return null;

            if (!_interactionsEnabled) { yield break; }

            var eventSystem = EventSystem.current;
            if (eventSystem == null) { yield break; }

            if (_initialSelectedButton == null)
            {
                yield break;
            }

            eventSystem.SetSelectedGameObject(_initialSelectedButton.gameObject);
            _currentButton = _initialSelectedButton;
        }

        #endregion
    }
}
