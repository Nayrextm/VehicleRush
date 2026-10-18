using DG.Tweening;
using Project.Core;
using Project.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Project.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Controls")]
        [SerializeField] private Button _playButton;
        [SerializeField] private float _fadeDuration = 0.3f;

        [Header("Stats Display")]
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _totalScoreText;

        private CanvasGroup _canvasGroup;
        private GameStateManager _gameStateManager;
        private LevelProgressionManager _progressionManager;

        [Inject]
        public void Construct(GameStateManager gameStateManager, LevelProgressionManager progressionManager)
        {
            _gameStateManager = gameStateManager;
            _progressionManager = progressionManager;
        }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            UpdateStatsText();
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
                UpdateStatsText();

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

        private void UpdateStatsText()
        {
            if (_progressionManager == null) return;

            if (_levelText != null)
            {
                _levelText.SetText("{0}", _progressionManager.CurrentLevel);
            }

            if (_totalScoreText != null)
            {
                _totalScoreText.SetText("{0}m", _progressionManager.DisplayScore);
            }
        }
    }
}