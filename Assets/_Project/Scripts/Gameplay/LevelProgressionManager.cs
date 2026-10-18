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

        private bool _isRunSaved = false;

        public int SavedTotalScore { get; private set; }
        public int CurrentLevel { get; private set; }
        public int CurrentLevelTarget => _levelDistanceStep;

        public int DisplayScore
        {
            get
            {
                if (_isRunSaved) return SavedTotalScore;
                int currentRun = _distanceTracker != null ? _distanceTracker.RunDistance : 0;
                return SavedTotalScore + currentRun;
            }
        }

        [Inject]
        public void Construct(GameStateManager gameStateManager, DistanceTracker distanceTracker)
        {
            _gameStateManager = gameStateManager;
            _distanceTracker = distanceTracker;
        }

        private void Awake()
        {
            SavedTotalScore = PlayerPrefs.GetInt("TotalScore", 0);
            CurrentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);

            if (CurrentLevel < 1)
            {
                CurrentLevel = 1;
                PlayerPrefs.SetInt("CurrentLevel", 1);
                PlayerPrefs.Save();
            }
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

        private void Update()
        {
            if (_gameStateManager == null || _gameStateManager.CurrentState != GameState.Playing)
                return;

            if (_distanceTracker != null && _distanceTracker.RunDistance >= _levelDistanceStep)
            { 
                if (!_isRunSaved)
                {
                    SavedTotalScore += _distanceTracker.RunDistance;
                    CurrentLevel++;

                    PlayerPrefs.SetInt("TotalScore", SavedTotalScore);
                    PlayerPrefs.SetInt("CurrentLevel", CurrentLevel);
                    PlayerPrefs.Save(); 

                    _isRunSaved = true;
                }

                _gameStateManager.ChangeState(GameState.Win);
            }
        }

        private void HandleStateChanged(GameState previousState, GameState newState)
        {
            if (newState == GameState.Playing)
            {
                _isRunSaved = false;
            }

            if (previousState == GameState.Playing && newState == GameState.Lose)
            {
                if (!_isRunSaved && _distanceTracker != null)
                {
                    SavedTotalScore += _distanceTracker.RunDistance;
                    PlayerPrefs.SetInt("TotalScore", SavedTotalScore);
                    PlayerPrefs.Save();

                    _isRunSaved = true;
                }
            }
        }

        [ContextMenu("Clear Player Prefs Saved Data")]
        public void ClearSaveData()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("[LevelProgressionManager] All PlayerPrefs cleared fully!");
        }
    }
}