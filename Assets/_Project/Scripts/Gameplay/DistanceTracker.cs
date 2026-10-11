using Project.Core;
using UnityEngine;
using VContainer;

namespace Project.Gameplay
{
    public class DistanceTracker : MonoBehaviour
    {
        private GameStateManager _gameStateManager;
        private Transform _playerTransform;

        private float _initialPlayerZ;

        public int CurrentDistance { get; private set; }

        [Inject]
        public void Construct(GameStateManager gameStateManager, CarEntity playerEntity)
        {
            _gameStateManager = gameStateManager;
            if (playerEntity != null)
            {
                _playerTransform = playerEntity.transform;
            }
        }

        private void Start()
        {
            if (_playerTransform != null)
            {
                _initialPlayerZ = _playerTransform.position.z;
            }
        }

        private void Update()
        {
            if (_gameStateManager == null || _gameStateManager.CurrentState != GameState.Playing || _playerTransform == null)
                return;

            CurrentDistance = Mathf.Max(0, Mathf.FloorToInt(_playerTransform.position.z - _initialPlayerZ));
        }
    }
}