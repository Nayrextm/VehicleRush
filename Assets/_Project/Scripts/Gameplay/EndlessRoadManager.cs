using System.Collections.Generic;
using Project.Core;
using UnityEngine;
using VContainer;

namespace Project.Gameplay
{
    public class EndlessRoadManager : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Road Prefab")]
        [SerializeField] private GameObject _roadPrefab;

        [Header("Settings")]
        [Tooltip("Number of segments on the stage (5-7 good enough)")]
        [SerializeField] private int _segmentsCount = 5;

        [Tooltip("The EXACT length of road model along the axis Z")]
        [SerializeField] private float _segmentLength = 30f;

        private GameStateManager _gameStateManager;
        private CarEntity _playerEntity;
        private Transform _playerTransform;

        private readonly Queue<Transform> _activeSegments = new Queue<Transform>();
        private float _spawnZ = 0f;

        [Inject]
        public void Construct(GameStateManager gameStateManager, CarEntity playerEntity)
        {
            _gameStateManager = gameStateManager;
            _playerEntity = playerEntity;
        }

        private void Start()
        {
            if (_playerEntity != null)
            {
                _playerTransform = _playerEntity.transform;
            }

            _spawnZ = -_segmentLength;

            for (int i = 0; i < _segmentsCount; i++)
            {
                SpawnSegment();
            }
        }

        private void Update()
        {
            if (_gameStateManager == null || _gameStateManager.CurrentState != GameState.Playing || _playerTransform == null)
                return;

            if (_playerTransform.position.z > _activeSegments.Peek().position.z + _segmentLength)
            {
                RelocateOldestSegment();
            }
        }

        private void SpawnSegment()
        {
            GameObject segment = Instantiate(_roadPrefab, Vector3.forward * _spawnZ, Quaternion.identity, transform);
            _activeSegments.Enqueue(segment.transform);

            _spawnZ += _segmentLength;
        }

        private void RelocateOldestSegment()
        {
            if (_activeSegments.Count == 0) return;

            Transform oldestSegment = _activeSegments.Dequeue();

            oldestSegment.position = Vector3.forward * _spawnZ;
            _spawnZ += _segmentLength;

            _activeSegments.Enqueue(oldestSegment);
        }
    }
}