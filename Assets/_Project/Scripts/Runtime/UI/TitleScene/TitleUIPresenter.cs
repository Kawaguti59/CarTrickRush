using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        [SerializeField] private Button _startGameButton = default;

        /// <summary>
        /// 設定ボタン.
        /// </summary>
        [SerializeField] private Button _openSettingsButton = default;

        /// <summary>
        /// 終了ボタン.
        /// </summary>
        [SerializeField] private Button _quitButton = default;

        /// <summary>
        /// 初期選択ボタン.
        /// </summary>  
        [SerializeField] private Button _initialSelectedButton = default;

        /// <summary>
        /// インタラクション有効か.
        /// </summary>
        private bool _interactionsEnabled = default;

        /// <summary>
        /// 現在の選択ボタン.
        /// </summary>
        private Button _currentButton = default;

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
        }

        private void Start()
        {
            var eventSystem = EventSystem.current;
            if (eventSystem == null || _initialSelectedButton == null) { return; }

            eventSystem.SetSelectedGameObject(_initialSelectedButton.gameObject);
            _currentButton = _initialSelectedButton;
        }

        private void LateUpdate()
        {
            if (!_interactionsEnabled) { return; }
            if (_startGameButton == null || _openSettingsButton == null || _quitButton == null) { return; }

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
        /// インタラクション有効かを設定する.
        /// </summary>
        /// <param name="enabled">有効か</param>
        private void SetInteractions(bool enabled)
        {
            _interactionsEnabled = enabled;

            if (_startGameButton != null) { _startGameButton.interactable = enabled; }
            if (_openSettingsButton != null) { _openSettingsButton.interactable = enabled; }
            if (_quitButton != null) { _quitButton.interactable = enabled; }
        }

        #endregion
    }
}
