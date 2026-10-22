using Project.Core;
using UnityEngine;
using VContainer;

namespace Project.Gameplay
{
    [RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
    public class PhysicsFloorFollower : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Floor height Y (ground level)")]
        [SerializeField] private float _yOffset = 0f;

        [Tooltip("Ground Width X")]
        [SerializeField] private float _trackWidth = 40f;

        [Tooltip("Lenght of the floor Z")]
        [SerializeField] private float _floorLength = 150f;

        [Tooltip("Floor displacement step in meters")]
        [SerializeField] private float _moveStep = 75f;

        private Rigidbody _rb;
        private BoxCollider _boxCollider;
        private GameStateManager _gameStateManager;
        private Transform _playerTransform;

        private float _lastAnchorZ;

        [Inject]
        public void Construct(GameStateManager gameStateManager, CarEntity playerEntity)
        {
            _gameStateManager = gameStateManager;
            if (playerEntity != null)
            {
                _playerTransform = playerEntity.transform;
            }
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _boxCollider = GetComponent<BoxCollider>();

            _rb.isKinematic = true;
            _rb.useGravity = false;

            _boxCollider.size = new Vector3(_trackWidth, 1f, _floorLength);
            _boxCollider.center = new Vector3(0f, -0.5f, 0f);
        }

        private void Start()
        {
            _lastAnchorZ = 0f;
            transform.position = new Vector3(0f, _yOffset, 0f);
        }

        private void FixedUpdate()
        {
            if (_gameStateManager == null || _gameStateManager.CurrentState != GameState.Playing || _playerTransform == null)
                return;

            float playerZ = _playerTransform.position.z;

            if (playerZ - _lastAnchorZ >= _moveStep)
            {
                _lastAnchorZ = playerZ;
                Vector3 targetPosition = new Vector3(0f, _yOffset, playerZ);

                _rb.MovePosition(targetPosition);
            }
        }
    }
}