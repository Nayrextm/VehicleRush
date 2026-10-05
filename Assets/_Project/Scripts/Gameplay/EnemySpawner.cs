using Project.Core;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

namespace Project.Gameplay
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EnemyEntity _enemyPrefab;

        [Header("Spawn Settings")]
        [SerializeField] private float _spawnZOffset = 120f;
        [SerializeField] private float _spawnInterval = 4f;
        [SerializeField] private int _clusterSizeMin = 3;
        [SerializeField] private int _clusterSizeMax = 6;
        [SerializeField] private float _spawnWidth = 7f;

        [Header("Pool Settings")]
        [SerializeField] private int _defaultCapacity = 30;
        [SerializeField] private int _maxPoolSize = 60;

        private IObjectPool<EnemyEntity> _enemyPool;
        private GameStateManager _gameStateManager;
        private CarEntity _playerEntity;
        private Transform _playerTransform;
        private float _nextSpawnTime;

        [Inject]
        public void Construct(GameStateManager gameStateManager, CarEntity playerEntity)
        {
            _gameStateManager = gameStateManager;
            _playerEntity = playerEntity;
        }

        private void Awake()
        {
            _enemyPool = new ObjectPool<EnemyEntity>(
                createFunc: () => Instantiate(_enemyPrefab, transform),
                actionOnGet: enemy => enemy.gameObject.SetActive(true),
                actionOnRelease: enemy => enemy.gameObject.SetActive(false),
                actionOnDestroy: enemy => Destroy(enemy.gameObject),
                collectionCheck: false,
                defaultCapacity: _defaultCapacity,
                maxSize: _maxPoolSize
            );
        }

        private void Start()
        {
            if (_playerEntity != null)
            {
                _playerTransform = _playerEntity.transform;
            }

            EnemyEntity[] preWarm = new EnemyEntity[_defaultCapacity];
            for (int i = 0; i < _defaultCapacity; i++) preWarm[i] = _enemyPool.Get();
            for (int i = 0; i < _defaultCapacity; i++) _enemyPool.Release(preWarm[i]);
        }

        private void Update()
        {
            if (_gameStateManager == null || _gameStateManager.CurrentState != GameState.Playing || _playerTransform == null)
                return;

            if (Time.time >= _nextSpawnTime)
            {
                SpawnCluster();
                _nextSpawnTime = Time.time + _spawnInterval;
            }
        }

        private void SpawnCluster()
        {
            if (_enemyPool == null || _playerTransform == null) return;

            int clusterSize = Random.Range(_clusterSizeMin, _clusterSizeMax + 1);
            float baseZ = _playerTransform.position.z + _spawnZOffset;

            for (int i = 0; i < clusterSize; i++)
            {
                EnemyEntity enemy = _enemyPool.Get();

                float randomX = Random.Range(-_spawnWidth, _spawnWidth);
                float randomZ = baseZ + Random.Range(-4f, 4f);

                enemy.transform.position = new Vector3(randomX, 0f, randomZ);

                enemy.Init(_enemyPool, _playerTransform, _gameStateManager);
            }
        }
    }
}