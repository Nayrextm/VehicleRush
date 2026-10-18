using DG.Tweening;
using Project.Core;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Project.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PauseScreenUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform _popupTransform;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _menuButton;

        private CanvasGroup _canvasGroup;
        private GameStateManager _gameStateManager;
        private Sequence _showSequence;

        [Inject]
        public void Construct(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            HidePanel();
        }

        private void OnEnable()
        {
            if (_gameStateManager != null)
                _gameStateManager.OnStateChanged += HandleStateChanged;

            _resumeButton.onClick.AddListener(OnResumeClicked);
            _menuButton.onClick.AddListener(OnMenuClicked);
        }

        private void OnDisable()
        {
            if (_gameStateManager != null)
                _gameStateManager.OnStateChanged -= HandleStateChanged;

            _resumeButton.onClick.RemoveListener(OnResumeClicked);
            _menuButton.onClick.RemoveListener(OnMenuClicked);

            _showSequence?.Kill();
            _canvasGroup.DOKill();
            if (_popupTransform != null) _popupTransform.DOKill();
        }

        private void HandleStateChanged(GameState previousState, GameState newState)
        {
            if (newState == GameState.Pause)
            {
                ShowPanel();
            }
            else if (previousState == GameState.Pause && (newState == GameState.Playing || newState == GameState.Menu))
            {
                HidePanel();
            }
        }

        private void ShowPanel()
        {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;

            _canvasGroup.alpha = 0f;
            if (_popupTransform != null)
            {
                _popupTransform.localScale = Vector3.zero;
            }

            _showSequence?.Kill();

            _showSequence = DOTween.Sequence();

#pragma warning disable CS4014
            _showSequence.SetUpdate(true);

            _showSequence.Append(_canvasGroup.DOFade(1f, 0.2f));

            if (_popupTransform != null)
            {
                _showSequence.Join(_popupTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack));
            }

            _showSequence.SetLink(gameObject);
#pragma warning restore CS4014
        }

        private void OnResumeClicked()
        {
            _gameStateManager.ChangeState(GameState.Playing);
        }

        private void OnMenuClicked()
        {
            _gameStateManager.ChangeState(GameState.Menu);
        }

        private void HidePanel()
        {
            _showSequence?.Kill();

            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            if (_popupTransform != null)
            {
                _popupTransform.localScale = Vector3.zero;
            }
        }
    }
}