using DG.Tweening;
using Project.Core;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Project.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private float _fadeDuration = 0.3f;

        private CanvasGroup _canvasGroup;
        private GameStateManager _gameStateManager;

        [Inject]
        public void Construct(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            if (_playButton != null) _playButton.onClick.AddListener(OnPlayClicked);

            if (_gameStateManager != null)
                _gameStateManager.OnStateChanged += HandleStateChanged;
        }

        private void OnDisable()
        {
            if (_playButton != null) _playButton.onClick.RemoveListener(OnPlayClicked);

            if (_gameStateManager != null)
                _gameStateManager.OnStateChanged -= HandleStateChanged;

#pragma warning disable CS4014
            _canvasGroup?.DOKill();
#pragma warning restore CS4014
        }

        private void OnPlayClicked()
        {
            if (_gameStateManager != null && _gameStateManager.CurrentState == GameState.Menu)
            {
                _gameStateManager.ChangeState(GameState.Playing);
            }
        }

        private void HandleStateChanged(GameState previousState, GameState newState)
        {
#pragma warning disable CS4014
            _canvasGroup.DOKill();

            if (newState == GameState.Menu)
            {
                _canvasGroup.blocksRaycasts = true;
                _canvasGroup.interactable = true;
                _canvasGroup.DOFade(1f, _fadeDuration).SetEase(Ease.OutSine).SetLink(gameObject);
            }
            else
            {
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.interactable = false;
                _canvasGroup.DOFade(0f, _fadeDuration).SetEase(Ease.InSine).SetLink(gameObject);
            }
#pragma warning restore CS4014
        }
    }
}
