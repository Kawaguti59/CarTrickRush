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
        [SerializeField] private TextMeshProUGUI _scoreValueText;

        /// <summary>
        /// ベストスコア値テキスト.
        /// </summary>
        [SerializeField] private TextMeshProUGUI _bestScoreValueText;

        /// <summary>
        /// ニューレコード用の区切りライン.
        /// </summary>
        [SerializeField] private GameObject _lineBeforeNewRecord;

        /// <summary>
        /// ニューレコード表示ルート.
        /// </summary>
        [SerializeField] private GameObject _newRecordRoot;

        /// <summary>
        /// ニューレコード用スコア値テキスト.
        /// </summary>
        [SerializeField] private TextMeshProUGUI _newRecordBestScoreText;

        /// <summary>
        /// リトライボタン.
        /// </summary>
        [FormerlySerializedAs("_replayButton")]
        [SerializeField] private Button _retryButton;

        /// <summary>
        /// タイトルへ戻るボタン.
        /// </summary>
        [SerializeField] private Button _backToTitleButton;

        /// <summary>
        /// 最初にフォーカスするボタン.
        /// </summary>
        [SerializeField] private Button _initialSelectedButton;

        /// <summary>
        /// オンのとき、<see cref="EnableResultInteractions"/> が呼ばれるまでボタン操作を無効にする.
        /// </summary>
        [SerializeField] private bool _waitForAnimationBeforeInteractions;

        /// <summary>
        /// ボタン操作が有効かどうか.
        /// </summary>
        private bool _interactionsEnabled;

        /// <summary>
        /// リトライ／タイトルいずれかが選択されているときの GameObject（選択が null に落ちたときだけ復帰に使う）.
        /// </summary>
        private GameObject _lastSelectedButton;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            ApplyResult();
            SetupPointerDownSelection(_retryButton);
            SetupPointerDownSelection(_backToTitleButton);
            SetInteractionsEnabled(false);
        }

        private void Start()
        {
            if (!_waitForAnimationBeforeInteractions)
            {
                EnableResultInteractions();
            }
        }

        private void LateUpdate()
        {
            if (!_interactionsEnabled) { return; }

            var es = EventSystem.current;
            if (es == null) { return; }

            var sel = es.currentSelectedGameObject;
            if (sel == _retryButton.gameObject || sel == _backToTitleButton.gameObject)
            {
                _lastSelectedButton = sel;
                return;
            }

            if (sel == null && _lastSelectedButton != null)
            {
                es.SetSelectedGameObject(_lastSelectedButton);
            }
        }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// リザルトアニメーション完了後などに呼び出し、ボタン操作と初期フォーカスを有効にする.
        /// </summary>
        public void EnableResultInteractions()
        {
            SetInteractionsEnabled(true);
            StartCoroutine(SelectInitialButtonNextFrame());
        }

        /// <summary>
        /// リトライボタン用。Button の OnClick から割り当てる.
        /// </summary>
        public void OnClickRetry()
        {
            GameManager.Instance?.Replay();
        }

        /// <summary>
        /// タイトルへ戻るボタン用。Button の OnClick から割り当てる.
        /// </summary>
        public void OnClickBackToTitle()
        {
            GameManager.Instance?.ReturnToTitle();
        }

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// リザルト表示を反映する.
        /// </summary>
        private void ApplyResult()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            GameManager.Instance?.AssignDebugResultDataIfEmpty();
#endif
            var resultData = GameManager.Instance?.CurrentResultData;

            if (resultData == null) { return; }

            _scoreValueText.text = resultData.CurrentScore.ToString("N0");
            _bestScoreValueText.text = resultData.BestScore.ToString("N0");
            _newRecordBestScoreText.text = resultData.BestScore.ToString("N0");

            var isNew = resultData.IsNewRecord;
            _lineBeforeNewRecord.SetActive(isNew);
            _newRecordRoot.SetActive(isNew);
        }

        /// <summary>
        /// ボタン押下時の選択処理を設定する.
        /// </summary>
        private void SetupPointerDownSelection(Button button)
        {
            var trigger = button.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = button.gameObject.AddComponent<EventTrigger>();
            }

            var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            entry.callback.AddListener(_ => OnButtonPointerDown(button));
            trigger.triggers.Add(entry);
        }

        /// <summary>
        /// ボタン押下時の選択処理.
        /// </summary>
        private void OnButtonPointerDown(Button button)
        {
            if (!_interactionsEnabled) { return; }

            var es = EventSystem.current;
            if (es == null) { return; }

            es.SetSelectedGameObject(button.gameObject);
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
        /// <see cref="_initialSelectedButton"/> を次のフレームで選択する.
        /// </summary>
        private IEnumerator SelectInitialButtonNextFrame()
        {
            yield return null;

            if (!_interactionsEnabled) { yield break; }

            var es = EventSystem.current;
            if (es == null) { yield break; }

            es.SetSelectedGameObject(_initialSelectedButton.gameObject);
            _lastSelectedButton = _initialSelectedButton.gameObject;
        }

        #endregion
    }
}
