using DG.Tweening;
using Project.Core;
using Project.Gameplay;
using TMPro;
using UnityEngine;
using VContainer;

namespace Project.UI
{
    public class LevelIndicatorUI : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Level Count")]
        [SerializeField] private TextMeshProUGUI _levelText;

        [Tooltip("Level Container for animation (Scale)")]
        [SerializeField] private RectTransform _containerToAnimate;

        private LevelProgressionManager _progressionManager;
        private GameStateManager _gameStateManager;

        [Inject]
        public void Construct(LevelProgressionManager progressionManager, GameStateManager gameStateManager)
        {
            _progressionManager = progressionManager;
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

#pragma warning disable CS4014
            _containerToAnimate?.DOKill();
#pragma warning restore CS4014
        }

        private void Start()
        {
            UpdateLevelUI();
        }

        private void HandleStateChanged(GameState previousState, GameState newState)
        {
            if (newState == GameState.Playing)
            {
                UpdateLevelUI();

#pragma warning disable CS4014

                if (_containerToAnimate != null)
                {
                    _containerToAnimate.DOKill();
                    _containerToAnimate.localScale = Vector3.zero;

                    _containerToAnimate.DOScale(Vector3.one, 0.5f)
                        .SetEase(Ease.OutBack)
                        .SetLink(gameObject);
                }
#pragma warning restore CS4014
            }
        }

        private void UpdateLevelUI()
        {
            if (_levelText != null && _progressionManager != null)
            {
                _levelText.SetText("\u00A0{0}", _progressionManager.CurrentLevel);
            }
        }
    }
}