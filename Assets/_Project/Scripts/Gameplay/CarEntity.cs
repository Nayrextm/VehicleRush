using Project.Core;
using UnityEngine;
using VContainer;

namespace Project.Gameplay
{
    [RequireComponent(typeof(Health))]
    public class CarEntity : MonoBehaviour
    {
        private Health _health;
        private GameStateManager _gameStateManager;

        [Inject]
        public void Construct(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }

        private void Awake()
        {
            _health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            _health.OnDied += HandleDeath;
        }

        private void OnDisable()
        {
            _health.OnDied -= HandleDeath;
        }

        private void HandleDeath()
        {
            _gameStateManager.ChangeState(GameState.Lose);
        }
    }
}