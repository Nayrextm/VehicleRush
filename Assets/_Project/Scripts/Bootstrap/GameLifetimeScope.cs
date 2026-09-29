using Project.Core;
using VContainer;
using VContainer.Unity;

namespace Project.Bootstrap
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<GameStateManager>(Lifetime.Singleton);
        }
    }
}