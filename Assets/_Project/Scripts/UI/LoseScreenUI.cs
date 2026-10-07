using Cysharp.Threading.Tasks;
using DG.Tweening;
using Project.Core;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Project.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class LoseScreenUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform _popupTransform;
        [SerializeField] private Button _tapToContinueButton;

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

            _tapToContinueButton.onClick.AddListener(OnTapContinue);
        }

        private void OnDisable()
        {
            if (_gameStateManager != null)
                _gameStateManager.OnStateChanged -= HandleStateChanged;

            _tapToContinueButton.onClick.RemoveListener(OnTapContinue);

            _showSequence?.Kill();
            _canvasGroup.DOKill();
            _popupTransform.DOKill();
        }

        private void HandleStateChanged(GameState previousState, GameState newState)
        {
            if (newState == GameState.Lose)
            {
                ShowPanelWithDelayAsync().Forget();
            }
            else if (newState == GameState.Menu)
            {
                HidePanel();
            }
        }

        private async UniTaskVoid ShowPanelWithDelayAsync()
        {
            var cancelToken = this.GetCancellationTokenOnDestroy();

            await UniTask.Delay(1500, cancellationToken: cancelToken);

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

        private void OnTapContinue()
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