using System;
using UnityEngine;

namespace Project.Services
{
    public class SwipeInputService : IDisposable
    {
        private readonly GameInput _input;

        public SwipeInputService()
        {
            _input = new GameInput();
            _input.Gameplay.Enable();
        }

        public float GetHorizontalDelta()
        {
            Vector2 delta = _input.Gameplay.Aim.ReadValue<Vector2>();
            return delta.x;
        }

        public void Dispose()
        {
            _input.Gameplay.Disable();
        }
    }
}