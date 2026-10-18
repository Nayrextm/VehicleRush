using Project.Core;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Project.UI
{
    [RequireComponent(typeof(Button))]
    public class PauseButtonUI : MonoBehaviour
    {
        private Button _button;
        private GameStateManager _gameStateManager;

        [Inject]
        public void Construct(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable() => _button.onClick.AddListener(OnClick);
        private void OnDisable() => _button.onClick.RemoveListener(OnClick);

        private void OnClick()
        {
            if (_gameStateManager != null && _gameStateManager.CurrentState == GameState.Playing)
            {
                _gameStateManager.ChangeState(GameState.Pause);
            }
        }
    }
}