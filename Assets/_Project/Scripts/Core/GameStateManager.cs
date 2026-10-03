using System;

namespace Project.Core
{
    public class GameStateManager
    {
        public GameState CurrentState { get; private set; }

        public event Action<GameState> OnStateChanged;

        public GameStateManager()
        {
            CurrentState = GameState.Playing;
        }

        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState) return;
            CurrentState = newState;
            OnStateChanged?.Invoke(CurrentState);
        }
    }
}