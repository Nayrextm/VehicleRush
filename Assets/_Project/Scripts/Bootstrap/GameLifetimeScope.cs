using Project.Core;
using Project.Gameplay;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Project.Bootstrap
{
    public class GameLifetimeScope : LifetimeScope
    {
        [Header("Scene References")]
        [SerializeField] private CarMovement _carMovement;
        [SerializeField] private CarEntity _carEntity;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<GameStateManager>(Lifetime.Singleton);

            builder.RegisterComponent(_carMovement);
            builder.RegisterComponent(_carEntity);
        }
    }
}