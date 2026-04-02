using UnityEditor;
using UnityEngine;

using CarTrickRush.Managers;

namespace CarTrickRush.Editor
{
    /// =========================================================================================
    /// <summary>
    /// エディタからセーブデータを削除するデバッグ用メニュー.
    /// </summary>
    /// =========================================================================================
    public static class SaveDataDebugMenu
    {
        #region ------------------ Private Methods ------------------

        [MenuItem("CarTrickRush/セーブデータを削除")]
        private static void ClearSaveData()
        {
            if (!EditorUtility.DisplayDialog(
                    "セーブデータの削除",
                    "PlayerPrefs に保存されているベストスコアを削除します。元に戻せません。",
                    "削除",
                    "キャンセル"))
            {
                return;
            }

            PlayerPrefs.DeleteKey(SaveManager.BestScorePlayerPrefsKey);
            PlayerPrefs.Save();
            Debug.Log("[CarTrickRush] セーブデータを削除しました（ベストスコア）。");
        }

        #endregion
    }
}
