using Project.Core;
using Project.Gameplay;
using UnityEngine;
using VContainer;
using Project.Services;
using VContainer.Unity;
using Project.UI;

namespace Project.Bootstrap
{
    public class GameLifetimeScope : LifetimeScope
    {
        [Header("Scene References")]
        [SerializeField] private CarMovement _carMovement;
        [SerializeField] private CarEntity _carEntity;
        [SerializeField] private TurretAiming _turretAiming;
        [SerializeField] private TurretWeapon _turretWeapon;
        [SerializeField] private EndlessRoadManager _endlessRoadManager;
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private PlayerDeathObserver _playerDeathObserver;
        [SerializeField] private LoseScreenUI _loseScreenUI;


        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<GameStateManager>(Lifetime.Singleton);

            builder.Register<SwipeInputService>(Lifetime.Singleton);

            builder.RegisterComponent(_carMovement);
            builder.RegisterComponent(_carEntity);
            builder.RegisterComponent(_turretAiming);
            builder.RegisterComponent(_turretWeapon);
            builder.RegisterComponent(_endlessRoadManager);
            builder.RegisterComponent(_enemySpawner);
            builder.RegisterComponent(_playerDeathObserver);
            builder.RegisterComponent(_loseScreenUI);

        }
    }
}