using Project.Core;
using UnityEngine;
using VContainer;

namespace Project.Gameplay
{
    public class LevelProgressionManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int _levelDistanceStep = 200;

        private GameStateManager _gameStateManager;
        private DistanceTracker _distanceTracker;

        private int _initialMilestone;
        private int _milestoneAtCurrentLevelStart;

        public int CurrentTarget { get; private set; }
        public int LevelStep => _levelDistanceStep;

        public int CurrentLevelStart => _milestoneAtCurrentLevelStart;

        public int CurrentTotalDistance => _initialMilestone + (_distanceTracker != null ? _distanceTracker.RunDistance : 0);

        [Inject]
        public void Construct(GameStateManager gameStateManager, DistanceTracker distanceTracker)
        {
            _gameStateManager = gameStateManager;
            _distanceTracker = distanceTracker;
        }

        private void Awake()
        {
            _initialMilestone = PlayerPrefs.GetInt("SavedMilestone", 0);
            CurrentTarget = PlayerPrefs.GetInt("CurrentTarget", _levelDistanceStep);

            _milestoneAtCurrentLevelStart = CurrentTarget - _levelDistanceStep;
        }

        private void Update()
        {
            if (_gameStateManager == null || _gameStateManager.CurrentState != GameState.Playing)
                return;

            if (CurrentTotalDistance >= CurrentTarget)
            {
                _gameStateManager.ChangeState(GameState.Win);
            }
        }

        public void AdvanceLevel()
        {
            PlayerPrefs.SetInt("SavedMilestone", CurrentTarget);

            CurrentTarget += _levelDistanceStep;
            PlayerPrefs.SetInt("CurrentTarget", CurrentTarget);
            PlayerPrefs.Save();

            _milestoneAtCurrentLevelStart = CurrentTarget - _levelDistanceStep;

            if (_distanceTracker != null)
            {
                _distanceTracker.ResetRunDistanceOrigin();
            }

            _initialMilestone = PlayerPrefs.GetInt("SavedMilestone", 0);
        }
    }
}