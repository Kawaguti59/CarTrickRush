using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
        /// ニューレコードか（<see cref="ResultSceneSession"/> から受け取り）.
        /// </summary>
        private bool _isNewRecord;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            ApplyResult();
            SetInteractionsEnabled(false);
        }

        private void Start()
        {
            if (!_waitForAnimationBeforeInteractions)
            {
                EnableResultInteractions();
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
        }

        #endregion
    }
}
