using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

using System.Collections;

using TMPro;

using CarTrickRush.Data;
using CarTrickRush.Managers;

namespace CarTrickRush.UI.Result
{
    /// =========================================================================================
    /// <summary>
    /// リザルト画面表示制御クラス.
    /// </summary>
    /// =========================================================================================
    public sealed class ResultUIPresenter : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// スコア値テキスト.
        /// </summary>
        [SerializeField] private TextMeshProUGUI _scoreValueText = default;

        /// <summary>
        /// ベストスコア値テキスト.
        /// </summary>
        [SerializeField] private TextMeshProUGUI _bestScoreValueText = default;

        /// <summary>
        /// ニューレコード用の区切りライン.
        /// </summary>
        [SerializeField] private GameObject _lineBeforeNewRecord = default;

        /// <summary>
        /// ニューレコード表示ルート.
        /// </summary>
        [SerializeField] private GameObject _newRecordRoot = default;

        /// <summary>
        /// ニューレコード用スコア値テキスト.
        /// </summary>
        [SerializeField] private TextMeshProUGUI _newRecordBestScoreText = default;

        /// <summary>
        /// リトライボタン.
        /// </summary>
        [FormerlySerializedAs("_replayButton")]
        [SerializeField] private Button _retryButton = default;

        /// <summary>
        /// タイトルへ戻るボタン.
        /// </summary>
        [SerializeField] private Button _backToTitleButton = default;

        /// <summary>
        /// 最初にフォーカスするボタン.
        /// </summary>
        [SerializeField] private Button _initialSelectedButton = default;

        /// <summary>
        /// オンならイントロを待たず操作できる.
        /// </summary>
        [SerializeField] private bool _canOperate = default;

        /// <summary>
        /// ボタン操作が有効かどうか.
        /// </summary>
        private bool _interactionsEnabled = default;

        /// <summary>
        /// ニューレコードか.
        /// </summary>
        private bool _isNewRecord = default;

        /// <summary>
        /// 現在のフォーカス対象.
        /// </summary>
        private Button _currentButton = default;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// 現在のフォーカス対象ボタン.
        /// </summary>
        public Button CurrentButton => _currentButton;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            ApplyResult();
            SetupButtonPointerSyncSelection(_retryButton);
            SetupButtonPointerSyncSelection(_backToTitleButton);
            SetInteractionsEnabled(false);
        }

        private void Start()
        {
            if (_canOperate)
            {
                EnableResultInteractions();
            }
        }

        private void LateUpdate()
        {
            if (!_interactionsEnabled) { return; }

            var eventSystem = EventSystem.current;
            if (eventSystem == null) { return; }

            var selectedGameObject = eventSystem.currentSelectedGameObject;
            if (selectedGameObject == _retryButton.gameObject)
            {
                _currentButton = _retryButton;
                return;
            }

            if (selectedGameObject == _backToTitleButton.gameObject)
            {
                _currentButton = _backToTitleButton;
                return;
            }

            if (selectedGameObject == null && _currentButton != null)
            {
                eventSystem.SetSelectedGameObject(_currentButton.gameObject);
            }
        }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// ボタン操作と初期フォーカスを有効にする.
        /// </summary>
        public void EnableResultInteractions()
        {
            SetInteractionsEnabled(true);
            StartCoroutine(SelectInitialButtonNextFrame());
        }

        /// <summary>
        /// 再プレイボタン処理.
        /// </summary>
        public void OnClickRetry()
        {
            SceneLoadManager.LoadScene("GameScene", 0);
        }

        /// <summary>
        /// タイトルへ戻るボタン処理.
        /// </summary>
        public void OnClickBackToTitle()
        {
            SceneLoadManager.LoadScene("TitleScene", 0);
        }

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// リザルト表示を反映する.
        /// </summary>
        private void ApplyResult()
        {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            ResultSceneSession.AssignDebugPlaceholderIfEmpty();
            #endif

            if (!ResultSceneSession.TryConsume(out var resultData, out _isNewRecord))
            {
                return;
            }

            _scoreValueText.text = resultData.CurrentScore.ToString("N0");
            _bestScoreValueText.text = resultData.BestScore.ToString("N0");
            _newRecordBestScoreText.text = resultData.BestScore.ToString("N0");

            _lineBeforeNewRecord.SetActive(_isNewRecord);
            _newRecordRoot.SetActive(_isNewRecord);
        }

        /// <summary>
        /// ボタンのポインター同期選択を設定する.
        /// </summary>
        /// <param name="button">ボタン.</param>
        private void SetupButtonPointerSyncSelection(Button button)
        {
            var eventTrigger = button.gameObject.GetComponent<EventTrigger>();

            var pointerEnterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            pointerEnterEntry.callback.AddListener(_ => SyncSelectionToButton(button));
            eventTrigger.triggers.Add(pointerEnterEntry);

            var pointerDownEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            pointerDownEntry.callback.AddListener(_ => SyncSelectionToButton(button));
            eventTrigger.triggers.Add(pointerDownEntry);
        }

        /// <summary>
        /// 選択をボタンに同期する.
        /// </summary>
        /// <param name="button">ボタン.</param>
        private void SyncSelectionToButton(Button button)
        {
            if (!_interactionsEnabled) { return; }

            var eventSystem = EventSystem.current;
            if (eventSystem == null) { return; }

            eventSystem.SetSelectedGameObject(button.gameObject);
            _currentButton = button;
        }

        /// <summary>
        /// ボタン操作が有効かどうかを設定する.
        /// </summary>
        private void SetInteractionsEnabled(bool enabled)
        {
            _interactionsEnabled = enabled;

            _retryButton.interactable = enabled;
            _backToTitleButton.interactable = enabled;
        }

        /// <summary>
        /// 最初に選択するボタンを設定する.
        /// </summary>
        private IEnumerator SelectInitialButtonNextFrame()
        {
            yield return null;

            if (!_interactionsEnabled) { yield break; }

            var eventSystem = EventSystem.current;
            if (eventSystem == null) { yield break; }

            eventSystem.SetSelectedGameObject(_initialSelectedButton.gameObject);
            _currentButton = _initialSelectedButton;
        }

        #endregion
    }
}
