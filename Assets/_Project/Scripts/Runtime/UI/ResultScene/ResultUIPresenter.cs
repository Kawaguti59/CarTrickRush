using UnityEngine;
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
        /// SCOREラベル.
        /// </summary>
        [SerializeField] private TextMeshProUGUI _scoreLabelText;

        /// <summary>
        /// 今回スコア表示.
        /// </summary>
        [SerializeField] private TextMeshProUGUI _scoreValueText;

        /// <summary>
        /// ベストスコアラベル.
        /// </summary>
        [SerializeField] private TextMeshProUGUI _bestScoreLabelText;

        /// <summary>
        /// ベストスコア表示.
        /// </summary>
        [SerializeField] private TextMeshProUGUI _bestScoreValueText;

        /// <summary>
        /// ニューレコード表示.
        /// </summary>
        [SerializeField] private GameObject _newRecordRoot;

        /// <summary>
        /// リプレイボタン.
        /// </summary>
        [SerializeField] private Button _replayButton;

        /// <summary>
        /// タイトルへ戻るボタン.
        /// </summary>
        [SerializeField] private Button _backToTitleButton;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            BindButtons();
            ApplyResult();
        }

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// ボタンイベントを設定する.
        /// </summary>
        private void BindButtons()
        {
            _replayButton.onClick.RemoveAllListeners();
            _replayButton.onClick.AddListener(OnClickReplay);

            _backToTitleButton.onClick.RemoveAllListeners();
            _backToTitleButton.onClick.AddListener(OnClickBackToTitle);
        }

        /// <summary>
        /// リザルト表示を反映する.
        /// </summary>
        private void ApplyResult()
        {
            var resultData = GameManager.Instance?.CurrentResultData;

            if (resultData == null) { return; }

            _scoreLabelText.text = "SCORE";
            _scoreValueText.text = resultData.CurrentScore.ToString("N0");
            _bestScoreLabelText.text = "BEST SCORE";
            _bestScoreValueText.text = resultData.BestScore.ToString("N0");
            _newRecordRoot.SetActive(resultData.IsNewRecord);
        }

        /// <summary>
        /// リプレイボタン押下処理.
        /// </summary>
        private void OnClickReplay()
        {
            GameManager.Instance?.Replay();
        }

        /// <summary>
        /// タイトル戻るボタン押下処理.
        /// </summary>
        private void OnClickBackToTitle()
        {
            GameManager.Instance?.ReturnToTitle();
        }

        #endregion
    }
}