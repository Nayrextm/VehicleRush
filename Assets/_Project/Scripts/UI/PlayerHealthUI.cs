using DG.Tweening;
using Project.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Project.UI
{
    public class PlayerHealthUI : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Front health slider")]
        [SerializeField] private Image _foregroundFill;

        [Tooltip("Back health slider")]
        [SerializeField] private Image _damageTrailFill;

        [Header("Animation Settings")]
        [SerializeField] private float _trailDelay = 0.3f;
        [SerializeField] private float _trailDuration = 0.5f;
        [SerializeField] private float _shakeIntensity = 5f;

        private Health _playerHealth;
        private Vector3 _originalPosition;

        [Inject]
        public void Construct(CarEntity playerEntity)
        {
            if (playerEntity != null)
            {
                _playerHealth = playerEntity.GetComponent<Health>();
            }
        }

        private void Awake()
        {
            RectTransform rectTransform = transform as RectTransform;
            if (rectTransform != null)
            {
                _originalPosition = rectTransform.anchoredPosition;
            }
        }

        private void OnEnable()
        {
            if (_playerHealth != null)
            {
                _playerHealth.OnHealthChanged += HandleHealthChanged;

                _playerHealth.ResetHealth();
            }
        }

        private void OnDisable()
        {
            if (_playerHealth != null)
            {
                _playerHealth.OnHealthChanged -= HandleHealthChanged;
            }

#pragma warning disable CS4014
            _damageTrailFill?.DOKill();
            transform.DOKill();
#pragma warning restore CS4014
        }

        private void HandleHealthChanged(int currentHealth, int maxHealth)
        {
            if (maxHealth <= 0 || _foregroundFill == null || _damageTrailFill == null) return;

            float targetFillAmount = (float)currentHealth / maxHealth;

            _foregroundFill.fillAmount = targetFillAmount;

#pragma warning disable CS4014
            _damageTrailFill.DOKill();
            _damageTrailFill.DOFillAmount(targetFillAmount, _trailDuration)
                .SetDelay(_trailDelay)
                .SetEase(Ease.OutCubic)
                .SetLink(gameObject);

            if (currentHealth < maxHealth)
            {
                RectTransform rectTransform = transform as RectTransform;
                if (rectTransform != null)
                {
                    rectTransform.DOKill();
                    rectTransform.anchoredPosition = _originalPosition;
                    rectTransform.DOShakeAnchorPos(0.2f, _shakeIntensity, 20, 90f, false, true)
                        .SetLink(gameObject);
                }
            }
#pragma warning restore CS4014
        }
    }
}