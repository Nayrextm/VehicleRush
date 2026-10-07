using Project.Core;
using UnityEngine;
using UnityEngine.Pool;

namespace Project.Gameplay
{
    public class EnemyEntity : MonoBehaviour
    {
        private enum EnemyState { Sleep, Wander, Chase }

        [Header("Settings")]
        [SerializeField] private float _wanderSpeed = 1.5f;
        [SerializeField] private float _chaseSpeed = 6f;

        [Tooltip("Squares of distances. 10000 = 100ì, 1600 = 40ì")]
        [SerializeField] private float _wakeUpSqrDistance = 10000f;
        [SerializeField] private float _chaseSqrDistance = 1600f;

        private IObjectPool<EnemyEntity> _pool;
        private GameStateManager _gameStateManager;
        private Transform _playerTransform;
        private Health _health;
        private EnemyState _currentState = EnemyState.Sleep;

        private Vector3 _wanderDirection;
        private float _wanderDirectionTimer;

        private void Awake()
        {
            if (!TryGetComponent(out _health))
            {
                Debug.LogError($"[EnemyEntity] Missing Health component on {gameObject.name}!");
            }
        }

        public void Init(IObjectPool<EnemyEntity> pool, Transform player, GameStateManager gameState)
        {
            _pool = pool;
            _playerTransform = player;
            _gameStateManager = gameState;
            _currentState = EnemyState.Sleep;
            _wanderDirectionTimer = 0f;

            if (_health != null)
            {
                _health.ResetHealth();
            }
        }

        private void OnEnable()
        {
            if (_health != null) _health.OnDied += HandleDeath;
        }

        private void OnDisable()
        {
            if (_health != null) _health.OnDied -= HandleDeath;
        }

        private void Update()
        {
            if (_gameStateManager == null || _gameStateManager.CurrentState != GameState.Playing || _playerTransform == null)
                return;

            if (transform.position.z < _playerTransform.position.z - 15f)
            {
                ReturnToPool();
                return;
            }

            Vector3 vectorToPlayer = _playerTransform.position - transform.position;
            vectorToPlayer.y = 0;
            float sqrDistance = vectorToPlayer.sqrMagnitude;

            UpdateState(sqrDistance);
            ExecuteStateLogic(vectorToPlayer.normalized);
        }

        private void UpdateState(float sqrDistance)
        {
            if (sqrDistance > _wakeUpSqrDistance)
                _currentState = EnemyState.Sleep;
            else if (sqrDistance <= _wakeUpSqrDistance && sqrDistance > _chaseSqrDistance)
                _currentState = EnemyState.Wander;
            else
                _currentState = EnemyState.Chase;
        }

        private void ExecuteStateLogic(Vector3 directionToPlayer)
        {
            switch (_currentState)
            {
                case EnemyState.Sleep:
                    break;

                case EnemyState.Wander:
                    _wanderDirectionTimer -= Time.deltaTime;
                    if (_wanderDirectionTimer <= 0f)
                    {
                        Vector2 randomCircle = Random.insideUnitCircle.normalized;
                        _wanderDirection = new Vector3(randomCircle.x, 0f, randomCircle.y);
                        _wanderDirectionTimer = Random.Range(1.5f, 3f);
                    }

                    transform.forward = Vector3.Slerp(transform.forward, _wanderDirection, Time.deltaTime * 5f);
                    transform.Translate(Vector3.forward * (_wanderSpeed * Time.deltaTime), Space.Self);
                    break;

                case EnemyState.Chase:
                    if (directionToPlayer != Vector3.zero)
                    {
                        transform.forward = directionToPlayer;
                        transform.Translate(Vector3.forward * (_chaseSpeed * Time.deltaTime), Space.Self);
                    }
                    break;
            }
        }

        private void HandleDeath()
        {
            ReturnToPool();
        }

        public void ReturnToPool()
        {
            if (gameObject.activeSelf && _pool != null)
                _pool.Release(this);
        }
    }
}