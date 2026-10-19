using System;
using UnityEngine;

namespace _Project.Scripts.Core.Actor
{
    public class Health
    {
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private float regenPerSecond = 0f;

        public int MaxHealth => maxHealth;
        public float RegenPerSecond => regenPerSecond;

        public int CurrentHealth { get; private set; }

        public bool IsDead => CurrentHealth <= 0;

        public event Action<int, int> OnHealthChanged; // current, max
        public event Action OnDeath;

        public void Initialize()
        {
            CurrentHealth = maxHealth;
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        public void TakeDamage(int damage)
        {
            if (IsDead || damage <= 0) return;

            CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

            if (IsDead)
                OnDeath?.Invoke();
        }

        public void Heal(int value)
        {
            if (IsDead || value <= 0) return;

            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + value);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        public void Regenerate(float deltaTime)
        {
            if (regenPerSecond <= 0f || IsDead) return;

            float regenAmount = regenPerSecond * deltaTime;
            if (regenAmount < 1f) return;

            Heal(Mathf.FloorToInt(regenAmount));
        }

        public void ModifyMaxHealth(int value, bool heal = true)
        {
            maxHealth += value;

            if (heal)
                CurrentHealth = maxHealth;
            else
                CurrentHealth = Mathf.Min(CurrentHealth, maxHealth);

            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }
    }
}