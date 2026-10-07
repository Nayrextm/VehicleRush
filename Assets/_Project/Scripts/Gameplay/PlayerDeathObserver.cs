using Project.Core;
using UnityEngine;
using VContainer;

namespace Project.Gameplay
{
    [RequireComponent(typeof(Health))]
    public class PlayerDeathObserver : MonoBehaviour
    {
        private GameStateManager _gameStateManager;
        private Health _health;

        [Inject]
        public void Construct(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }

        private void Awake()
        {
            _health = GetComponent<Health>();
        }

        private void OnEnable() => _health.OnDied += HandlePlayerDeath;
        private void OnDisable() => _health.OnDied -= HandlePlayerDeath;

        private void HandlePlayerDeath()
        {
            if (_gameStateManager != null && _gameStateManager.CurrentState == GameState.Playing)
            {
                _gameStateManager.ChangeState(GameState.Lose);
            }
        }
    }
}