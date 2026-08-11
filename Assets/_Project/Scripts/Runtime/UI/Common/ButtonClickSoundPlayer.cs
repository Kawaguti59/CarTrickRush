using UnityEngine;

using CarTrickRush.Managers;

namespace CarTrickRush.UI.Common
{
    /// =========================================================================================
    /// <summary>
    /// ボタンクリック SE を再生するクラス.
    /// </summary>
    /// =========================================================================================
    public sealed class ButtonClickSoundPlayer
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// インターバル用の最小間隔 (秒).
        /// </summary>
        private const float MinIntervalSeconds = 0.12f;

        /// <summary>
        /// インターバル用の最後に再生した時刻.
        /// </summary>
        private float _lastPlayedUnscaledTime = float.NegativeInfinity;

        /// <summary>
        /// インターバル用の最後に再生したフレーム.
        /// </summary>
        private int _lastPlayFrame = -1;

        private readonly AudioManager _audioManager;

        #endregion

        #region ------------------ Constructor ------------------

        public ButtonClickSoundPlayer(AudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// ボタンクリック SE を、インターバル内の重複があればスキップして再生する.
        /// </summary>
        public void Play()
        {
            if (_audioManager == null) { return; }

            var frame = Time.frameCount;
            if (frame == _lastPlayFrame) { return; }

            var time = Time.unscaledTime;
            if (time - _lastPlayedUnscaledTime < MinIntervalSeconds) { return; }

            _lastPlayFrame = frame;
            _lastPlayedUnscaledTime = time;
            _audioManager.PlaySe("ButtonClick");
        }

        #endregion
    }
}
