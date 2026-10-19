using _Project.Scripts.Core.Actor;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public class Player : Actor
    {
        // TODO слоты оружия

        private void Update()
        {
            Stats.Health.Regenerate(Time.deltaTime);
        }

        public void TakeDamage(int damage)
        {
            Debug.Log(stats.Health.CurrentHealth + "[" + stats.Health.MaxHealth + "]");
            Stats.Health.TakeDamage(damage);
        }
    }
}