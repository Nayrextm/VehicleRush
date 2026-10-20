using DG.Tweening;
using Project.Core;
using UnityEngine;
using VContainer;

namespace Project.Gameplay.VFX
{
    public class WheelRotator : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Wheels Array (Mesh objects)")]
        [SerializeField] private Transform[] _wheels;

        [Header("Settings")]
        [Tooltip("How many seconds does one full 360 - degree rotation take. less = faster.")]
        [SerializeField] private float _spinDuration = 0.2f;

        [Tooltip("Rotation Axis. X (360, 0, 0)")]
        [SerializeField] private Vector3 _rotationAxis = new Vector3(360f, 0f, 0f);

        private GameStateManager _gameStateManager;
        private Tween[] _wheelTweens;

        [Inject]
        public void Construct(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }

        private void OnEnable()
        {
            if (_gameStateManager != null)
                _gameStateManager.OnStateChanged += HandleStateChanged;

            InitTweens();
        }

        private void OnDisable()
        {
            if (_gameStateManager != null)
                _gameStateManager.OnStateChanged -= HandleStateChanged;

            KillTweens();
        }

        private void InitTweens()
        {
            if (_wheels == null || _wheels.Length == 0) return;

            _wheelTweens = new Tween[_wheels.Length];

#pragma warning disable CS4014
            for (int i = 0; i < _wheels.Length; i++)
            {
                if (_wheels[i] == null) continue;

                _wheelTweens[i] = _wheels[i]
                    .DORotate(_rotationAxis, _spinDuration, RotateMode.FastBeyond360)
                    .SetRelative(true)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Incremental)
                    .SetAutoKill(false)
                    .SetLink(_wheels[i].gameObject);

                if (_gameStateManager == null || _gameStateManager.CurrentState != GameState.Playing)
                {
                    _wheelTweens[i].Pause();
                }
            }
#pragma warning restore CS4014
        }

        private void HandleStateChanged(GameState previousState, GameState newState)
        {
            if (_wheelTweens == null) return;

#pragma warning disable CS4014
            if (newState == GameState.Playing)
            {
                for (int i = 0; i < _wheelTweens.Length; i++)
                {
                    _wheelTweens[i]?.Play();
                }
            }
            else
            {
                for (int i = 0; i < _wheelTweens.Length; i++)
                {
                    _wheelTweens[i]?.Pause();
                }
            }
#pragma warning restore CS4014
        }

        private void KillTweens()
        {
            if (_wheelTweens == null) return;
#pragma warning disable CS4014
            for (int i = 0; i < _wheelTweens.Length; i++)
            {
                _wheelTweens[i]?.Kill();
            }
#pragma warning restore CS4014

            _wheelTweens = null;
        }
    }
}