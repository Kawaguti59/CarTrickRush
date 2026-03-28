using UnityEngine;

using CarTrickRush.Core;

namespace CarTrickRush.Managers
{
    /// =========================================================================================
    /// <summary>
    /// オーディオ管理Manager.
    /// </summary>
    /// =========================================================================================
    public sealed class AudioManager : MonoBehaviour
    {
        #region ------------------ Fields ------------------

        /// <summary>
        /// インスタンス.
        /// </summary>
        private static AudioManager _instance = default;

        /// <summary>
        /// BGM 用 AudioSource.
        /// </summary>
        private AudioSource _bgmSource = default;

        /// <summary>
        /// SE 用 AudioSource.
        /// </summary>
        private AudioSource _seSource = default;

        /// <summary>
        /// BGM音量 (0〜1).
        /// </summary>
        [SerializeField] [Range(0f, 1f)] private float _bgmVolume = 1f;

        /// <summary>
        /// SE音量 (0〜1).
        /// </summary>
        [SerializeField] [Range(0f, 1f)] private float _seVolume = 1f;

        #endregion

        #region ------------------ Properties ------------------

        /// <summary>
        /// インスタンス.
        /// </summary>
        public static AudioManager Instance => _instance;

        /// <summary>
        /// BGM音量 (0〜1).
        /// </summary>
        public float BgmVolume
        {
            get => _bgmVolume;
            set
            {
                _bgmVolume = Mathf.Clamp01(value);
                ApplyBGMVolume();
            }
        }

        /// <summary>
        /// SE音量 (0〜1).
        /// </summary>
        public float SeVolume
        {
            get => _seVolume;
            set
            {
                _seVolume = Mathf.Clamp01(value);
                ApplySEVolume();
            }
        }

        #endregion

        #region ------------------ MonoBehaviour Methods ------------------

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            EnsureSources();
            ApplyBGMVolume();
            ApplySEVolume();
            ManagerLocator.Register(this);
        }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// BGMを再生する.
        /// </summary>
        /// <param name="clip">クリップ.</param>
        /// <param name="loop">ループするか.</param>
        public void PlayBgm(AudioClip clip, bool loop = true)
        {
            if (clip == null || _bgmSource == null) { return; }

            // 再生中の場合は再生しない.
            if (_bgmSource.isPlaying && _bgmSource.clip == clip && _bgmSource.loop == loop) { return; }

            // 再生する.
            _bgmSource.clip = clip;
            _bgmSource.loop = loop;
            ApplyBGMVolume();
            _bgmSource.Play();
        }

        /// <summary>
        /// BGMを停止する.
        /// </summary>
        public void StopBGM()
        {
            if (_bgmSource == null) { return; }
            
            // 停止する.
            _bgmSource.Stop();
            _bgmSource.clip = null;
        }

        /// <summary>
        /// SEを再生する.
        /// </summary>
        /// <param name="clip">クリップ.</param>
        /// <param name="volumeScale">SE チャンネル音量に乗算するスケール (0〜1 推奨).</param>
        public void PlaySE(AudioClip clip, float volumeScale = 1f)
        {
            if (clip == null || _seSource == null) { return; }
            
            // 音量を適用して再生する.
            _seSource.PlayOneShot(clip, Mathf.Max(0f, volumeScale));
        }

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// AudioSourceを確保する.
        /// </summary>
        private void EnsureSources()
        {
            if (_bgmSource != null && _seSource != null) { return; }

            // AudioSourceを作成する.
            _bgmSource = CreateChildAudioSource("BgmSource", loop: true);
            _seSource = CreateChildAudioSource("SeSource", loop: false);
        }

        /// <summary>
        /// 子オブジェクトのAudioSourceを作成する.
        /// </summary>
        /// <param name="objectName">オブジェクト名.</param>
        /// <param name="loop">ループするか.</param>
        /// <returns>AudioSource.</returns>
        private AudioSource CreateChildAudioSource(string objectName, bool loop)
        {
            // 子オブジェクトを作成する.
            GameObject child = new GameObject(objectName);
            child.transform.SetParent(transform, false);
            // AudioSourceを追加する.
            AudioSource source = child.AddComponent<AudioSource>();
            // 再生しない.
            source.playOnAwake = false;
            // ループする.
            source.loop = loop;
            // AudioSourceを返す.
            return source;
        }

        /// <summary>
        /// BGM音量を適用する.
        /// </summary>
        private void ApplyBGMVolume()
        {
            if (_bgmSource != null)
            {
                _bgmSource.volume = _bgmVolume;
            }
        }

        /// <summary>
        /// SE音量を適用する.
        /// </summary>
        private void ApplySEVolume()
        {
            if (_seSource != null)
            {
                _seSource.volume = _seVolume;
            }
        }

        #endregion
    }
}
