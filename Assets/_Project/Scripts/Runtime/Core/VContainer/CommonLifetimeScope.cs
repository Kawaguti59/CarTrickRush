using UnityEngine;

using VContainer;
using VContainer.Unity;

namespace CarTrickRush.Runtime.Core.VContainer
{
    /// =========================================================================================
    /// <summary>
    /// 共通ライフタイムスコープ.
    /// </summary>
    /// =========================================================================================
    [DefaultExecutionOrder(-100)]
    public class CommonLifetimeScope : LifetimeScope
    {
        #region ------------------ Methods ------------------

        /// <summary>
        /// コンフィグレーションを設定します.
        /// </summary>
        /// <param name="builder">コンテナビルダー.</param>
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterAdditionalComponents(builder);

            // 参照関係が構築された後に、
            // シーンに存在する全てのオブジェクトにInject(注入)する
            builder.RegisterBuildCallback(resolver =>
            {
                foreach (var rootGameObject in gameObject.scene.GetRootGameObjects())
                {
                    resolver.InjectGameObject(rootGameObject);
                }
            });
        }

        /// <summary>
        /// シーン固有のコンポーネントを登録します.
        /// </summary>
        /// <param name="builder">コンテナビルダー.</param>
        protected virtual void RegisterAdditionalComponents(IContainerBuilder builder)
        {
        }

        #endregion
    }
}