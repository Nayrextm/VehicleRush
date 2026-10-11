using DG.Tweening;
using Project.Core;
using UnityEngine;
using VContainer;

namespace Project.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class GameplayHUDController : MonoBehaviour
    {
        [Header("Settings")]
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

            HideInstant();
        }

        private void OnEnable()
        {
            if (_gameStateManager != null)
                _gameStateManager.OnStateChanged += HandleStateChanged;
        }

        private void OnDisable()
        {
            if (_gameStateManager != null)
                _gameStateManager.OnStateChanged -= HandleStateChanged;

#pragma warning disable CS4014
            _canvasGroup?.DOKill();
#pragma warning restore CS4014
        }

        private void HandleStateChanged(GameState previousState, GameState newState)
        {
#pragma warning disable CS4014
            _canvasGroup.DOKill();

            if (newState == GameState.Playing)
            {
                _canvasGroup.blocksRaycasts = true;
                _canvasGroup.interactable = true;

                _canvasGroup.DOFade(1f, _fadeDuration)
                    .SetEase(Ease.OutSine)
                    .SetLink(gameObject);
            }
            else
            {
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.interactable = false;

                _canvasGroup.DOFade(0f, _fadeDuration)
                    .SetEase(Ease.InSine)
                    .SetLink(gameObject);
            }
#pragma warning restore CS4014
        }

        private void HideInstant()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }
    }
}