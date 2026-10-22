using Cysharp.Threading.Tasks;
using Project.Core;
using UnityEngine;
using VContainer;

namespace Project.Gameplay.VFX
{
    public class CarEffectsController : MonoBehaviour
    {
        [Header("References: Exhaust")]
        [SerializeField] private ParticleSystem[] _exhaustParticles;

        [Header("References: Tire Tracks")]
        [SerializeField] private TrailRenderer[] _tireTrails;

        [Header("References: Engine Fire")]
        [Tooltip("Particles from under the hood")]
        [SerializeField] private ParticleSystem _engineFireParticles;
        [Tooltip("Delay before flash firing after camera shake (in milliseconds)")]
        [SerializeField] private int _fireDelayMs = 400;

        private GameStateManager _gameStateManager;

        [Inject]
        public void Construct(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
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
        }

        private void Start()
        {
            SetEffectsActive(false);
            ClearTrails();

            if (_engineFireParticles != null)
                _engineFireParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private void HandleStateChanged(GameState previousState, GameState newState)
        {
            if (newState == GameState.Playing)
            {
                ClearTrails();
                SetEffectsActive(true);

                if (_engineFireParticles != null)
                {
                    _engineFireParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
            }
            else if (newState == GameState.Lose)
            {
                SetEffectsActive(false);
                PlayFireWithDelayAsync().Forget();
            }
            else
            {
                SetEffectsActive(false);
                if (_engineFireParticles != null)
                {
                    _engineFireParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
            }
        }

        private async UniTaskVoid PlayFireWithDelayAsync()
        {
            var cancelToken = this.GetCancellationTokenOnDestroy();

            await UniTask.Delay(_fireDelayMs, cancellationToken: cancelToken);

            if (_engineFireParticles != null)
            {
                _engineFireParticles.Play();
            }
        }

        private void SetEffectsActive(bool isActive)
        {
            if (_exhaustParticles != null)
            {
                for (int i = 0; i < _exhaustParticles.Length; i++)
                {
                    if (_exhaustParticles[i] == null) continue;
                    if (isActive) _exhaustParticles[i].Play();
                    else _exhaustParticles[i].Stop();
                }
            }

            if (_tireTrails != null)
            {
                for (int i = 0; i < _tireTrails.Length; i++)
                {
                    if (_tireTrails[i] == null) continue;
                    _tireTrails[i].emitting = isActive;
                }
            }
        }

        private void ClearTrails()
        {
            if (_tireTrails == null) return;
            for (int i = 0; i < _tireTrails.Length; i++)
            {
                if (_tireTrails[i] != null) _tireTrails[i].Clear();
            }
        }
    }
}