using UnityEngine;

using VContainer;
using VContainer.Unity;

using CarTrickRush.Managers;

namespace CarTrickRush.Runtime.Core.VContainer
{
    /// =========================================================================================
    /// <summary>
    /// ルートライフタイムスコープ.
    /// </summary>
    /// =========================================================================================
    public sealed class RootLifetimeScope : LifetimeScope
    {
        #region ------------------ Constants ------------------

        private const string ManagersRootName = "Managers";

        #endregion

        #region ------------------ Methods ------------------

        protected override void Awake()
        {
            DontDestroyOnLoad(gameObject);
            base.Awake();
        }

        /// <summary>
        /// コンフィグレーションを設定します.
        /// </summary>
        /// <param name="builder">コンテナビルダー.</param>
        protected override void Configure(IContainerBuilder builder)
        {
            var managersRoot = GetManagersRoot();

            builder.RegisterComponentInHierarchy<AudioManager>();

            RegisterManager<GameManager>(builder, managersRoot);
            RegisterManager<SceneLoadManager>(builder, managersRoot);
            RegisterManager<InputManager>(builder, managersRoot);
            RegisterManager<TimeManager>(builder, managersRoot);
            RegisterManager<SaveManager>(builder, managersRoot);
            RegisterManager<ScoreManager>(builder, managersRoot);

            builder.RegisterBuildCallback(resolver =>
            {
                foreach (Transform child in transform)
                {
                    resolver.InjectGameObject(child.gameObject);
                }
            });
        }

        #endregion

        #region ------------------ Private Methods ------------------

        /// <summary>
        /// Managers のルートを取得します.
        /// </summary>
        /// <returns>Managers のルート.</returns>
        Transform GetManagersRoot()
        {
            var existing = transform.Find(ManagersRootName);
            if (existing == null)
            {
                Debug.LogError($"{nameof(RootLifetimeScope)} : '{ManagersRootName}' が見つかりません。Root プレハブに配置してください。");
                return transform;
            }

            return existing;
        }

        /// <summary>
        /// マネージャーを登録します.
        /// </summary>
        /// <typeparam name="T">マネージャーの型.</typeparam>
        /// <param name="builder">コンテナビルダー.</param>
        /// <param name="parent">親のTransform.</param>
        static void RegisterManager<T>(IContainerBuilder builder, Transform parent) where T : Component
        {
            builder.RegisterComponentOnNewGameObject<T>(Lifetime.Singleton)
                .UnderTransform(parent)
                .DontDestroyOnLoad();
        }

        #endregion
    }
}
