using System;
using UnityEngine;

namespace Project.Gameplay
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int _maxHealth = 100;
        private int _currentHealth;

        public event Action<int, int> OnHealthChanged;
        public event Action OnDied;

        private void Awake()
        {
            ResetHealth();
        }

        public void ResetHealth()
        {
            _currentHealth = _maxHealth;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        public void TakeDamage(int amount)
        {
            if (_currentHealth <= 0) return;

            _currentHealth -= amount;
            _currentHealth = Mathf.Max(0, _currentHealth);

            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (_currentHealth == 0)
            {
                OnDied?.Invoke();
            }
        }
    }
}