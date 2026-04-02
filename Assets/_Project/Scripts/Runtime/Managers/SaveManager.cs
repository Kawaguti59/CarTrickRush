using UnityEngine;

using CarTrickRush.Core;
using CarTrickRush.Data;

namespace CarTrickRush.Managers
{
    /// =========================================================================================
    /// <summary>
    /// セーブデータの保存と読込を管理するクラス.
    /// </summary>
    /// =========================================================================================
    public sealed class SaveManager : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// ベストスコアのキー.
        /// </summary>
        private const string BestScoreKey = "BEST_SCORE";

        /// <summary>
        /// ディスクへ永続化するか（WebGL プレイヤーではオフ）.
        /// </summary>
        private static bool PersistBestScoreToDisk =>
            Application.platform != RuntimePlatform.WebGLPlayer;

        /// <summary>
        /// セーブデータ.
        /// </summary>
        private UserSaveData _userSaveData = default;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// 現在のベストスコア.
        /// </summary>
        public int BestScore => _userSaveData?.BestScore ?? 0;

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            ManagerLocator.Register(this);
            Load();
        }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// セーブデータを読み込む.
        /// </summary>
        public void Load()
        {
            _userSaveData = new UserSaveData();

            if (!PersistBestScoreToDisk) { return; }

            var bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
            _userSaveData.SetBestScore(bestScore);
        }

        /// <summary>
        /// セーブデータを保存する.
        /// </summary>
        public void Save()
        {
            if (!PersistBestScoreToDisk) { return; }

            PlayerPrefs.SetInt(BestScoreKey, _userSaveData?.BestScore ?? 0);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// ベストスコアを更新する.
        /// </summary>
        /// <param name="currentScore">今回スコア.</param>
        public void UpdateBestScore(int currentScore)
        {
            if (currentScore <= (_userSaveData?.BestScore ?? 0)) { return; }

            _userSaveData?.SetBestScore(currentScore);
            Save();
        }

        #endregion
    }
}