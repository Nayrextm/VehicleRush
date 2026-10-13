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
        [SerializeField] private PlayerHealthUI _playerHealthUI;
        [SerializeField] private DistanceTracker _distanceTracker;
        [SerializeField] private DistanceProgressUI _distanceProgressUI;
        [SerializeField] private GameplayHUDController _gameplayHUDController;
        [SerializeField] private CameraTransitionController _cameraTransitionController;
        [SerializeField] private MainMenuUI _mainMenuUI;

        [Header("Prefabs Configuration")]
        [SerializeField] private PooledVFX _deathEffectPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<GameStateManager>(Lifetime.Singleton);
            builder.Register<SwipeInputService>(Lifetime.Singleton);

            var vfxPoolService = new VFXPoolService(_deathEffectPrefab, 15);
            vfxPoolService.PreWarm(15);

            builder.RegisterInstance(vfxPoolService);

            builder.RegisterComponent(_carMovement);
            builder.RegisterComponent(_carEntity);
            builder.RegisterComponent(_turretAiming);
            builder.RegisterComponent(_turretWeapon);
            builder.RegisterComponent(_endlessRoadManager);
            builder.RegisterComponent(_enemySpawner);
            builder.RegisterComponent(_playerDeathObserver);
            builder.RegisterComponent(_loseScreenUI);
            builder.RegisterComponent(_playerHealthUI);
            builder.RegisterComponent(_distanceTracker);
            builder.RegisterComponent(_distanceProgressUI);
            builder.RegisterComponent(_gameplayHUDController);
            builder.RegisterComponent(_cameraTransitionController);
            builder.RegisterComponent(_mainMenuUI);
        }
    }
}