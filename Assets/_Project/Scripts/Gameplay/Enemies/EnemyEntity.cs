using Project.Core;
using Project.Gameplay.VFX;
using UnityEngine;
using UnityEngine.Pool;

namespace Project.Gameplay.Enemies
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

        [Header("Dependencies")]
        [Tooltip("Animation controller. If it is empty, it will be found automatically.")]
        [SerializeField] private EnemyAnimationController _animationController;

        private IObjectPool<EnemyEntity> _pool;
        private GameStateManager _gameStateManager;
        private Transform _playerTransform;
        private Health _health;
        private VFXPoolService _vfxService;

        private EnemyState _currentState = EnemyState.Sleep;
        private Vector3 _wanderDirection;
        private float _wanderDirectionTimer;
        private float _stunTimer;

        private void Awake()
        {
            if (!TryGetComponent(out _health))
                Debug.LogError($"[EnemyEntity] Missing Health component on {gameObject.name}!");

            if (_animationController == null)
                _animationController = GetComponentInChildren<EnemyAnimationController>();
        }

        public void Init(IObjectPool<EnemyEntity> pool, Transform player, GameStateManager gameState, VFXPoolService vfxService)
        {
            _pool = pool;
            _playerTransform = player;
            _gameStateManager = gameState;
            _vfxService = vfxService;

            _wanderDirectionTimer = 0f;
            _stunTimer = 0f;

            if (_health != null)
                _health.ResetHealth();

            ChangeState(EnemyState.Sleep, force: true);
        }

        private void OnEnable()
        {
            if (_health != null)
            {
                _health.OnDied += HandleDeath;
                _health.OnDamaged += PlayHitReaction;
            }
        }

        private void OnDisable()
        {
            if (_health != null)
            {
                _health.OnDied -= HandleDeath;
                _health.OnDamaged -= PlayHitReaction;
            }
        }

        private void Update()
        {
            if (_gameStateManager == null || _gameStateManager.CurrentState != GameState.Playing || _playerTransform == null)
            {
                if (_animationController != null) _animationController.SetSpeed(0f);
                return;
            }

            if (_animationController != null) _animationController.SetSpeed(1f);

            if (transform.position.z < _playerTransform.position.z - 15f)
            {
                ReturnToPool();
                return;
            }

            if (_stunTimer > 0f)
            {
                _stunTimer -= Time.deltaTime;
                return;
            }

            Vector3 vectorToPlayer = _playerTransform.position - transform.position;
            vectorToPlayer.y = 0;
            float sqrDistance = vectorToPlayer.sqrMagnitude;

            CheckStateTransitions(sqrDistance);
            ExecuteStateLogic(vectorToPlayer.normalized);
        }

        private void CheckStateTransitions(float sqrDistance)
        {
            EnemyState nextState;

            if (sqrDistance > _wakeUpSqrDistance)
                nextState = EnemyState.Sleep;
            else if (sqrDistance > _chaseSqrDistance)
                nextState = EnemyState.Wander;
            else
                nextState = EnemyState.Chase;

            if (nextState != _currentState)
            {
                ChangeState(nextState);
            }
        }

        private void ChangeState(EnemyState newState, bool force = false)
        {
            if (!force && _currentState == newState) return;

            _currentState = newState;

            if (_animationController != null)
            {
                _animationController.SetRunningState(newState == EnemyState.Chase);
            }
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
                        transform.forward = Vector3.Slerp(transform.forward, directionToPlayer, Time.deltaTime * 10f);
                        transform.Translate(Vector3.forward * (_chaseSpeed * Time.deltaTime), Space.Self);
                    }
                    break;
            }
        }

        private void PlayHitReaction(int damageAmount)
        {
            if (_animationController != null)
                _animationController.PlayHitAnimation();

            _stunTimer = 0.2f;
        }

        public void Explode()
        {
            if (_vfxService != null)
                _vfxService.PlayDeathEffect(transform.position + Vector3.up);

            ReturnToPool();
        }

        private void HandleDeath()
        {
            Explode();
        }

        public void ReturnToPool()
        {
            if (gameObject.activeSelf && _pool != null)
                _pool.Release(this);
        }
    }
}