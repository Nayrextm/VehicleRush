using Project.Core;
using UnityEngine;
using VContainer;

namespace Project.Gameplay.VFX
{
    public class CarEffectsController : MonoBehaviour
    {
        [Header("References: Exhaust")]
        [Tooltip("Exhaust particles (there may be several pipes)")]
        [SerializeField] private ParticleSystem[] _exhaustParticles;

        [Header("References: Tire Tracks")]
        [Tooltip("Wheel(Tire) tracks")]
        [SerializeField] private TrailRenderer[] _tireTrails;

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
        }

        private void HandleStateChanged(GameState previousState, GameState newState)
        {
            if (newState == GameState.Playing)
            {
                ClearTrails();
                SetEffectsActive(true);
            }
            else
            {
                SetEffectsActive(false);
            }
        }

        private void SetEffectsActive(bool isActive)
        {
            if (_exhaustParticles != null)
            {
#pragma warning disable CS4014
                for (int i = 0; i < _exhaustParticles.Length; i++)
                {
                    if (_exhaustParticles[i] == null) continue;

                    if (isActive) _exhaustParticles[i].Play();
                    else _exhaustParticles[i].Stop(); 
                }
#pragma warning restore CS4014
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