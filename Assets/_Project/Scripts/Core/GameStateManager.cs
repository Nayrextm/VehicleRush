using System;
using DG.Tweening;
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
        }

        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState) return;

            GameState previousState = CurrentState;
            CurrentState = newState;

            if (newState == GameState.Menu || (previousState == GameState.Win && newState == GameState.Playing))
            {
                ExecuteSceneReload();
                return; 
            }

            OnStateChanged?.Invoke(previousState, CurrentState);
        }

        private void ExecuteSceneReload()
        {
#pragma warning disable CS4014
            DOTween.KillAll();
#pragma warning restore CS4014
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}