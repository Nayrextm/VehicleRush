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

        private int _baseTotalScore;
        private int _bestLevelScore;

        private bool _isLevelCompleted = false;

        public int CurrentLevel { get; private set; }
        public int CurrentLevelTarget => _levelDistanceStep;

        public int DisplayScore
        {
            get
            {
                int currentRun = (_gameStateManager != null && _gameStateManager.CurrentState == GameState.Playing && _distanceTracker != null)
                    ? _distanceTracker.RunDistance
                    : 0;

                return _baseTotalScore + Mathf.Max(_bestLevelScore, currentRun);
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
            _baseTotalScore = PlayerPrefs.GetInt("BaseTotalScore", 0);
            _bestLevelScore = PlayerPrefs.GetInt("BestLevelScore", 0);
            CurrentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);

            if (CurrentLevel < 1)
            {
                CurrentLevel = 1;
                SaveProgress();
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

            if (!_isLevelCompleted && _distanceTracker != null && _distanceTracker.RunDistance >= _levelDistanceStep)
            {
                _isLevelCompleted = true;

                _baseTotalScore += _levelDistanceStep;
                _bestLevelScore = 0;
                CurrentLevel++;

                SaveProgress(); 

                _gameStateManager.ChangeState(GameState.Win);
            }
        }

        private void HandleStateChanged(GameState previousState, GameState newState)
        {
            if (newState == GameState.Playing)
            {
                _isLevelCompleted = false;
            }

            else if (previousState == GameState.Playing && (newState == GameState.Lose || newState == GameState.Menu))
            {
                if (!_isLevelCompleted && _distanceTracker != null)
                {
                    int runDistance = _distanceTracker.RunDistance;

                    if (runDistance > _bestLevelScore)
                    {
                        _bestLevelScore = runDistance;
                        SaveProgress();
                    }
                }
            }
        }

        private void SaveProgress()
        {
            PlayerPrefs.SetInt("BaseTotalScore", _baseTotalScore);
            PlayerPrefs.SetInt("BestLevelScore", _bestLevelScore);
            PlayerPrefs.SetInt("CurrentLevel", CurrentLevel);
            PlayerPrefs.Save();
        }

        [ContextMenu("Clear Player Prefs Saved Data")]
        public void ClearSaveData()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            _baseTotalScore = 0;
            _bestLevelScore = 0;
            CurrentLevel = 1;

            Debug.Log("[LevelProgressionManager] All PlayerPrefs cleared fully!");
        }
    }
}