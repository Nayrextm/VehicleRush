using DG.Tweening;
using Project.Core;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

namespace Project.Gameplay.VFX
{
    [RequireComponent(typeof(CinemachineCamera), typeof(CinemachineBasicMultiChannelPerlin))]
    public class CameraShakeController : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Intensity of camera shaking during a car crash")]
        [SerializeField] private float _shakeIntensity = 5f;
        [Tooltip("Duration of shaking decay")]
        [SerializeField] private float _shakeDuration = 0.5f;

        private CinemachineBasicMultiChannelPerlin _noiseComponent;
        private GameStateManager _gameStateManager;

        [Inject]
        public void Construct(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }

        private void Awake()
        {
            _noiseComponent = GetComponent<CinemachineBasicMultiChannelPerlin>();

            if (_noiseComponent != null)
                _noiseComponent.AmplitudeGain = 0f;
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

            DOTween.Kill(this);
        }

        private void HandleStateChanged(GameState previousState, GameState newState)
        {
            if (newState == GameState.Lose)
            {
                PlayShakeImpact();
            }
        }

        private void PlayShakeImpact()
        {
            if (_noiseComponent == null) return;

            DOTween.Kill(this);

            _noiseComponent.AmplitudeGain = _shakeIntensity;

            DOTween.To(
                () => _noiseComponent.AmplitudeGain,
                (float x) => _noiseComponent.AmplitudeGain = x,
                0f,
                _shakeDuration
            )
            .SetEase(Ease.OutCubic)
            .SetTarget(this)
            .SetLink(gameObject);
        }
    }
}