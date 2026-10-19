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
            Stats.Health.TakeDamage(damage);
        }
    }
}