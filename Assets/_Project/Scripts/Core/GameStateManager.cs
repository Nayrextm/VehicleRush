using System;

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

            OnStateChanged?.Invoke(previousState, CurrentState);
        }
    }
}