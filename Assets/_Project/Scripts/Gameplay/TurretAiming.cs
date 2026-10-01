using Project.Core;
using Project.Services;
using UnityEngine;
using VContainer;

namespace Project.Gameplay
{
    public class TurretAiming : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Object to be rotated (barrel/turret)")]
        [SerializeField] private Transform _turretHead;

        [Header("Settings")]
        [SerializeField] private float _rotationSpeed = 0.2f;
        [SerializeField] private float _maxAngle = 60f;

        private SwipeInputService _inputService;
        private GameStateManager _gameStateManager;
        private float _currentAngle = 0f;

        [Inject]
        public void Construct(SwipeInputService inputService, GameStateManager gameStateManager)
        {
            _inputService = inputService;
            _gameStateManager = gameStateManager;
        }

        private void Update()
        {
            if (_gameStateManager == null || _gameStateManager.CurrentState != GameState.Playing)
                return;

            float swipeDelta = _inputService.GetHorizontalDelta();

            if (Mathf.Abs(swipeDelta) > 0.01f)
            {
                _currentAngle += swipeDelta * _rotationSpeed;
                _currentAngle = Mathf.Clamp(_currentAngle, -_maxAngle, _maxAngle);

                _turretHead.localRotation = Quaternion.Euler(0, _currentAngle, 0);
            }
        }
    }
}