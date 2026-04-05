using UnityEngine;

using CarTrickRush.Core;
using CarTrickRush.Managers;

namespace CarTrickRush.UI.Common
{
    /// =========================================================================================
    /// <summary>
    /// シーン遷移用のボタンクラス.
    /// </summary>
    /// =========================================================================================
    public sealed class SceneNavigationOnClick : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// 読み込みモード.
        /// </summary>
        public enum LoadMode
        {
            /// <summary>
            /// 単一シーン読み込み（現在のシーンを置き換え）.
            /// </summary>
            Single,

            /// <summary>
            /// 加算読み込み（オーバーレイ等）.
            /// </summary>
            Additive,
        }

        /// <summary>
        /// 遷移先シーン名.
        /// </summary>     
        [SerializeField] private string _sceneName = default;

        /// <summary>
        /// 読み込みモード.
        /// </summary>
        [SerializeField] private LoadMode _loadMode = LoadMode.Single;

        /// <summary>
        /// ルールフェードのセットID.
        /// </summary>
        [SerializeField] private int _transitionSetId = 0;

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// Button の OnClick に割り当てる.
        /// </summary>
        public void SceneLoad()
        {
            if (string.IsNullOrWhiteSpace(_sceneName))
            {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogWarning($"{nameof(SceneNavigationOnClick)}: シーン名が未設定です ({gameObject.name}).");
                #endif
                return;
            }

            switch (_loadMode)
            {
                // 単一シーン読み込み
                case LoadMode.Single:
                    if (SceneLoadManager.IsSingleLoadTransitionRunning) { return; }
                    SceneLoadManager.LoadScene(_sceneName, _transitionSetId);
                    UIButtonClickSound.Play();
                    break;
                // 加算読み込み
                case LoadMode.Additive:
                    if (SceneLoadManager.IsSceneLoaded(_sceneName)) { return; }
                    SceneLoadManager.LoadSceneAdditive(_sceneName);
                    UIButtonClickSound.Play();
                    break;
                default:
                    #if UNITY_EDITOR || DEVELOPMENT_BUILD
                    Debug.LogWarning($"{nameof(SceneNavigationOnClick)}: 読み込みモードが未設定です ({gameObject.name}).");
                    #endif
                    break;
            }
        }

        #endregion
    }
}
