using Cysharp.Threading.Tasks;
using DG.Tweening;
using Project.Core;
using UnityEngine;
using VContainer;

namespace Project.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class TutorialSwipeUI : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Crosshair image for swipe")]
        [SerializeField] private RectTransform _crosshairTransform;

        [Header("Animation Settings")]
        [Tooltip("Travel distance in one direction along the X-axis (pixels Canvas)")]
        [SerializeField] private float _moveDistance = 150f;
        [Tooltip("Duration of a single movement (swipe) in seconds")]
        [SerializeField] private float _swipeDuration = 0.8f;

        [Tooltip("Panel Duration before fading")]
        [SerializeField] private int _displayDurationMs = 2500;
        [SerializeField] private float _fadeDuration = 0.3f;

        private CanvasGroup _canvasGroup;
        private GameStateManager _gameStateManager;
        private Tween _moveTween;

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

            KillTweens();
        }

        private void HandleStateChanged(GameState previousState, GameState newState)
        {
            if (newState == GameState.Playing && previousState == GameState.Menu)
            {
                ShowTutorialAsync().Forget();
            }
            else if (newState != GameState.Playing)
            {
                HideInstant();
            }
        }

        private async UniTaskVoid ShowTutorialAsync()
        {
            KillTweens();

            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;

            if (_crosshairTransform != null)
            {
                _crosshairTransform.anchoredPosition = new Vector2(-_moveDistance, 0f);

                _moveTween = _crosshairTransform.DOAnchorPosX(_moveDistance, _swipeDuration)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetLink(gameObject);
            }

            _canvasGroup.alpha = 0f;
            _ = _canvasGroup.DOFade(1f, _fadeDuration).SetLink(gameObject);

            bool canceled = await UniTask.Delay(_displayDurationMs, cancellationToken: this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();
            if (canceled) return;

            if (_gameStateManager != null && _gameStateManager.CurrentState == GameState.Playing)
            {
                _ = _canvasGroup.DOFade(0f, _fadeDuration)
                    .OnComplete(KillTweens)
                    .SetLink(gameObject);
            }
        }

        private void HideInstant()
        {
            KillTweens();
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.interactable = false;
            }
        }

        private void KillTweens()
        {
            _canvasGroup?.DOKill();
            _moveTween?.Kill();
            if (_crosshairTransform != null) _crosshairTransform.DOKill();
        }
    }
}