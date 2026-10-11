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

        [Tooltip("Fill Bar")]
        [SerializeField] private Image _fillImage;

        [Tooltip("Placeholder container for icon(Indicator)")]
        [SerializeField] private RectTransform _indicatorContainer;

        [Tooltip("Distance text")]
        [SerializeField] private TextMeshProUGUI _distanceText;

        [Header("Settings")]
        [Tooltip("Distance at which the gauge fills to 100% (target in meters)")]
        [SerializeField] private float _targetDistance = 1000f;
        [SerializeField] private float _animationDuration = 0.15f;

        private DistanceTracker _distanceTracker;
        private int _lastDistance = -1;

        [Inject]
        public void Construct(DistanceTracker distanceTracker)
        {
            _distanceTracker = distanceTracker;
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
            if (_distanceTracker == null) return;

            int currentDistance = _distanceTracker.CurrentDistance;

            if (currentDistance != _lastDistance)
            {
                _lastDistance = currentDistance;
                UpdateProgress(currentDistance);
            }
        }

        private void UpdateProgress(int distance)
        {
            if (_distanceText != null)
            {
                _distanceText.SetText("{0}m", distance);
            }

            float progress = Mathf.Clamp01(distance / _targetDistance);

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