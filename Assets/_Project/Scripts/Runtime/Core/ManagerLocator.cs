using UnityEngine;

using CarTrickRush.Managers;

namespace CarTrickRush.Core
{
    /// =========================================================================================
    /// <summary>
    /// 常駐Managerへの参照を管理するロケータークラス.
    /// </summary>
    /// <remarks>
    /// Bootで生成するManagerのみ登録対象とする.
    /// </remarks>
    /// =========================================================================================
    public static class ManagerLocator
    {
        #region ------------------ Properties ------------------

        /// <summary>
        /// ゲーム全体管理クラス.
        /// </summary>
        public static GameManager GameManager { get; private set; }

        /// <summary>
        /// シーン遷移管理クラス.
        /// </summary>
        public static SceneLoadManager SceneLoadManager { get; private set; }

        /// <summary>
        /// 入力管理クラス.
        /// </summary>
        public static InputManager InputManager { get; private set; }

        /// <summary>
        /// 時間管理クラス.
        /// </summary>
        public static TimeManager TimeManager { get; private set; }

        /// <summary>
        /// スコア管理クラス.
        /// </summary>
        public static ScoreManager ScoreManager { get; private set; }

        /// <summary>
        /// セーブ管理クラス.
        /// </summary>
        public static SaveManager SaveManager { get; private set; }

        /// <summary>
        /// 音声管理クラス.
        /// </summary>
        public static AudioManager AudioManager { get; private set; }

        #endregion

        #region ------------------ Public Methods ------------------

        /// <summary>
        /// GameManagerを登録する.
        /// </summary>
        /// <param name="manager">GameManager</param>
        public static void Register(GameManager manager)
        {
            if (manager == null)
            {
                Debug.LogError($"{nameof(ManagerLocator)} : GameManager is null.");
                return;
            }

            GameManager = manager;
        }

        /// <summary>
        /// SceneLoadManagerを登録する.
        /// </summary>
        /// <param name="manager">SceneLoadManager</param>
        public static void Register(SceneLoadManager manager)
        {
            if (manager == null)
            {
                Debug.LogError($"{nameof(ManagerLocator)} : SceneLoadManager is null.");
                return;
            }

            SceneLoadManager = manager;
        }

        /// <summary>
        /// InputManagerを登録する.
        /// </summary>
        /// <param name="manager">InputManager</param>
        public static void Register(InputManager manager)
        {
            if (manager == null)
            {
                Debug.LogError($"{nameof(ManagerLocator)} : InputManager is null.");
                return;
            }

            InputManager = manager;
        }

        /// <summary>
        /// TimeManagerを登録する.
        /// </summary>
        /// <param name="manager">TimeManager</param>
        public static void Register(TimeManager manager)
        {
            if (manager == null)
            {
                Debug.LogError($"{nameof(ManagerLocator)} : TimeManager is null.");
                return;
            }

            TimeManager = manager;
        }

        /// <summary>
        /// ScoreManagerを登録する.
        /// </summary>
        /// <param name="manager">ScoreManager</param>
        public static void Register(ScoreManager manager)
        {
            if (manager == null)
            {
                Debug.LogError($"{nameof(ManagerLocator)} : ScoreManager is null.");
                return;
            }

            ScoreManager = manager;
        }

        /// <summary>
        /// SaveManagerを登録する.
        /// </summary>
        /// <param name="manager">SaveManager</param>
        public static void Register(SaveManager manager)
        {
            if (manager == null)
            {
                Debug.LogError($"{nameof(ManagerLocator)} : SaveManager is null.");
                return;
            }

            SaveManager = manager;
        }

        /// <summary>
        /// AudioManagerを登録する.
        /// </summary>
        /// <param name="manager">AudioManager</param>
        public static void Register(AudioManager manager)
        {
            if (manager == null)
            {
                Debug.LogError($"{nameof(ManagerLocator)} : AudioManager is null.");
                return;
            }

            AudioManager = manager;
        }

        /// <summary>
        /// 全Manager参照をクリアする.
        /// </summary>
        public static void Clear()
        {
            GameManager = null;
            SceneLoadManager = null;
            InputManager = null;
            TimeManager = null;
            ScoreManager = null;
            SaveManager = null;
            AudioManager = null;
        }

        #endregion
    }
}