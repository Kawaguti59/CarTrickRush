using UnityEngine;

namespace CarTrickRush.DataLoading
{
    /// =========================================================================================
    /// <summary>
    /// プロジェクト用データアセットの読み込み入口.
    /// </summary>
    /// =========================================================================================
    public static class Data
    {
        /// <summary>
        /// 指定したパスからアセットを読み込む.
        /// </summary>
        /// <typeparam name="T">読み込むアセットの型.</typeparam>
        /// <param name="resourcePath">読み込むアセットのパス.</param>
        /// <returns>読み込んだアセット.</returns>
        public static T Load<T>(string resourcePath) where T : Object
        {
            return Resources.Load<T>(resourcePath);
        }
    }
}
