using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Core
{
    public class GameStateManager
    {
        public GameState CurrentState { get; private set; }
        public event Action<GameState, GameState> OnStateChanged;

        public GameStateManager()
        {
            CurrentState = GameState.Menu;
            Time.timeScale = 1f;
        }

        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState) return;

            GameState previousState = CurrentState;
            CurrentState = newState;

            if (CurrentState == GameState.Pause)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }

            if (previousState == GameState.Lose && newState == GameState.Menu ||
                previousState == GameState.Win && newState == GameState.Playing ||
                previousState == GameState.Win && newState == GameState.Menu)
            {
                ExecuteSceneReload();
                return;
            }

            OnStateChanged?.Invoke(previousState, CurrentState);
        }

        private void ExecuteSceneReload()
        {
            Time.timeScale = 1f;

#pragma warning disable CS4014
            DOTween.KillAll();
#pragma warning restore CS4014

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}