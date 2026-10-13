using DG.Tweening;
using Project.Core;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

namespace Project.Gameplay
{
    [RequireComponent(typeof(CinemachineCamera))]
    public class CameraTransitionController : MonoBehaviour
    {
        [Header("Menu State (Garage View)")]
        [SerializeField] private Vector3 _menuFollowOffset = new Vector3(-4.5f, 5f, -5.5f);
        [SerializeField] private Vector3 _menuRotation = new Vector3(35f, 40f, 0f);

        [Header("Playing State (Top-Down View)")]
        [SerializeField] private Vector3 _playFollowOffset = new Vector3(0f, 12f, -10f);
        [SerializeField] private Vector3 _playRotation = new Vector3(35f, 0f, 0f);

        [Header("Animation Settings")]
        [SerializeField] private float _transitionDuration = 1.5f;

        private CinemachineCamera _virtualCamera;
        private CinemachineFollow _followComponent;
        private GameStateManager _gameStateManager;

        [Inject]
        public void Construct(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }

        private void Awake()
        {
            _virtualCamera = GetComponent<CinemachineCamera>();
            _followComponent = GetComponent<CinemachineFollow>();

            ApplyStateInstant(_menuFollowOffset, _menuRotation);
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

#pragma warning disable CS4014
            transform.DOKill();
            DOTween.Kill(this);
#pragma warning restore CS4014
        }

        private void HandleStateChanged(GameState previousState, GameState newState)
        {
#pragma warning disable CS4014
            transform.DOKill();
            DOTween.Kill(this);

            if (newState == GameState.Playing)
            {
                AnimateToState(_playFollowOffset, _playRotation);
            }
            else if (newState == GameState.Menu)
            {
                AnimateToState(_menuFollowOffset, _menuRotation);
            }
#pragma warning restore CS4014
        }

        private void AnimateToState(Vector3 targetOffset, Vector3 targetRotation)
        {
            if (_followComponent == null) return;

#pragma warning disable CS4014
            DOTween.To(
                () => _followComponent.FollowOffset,
                (Vector3 x) => _followComponent.FollowOffset = x,
                targetOffset,
                _transitionDuration
            ).SetEase(Ease.InOutCubic).SetTarget(this).SetLink(gameObject);

            transform.DORotate(targetRotation, _transitionDuration)
                .SetEase(Ease.InOutCubic)
                .SetLink(gameObject);
#pragma warning restore CS4014
        }

        private void ApplyStateInstant(Vector3 offset, Vector3 rotation)
        {
            if (_followComponent != null) _followComponent.FollowOffset = offset;
            transform.rotation = Quaternion.Euler(rotation);
        }
    }
}