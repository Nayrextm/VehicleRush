using DG.Tweening;
using Project.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Project.UI
{
    public class DistanceProgressUI : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Background Bar Container")]
        [SerializeField] private RectTransform _barContainer;

        [Tooltip("Fill Bar (Image Type must be Filled!)")]
        [SerializeField] private Image _fillImage;

        [Tooltip("Placeholder container for icon(Indicator)")]
        [SerializeField] private RectTransform _indicatorContainer;

        [Tooltip("Distance text")]
        [SerializeField] private TextMeshProUGUI _distanceText;

        [Header("Animation Settings")]
        [SerializeField] private float _animationDuration = 0.15f;

        private LevelProgressionManager _progressionManager;
        private int _lastDistance = -1;

        [Inject]
        public void Construct(LevelProgressionManager progressionManager)
        {
            _progressionManager = progressionManager;
        }

        private void OnDisable()
        {
#pragma warning disable CS4014
            _fillImage?.DOKill();
            _indicatorContainer?.DOKill();
#pragma warning restore CS4014
        }

        private void Update()
        {
            if (_progressionManager == null) return;

            int currentTotal = _progressionManager.CurrentTotalDistance;

            if (currentTotal != _lastDistance)
            {
                _lastDistance = currentTotal;
                UpdateProgress(currentTotal);
            }
        }

        private void UpdateProgress(int totalDistance)
        {
            if (_distanceText != null)
            {
                _distanceText.SetText("{ 0}m", totalDistance);
            }

            int levelStart = _progressionManager.CurrentLevelStart;
            int step = _progressionManager.LevelStep;
            float progress = Mathf.Clamp01((float)(totalDistance - levelStart) / step);

#pragma warning disable CS4014
            _fillImage.DOKill();
            _indicatorContainer.DOKill();

            _fillImage.DOFillAmount(progress, _animationDuration)
                .SetEase(Ease.OutSine)
                .SetLink(gameObject);

            float totalBarHeight = _barContainer.rect.height;
            float targetY = progress * totalBarHeight;

            _indicatorContainer.DOAnchorPosY(targetY, _animationDuration)
                .SetEase(Ease.OutSine)
                .SetLink(gameObject);
#pragma warning restore CS4014
        }
    }
}