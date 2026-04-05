using UnityEngine;
using UnityEngine.EventSystems;
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
        /// リザルト画面の表示を扱うView.
        /// </summary>
        [SerializeField] private ResultUIPresenterView _resultUIPresenterView = default;

        /// <summary>
        /// ボタン操作が有効かどうか.
        /// </summary>
        private bool _canOperate = false;

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
            BindPointerSelect(_retryButton);
            BindPointerSelect(_backToTitleButton);
            SetInteractionsEnabled(false);
        }

        private void Start()
        {
            StartCoroutine(InitializeCoroutine());
        }

        private void LateUpdate()
        {
            if (!_canOperate) { return; }
            if (_retryButton == null || _backToTitleButton == null) { return; }

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

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// イントロがあれば終了まで待ち、操作可能にして初期フォーカスを当てる.
        /// </summary>
        private IEnumerator InitializeCoroutine()
        {
            if (_resultUIPresenterView != null)
            {
                _resultUIPresenterView.Show();
                yield return null;
                while (_resultUIPresenterView.IsPlaying())
                {
                    yield return null;
                }
            }

            yield return StartCoroutine(SetupButtonCoroutine());
        }

        /// <summary>
        /// ボタンの初期設定を行う.
        /// </summary>
        private IEnumerator SetupButtonCoroutine()
        {
            SetInteractionsEnabled(true);
            yield return null;

            var eventSystem = EventSystem.current;
            if (eventSystem == null || _initialSelectedButton == null) { yield break; }

            eventSystem.SetSelectedGameObject(_initialSelectedButton.gameObject);
            _currentButton = _initialSelectedButton;
        }

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
                _resultUIPresenterView?.SetIsNewRecord(_isNewRecord);
                return;
            }

            _scoreValueText.text = resultData.CurrentScore.ToString("N0");
            _bestScoreValueText.text = resultData.BestScore.ToString("N0");
            _newRecordBestScoreText.text = resultData.BestScore.ToString("N0");

            _lineBeforeNewRecord.SetActive(_isNewRecord);
            _newRecordRoot.SetActive(_isNewRecord);
            _resultUIPresenterView?.SetIsNewRecord(_isNewRecord);
        }

        /// <summary>
        /// ボタン操作が有効かどうかを設定する.
        /// </summary>
        private void SetInteractionsEnabled(bool enabled)
        {
            _canOperate = enabled;

            if (_retryButton != null) { _retryButton.interactable = enabled; }
            if (_backToTitleButton != null) { _backToTitleButton.interactable = enabled; }
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
            if (!_canOperate || button == null) { return; }

            var eventSystem = EventSystem.current;
            if (eventSystem == null) { return; }

            eventSystem.SetSelectedGameObject(button.gameObject);
            _currentButton = button;
        }

        #endregion
    }
}
