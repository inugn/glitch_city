using _Project.Scripts.Core.Actor;
using UnityEngine;

namespace _Project.UI
{
    public sealed class BossHealthPresenter : MonoBehaviour
    {
        [SerializeField] private BossHealthView view;

        private Health health;

        public void Bind(Stats stats)
        {
            health = stats.Health;

            view.SetMaxHealth(health.MaxHealth);
            view.SetHealth(health.CurrentHealth);
            view.Show();

            health.OnHealthChanged += OnHealthChanged;
            health.OnDeath += OnDied;
        }

        private void OnHealthChanged(int value, int max)
        {
            view.SetHealth(value);
        }

        private void OnDied()
        {
            Unbind();
            view.Hide();
        }

        public void Unbind()
        {
            if (health == null) return;

            health.OnHealthChanged -= OnHealthChanged;
            health.OnDeath -= OnDied;
            health = null;
        }

        private void Awake()
        {
            view.Hide();
        }
    }
}