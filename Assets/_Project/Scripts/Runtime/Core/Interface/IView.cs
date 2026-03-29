namespace CarTrickRush.Core.View
{
    /// =========================================================================================
    /// <summary>
    /// Viewの共通インターフェース.
    /// </summary>
    /// =========================================================================================
    public interface IView
    {
        /// <summary>
        /// Viewを初期化する.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Viewを表示する.
        /// </summary>
        void Show();

        /// <summary>
        /// Viewを非表示にする.
        /// </summary>
        void Hide();

        /// <summary>
        /// アニメーションが再生中かどうか.
        /// </summary>
        bool IsPlaying();
    }
}