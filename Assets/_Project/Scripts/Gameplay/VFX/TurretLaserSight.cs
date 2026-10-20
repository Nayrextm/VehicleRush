using DG.Tweening;
using Project.Core;
using UnityEngine;
using VContainer;

namespace Project.Gameplay.VFX
{
    [RequireComponent(typeof(LineRenderer))]
    public class TurretLaserSight : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Laser point (usually barrel tip)")]
        [SerializeField] private Transform _firePoint;

        [Header("Laser Settings")]
        [Tooltip("Maximum length of the laser, if it didn't hit anything")]
        [SerializeField] private float _maxDistance = 50f;
        [Tooltip("Layers the laser interacts with (Enemies, Ground, Obstacles)")]
        [SerializeField] private LayerMask _hitMask;

        [Header("Visuals (DOTween)")]
        [SerializeField] private float _pulseDuration = 0.5f;
        [SerializeField] private float _minWidth = 0.02f;
        [SerializeField] private float _maxWidth = 0.08f;

        private LineRenderer _lineRenderer;
        private GameStateManager _gameStateManager;

        private float _currentWidth;

        [Inject]
        public void Construct(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();

            _lineRenderer.positionCount = 2;
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _lineRenderer.receiveShadows = false;
        }

        private void OnEnable()
        {
            if (_gameStateManager != null)
                _gameStateManager.OnStateChanged += HandleStateChanged;

            StartPulseAnimation();
        }

        private void OnDisable()
        {
            if (_gameStateManager != null)
                _gameStateManager.OnStateChanged -= HandleStateChanged;

#pragma warning disable CS4014
            _lineRenderer.DOKill();
            DOTween.Kill(this);
#pragma warning restore CS4014
        }

        private void LateUpdate()
        {
            if (_gameStateManager == null || _gameStateManager.CurrentState != GameState.Playing)
            {
                if (_lineRenderer.enabled) _lineRenderer.enabled = false;
                return;
            }

            if (!_lineRenderer.enabled) _lineRenderer.enabled = true;

            DrawLaser();
        }

        private void DrawLaser()
        {
            if (_firePoint == null) return;

            _lineRenderer.SetPosition(0, _firePoint.position);

            if (Physics.Raycast(_firePoint.position, _firePoint.forward, out RaycastHit hit, _maxDistance, _hitMask))
            {
                _lineRenderer.SetPosition(1, hit.point);
            }
            else
            {
                _lineRenderer.SetPosition(1, _firePoint.position + _firePoint.forward * _maxDistance);
            }
        }

        private void StartPulseAnimation()
        {
#pragma warning disable CS4014
            _lineRenderer.DOKill();
            DOTween.Kill(this);

            _currentWidth = _maxWidth;
            _lineRenderer.startWidth = _currentWidth;
            _lineRenderer.endWidth = _currentWidth;

            DG.Tweening.Core.DOGetter<float> getter = () => _currentWidth;
            DG.Tweening.Core.DOSetter<float> setter = x =>
            {
                _currentWidth = x;
                _lineRenderer.startWidth = _currentWidth;
                _lineRenderer.endWidth = _currentWidth;
            };

            DOTween.To(getter, setter, _minWidth, _pulseDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetTarget(this)
                .SetLink(gameObject);
#pragma warning restore CS4014
        }

        private void HandleStateChanged(GameState previousState, GameState newState)
        {
            _lineRenderer.enabled = (newState == GameState.Playing);
        }
    }
}