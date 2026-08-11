using UnityEngine;

using VContainer;
using VContainer.Unity;

using CarTrickRush.GameScene;
using CarTrickRush.UI;

namespace CarTrickRush.Runtime.Core.VContainer
{
    /// =========================================================================================
    /// <summary>
    /// GameScene 用ライフタイムスコープ.
    /// </summary>
    /// =========================================================================================
    [DefaultExecutionOrder(-100)]
    public sealed class GameLifetimeScope : CommonLifetimeScope
    {
        #region ------------------ Methods ------------------

        /// <inheritdoc />
        protected override void RegisterAdditionalComponents(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<MainProcess>();
            builder.RegisterComponentInHierarchy<GameUIPresenter>();
        }

        #endregion
    }
}
