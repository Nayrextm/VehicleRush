using Cysharp.Threading.Tasks;
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
    public class WinScreenUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform _popupTransform;
        [SerializeField] private TextMeshProUGUI _finalScoreText;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _menuButton;

        private CanvasGroup _canvasGroup;
        private GameStateManager _gameStateManager;
        private LevelProgressionManager _progressionManager;
        private DistanceTracker _distanceTracker; 
        private Sequence _showSequence;

        [Inject]
        public void Construct(
            GameStateManager gameStateManager,
            LevelProgressionManager progressionManager,
            DistanceTracker distanceTracker)
        {
            _gameStateManager = gameStateManager;
            _progressionManager = progressionManager;
            _distanceTracker = distanceTracker;
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

            _continueButton.onClick.AddListener(OnContinueClicked);
            _menuButton.onClick.AddListener(OnMenuClicked);
        }

        private void OnDisable()
        {
            if (_gameStateManager != null)
                _gameStateManager.OnStateChanged -= HandleStateChanged;

            _continueButton.onClick.RemoveListener(OnContinueClicked);
            _menuButton.onClick.RemoveListener(OnMenuClicked);

            _showSequence?.Kill();
            _canvasGroup.DOKill();
            _popupTransform.DOKill();
        }

        private void HandleStateChanged(GameState previousState, GameState newState)
        {
            if (newState == GameState.Win)
            {
                if (_finalScoreText != null && _distanceTracker != null)
                {
                    _finalScoreText.SetText("\u00A0{0}m", _distanceTracker.RunDistance);
                }
                ShowPanelWithDelayAsync().Forget();
            }
            else if (newState == GameState.Playing || newState == GameState.Menu)
            {
                HidePanel();
            }
        }

        private async UniTaskVoid ShowPanelWithDelayAsync()
        {
            var cancelToken = this.GetCancellationTokenOnDestroy();

            await UniTask.Delay(1000, cancellationToken: cancelToken);

            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;

            _showSequence?.Kill();
            _showSequence = DOTween.Sequence();

#pragma warning disable CS4014 
            _showSequence.Append(_canvasGroup.DOFade(1f, 0.3f));
            _showSequence.Join(_popupTransform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack));
            _showSequence.SetLink(gameObject);
#pragma warning restore CS4014 

            _showSequence.ToUniTask(cancellationToken: cancelToken).Forget();
        }

        private void OnContinueClicked()
        {
            _gameStateManager.ChangeState(GameState.Playing);
        }

        private void OnMenuClicked()
        {
            _gameStateManager.ChangeState(GameState.Menu);
        }

        private void HidePanel()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            if (_popupTransform != null)
            {
                _popupTransform.localScale = Vector3.zero;
            }
        }
        private void OnDestroy()
        {
            _showSequence?.Kill();
        }
    }
}