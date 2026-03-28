using UnityEngine;

using CarTrickRush.Core;

namespace CarTrickRush.Managers
{
    /// =========================================================================================
    /// <summary>
    /// ゲーム全体の進行状態管理Manager.
    /// </summary>
    /// =========================================================================================
    public sealed class GameManager : MonoBehaviour
    {
        private static GameManager _instance = default;

        public static GameManager Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            ManagerLocator.Register(this);
        }
    }
}
