using UnityEngine;

namespace _Project.Scripts.Core.Actor
{
    public class Actor : MonoBehaviour
    {
        [SerializeField] protected Stats stats;
        public Stats Stats => stats;

        protected virtual void Awake()
        {
            if (stats == null)
            {
                Debug.LogError($"[{name}] Stats is not assigned in Inspector.", this);
                enabled = false;
                return;
            }

            stats.Initialize();
            stats.Health.OnDeath += OnDeath;
        }

        protected virtual void OnDestroy()
        {
            if (stats?.Health != null)
                stats.Health.OnDeath -= OnDeath;
        }

        protected virtual void OnDeath()
        {
            // По умолчанию — ничего.
            // Враг/игрок переопределят.
        }
    }
}