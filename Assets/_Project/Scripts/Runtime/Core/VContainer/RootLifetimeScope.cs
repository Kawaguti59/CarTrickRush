using UnityEngine;

using VContainer;
using VContainer.Unity;

namespace CarTrickRush.Runtime.Core.VContainer
{
    /// =========================================================================================
    /// <summary>
    /// ルートライフタイムスコープ.
    /// </summary>
    /// =========================================================================================
    public sealed class RootLifetimeScope : LifetimeScope
    {
        #region ------------------ Methods ------------------

        /// <summary>
        /// コンフィグレーションを設定します.
        /// </summary>
        /// <param name="builder">コンテナビルダー.</param>
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterBuildCallback(resolver =>
            {
                foreach (Transform child in transform)
                {
                    resolver.InjectGameObject(child.gameObject);
                }
            });
        }

        #endregion
    }
}