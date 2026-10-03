using Project.Core;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

namespace Project.Gameplay
{
    public class TurretWeapon : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private Transform _firePoint;

        [Header("Combat Settings")]
        [SerializeField] private float _fireRate = 0.2f;
        [SerializeField] private float _bulletSpeed = 40f;
        [SerializeField] private float _bulletLifetime = 2f;

        [Header("Pool Settings")]
        [SerializeField] private int _defaultCapacity = 20;
        [SerializeField] private int _maxSize = 50;

        private IObjectPool<Projectile> _bulletPool;
        private GameStateManager _gameStateManager;
        private float _nextFireTime;

        [Inject]
        public void Construct(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }

        private void Awake()
        {
            _bulletPool = new ObjectPool<Projectile>(
                createFunc: CreateProjectile,
                actionOnGet: OnTakeFromPool,
                actionOnRelease: OnReturnedToPool,
                actionOnDestroy: OnDestroyPoolObject,
                collectionCheck: false,
                defaultCapacity: _defaultCapacity,
                maxSize: _maxSize
            );
        }

        private void Start()
        {
            Projectile[] preWarmedBullets = new Projectile[_defaultCapacity];

            for (int i = 0; i < _defaultCapacity; i++)
            {
                preWarmedBullets[i] = _bulletPool.Get();
            }

            for (int i = 0; i < _defaultCapacity; i++)
            {
                _bulletPool.Release(preWarmedBullets[i]);
            }
        }

        private void Update()
        {
            if (_gameStateManager == null || _gameStateManager.CurrentState != GameState.Playing)
                return;

            if (Time.time >= _nextFireTime)
            {
                Shoot();
                _nextFireTime = Time.time + _fireRate;
            }
        }

        private void Shoot()
        {
            if (_bulletPool != null)
            {
                _bulletPool.Get();
            }
        }

        #region Object Pool Callbacks

        private Projectile CreateProjectile()
        {
            return Instantiate(_projectilePrefab);
        }

        private void OnTakeFromPool(Projectile projectile)
        {
            projectile.transform.position = _firePoint.position;
            projectile.transform.rotation = _firePoint.rotation;
            projectile.gameObject.SetActive(true);

            projectile.Init(_bulletPool, _bulletSpeed, _bulletLifetime);
        }

        private void OnReturnedToPool(Projectile projectile)
        {
            projectile.gameObject.SetActive(false);
        }

        private void OnDestroyPoolObject(Projectile projectile)
        {
            Destroy(projectile.gameObject);
        }

        #endregion
    }
}