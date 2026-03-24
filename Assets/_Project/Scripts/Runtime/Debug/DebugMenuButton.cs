using UnityEngine;
using UnityEngine.UI;

using CarTrickRush.Managers;

namespace CarTrickRush.DebugMenu
{
    /// <summary>
    /// デバッグメニューボタンの処理を制御するクラス.
    /// </summary>
    public sealed class DebugMenuButton : MonoBehaviour
    {
        /// <summary>
        /// 動作の種類.
        /// </summary>
        public enum ActionType
        {
            /// <summary>
            /// シーンをロードする.
            /// </summary>
            LoadScene,
        }

        #region ------------------ Fields ------------------

        /// <summary>
        /// 実行する処理種別.
        /// </summary>
        [SerializeField] private ActionType _actionType = ActionType.LoadScene;

        /// <summary>
        /// 読み込み先シーン名.
        /// </summary>
        [SerializeField] private string _sceneName;

        /// <summary>
        /// ボタン本体.
        /// </summary>
        [SerializeField] private Button _button;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        /// <summary>
        /// ボタン押下イベントを登録する.
        /// </summary>
        private void Awake()
        {
            if (_button == null)
            {
                _button = GetComponent<Button>();
            }

            if (_button == null)
            {
                Debug.LogError("Buttonが見つからない.");
                return;
            }

            _button.onClick.AddListener(OnClick);
        }

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// ボタン押下時の処理.
        /// </summary>
        private void OnClick()
        {
            switch (_actionType)
            {
                case ActionType.LoadScene:
                    ExecuteLoadScene();
                    break;
            }
        }

        /// <summary>
        /// 指定シーンを読み込む.
        /// </summary>
        private void ExecuteLoadScene()
        {
            if (string.IsNullOrWhiteSpace(_sceneName))
            {
                Debug.LogWarning("シーン名が未設定.");
                return;
            }

            SceneLoadManager.LoadScene(_sceneName);
        }

        #endregion
    }
}