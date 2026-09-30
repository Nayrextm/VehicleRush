using Project.Core;
using UnityEngine;
using VContainer;

namespace Project.Gameplay
{
    public class CarMovement : MonoBehaviour
    {
        [SerializeField] private float _speed = 5f;

        private GameStateManager _gameStateManager;

        [Inject]
        public void Construct(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }

        private void Update()
        {
            if (_gameStateManager == null || _gameStateManager.CurrentState != GameState.Playing)
                return;

            transform.Translate(Vector3.forward * (_speed * Time.deltaTime), Space.World);
        }
    }
}