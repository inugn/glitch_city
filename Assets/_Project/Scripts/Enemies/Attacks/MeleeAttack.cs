using UnityEngine;
using _Project.Scripts.Enemies.Core;
using _Project.Scripts.Player;

namespace _Project.Scripts.Enemies.Attacks
{
    public sealed class MeleeAttack : MonoBehaviour, IEnemyAttack
    {
        private EnemyBase enemy;
        private float nextTime;

        public void Setup(EnemyBase e)
        {
            enemy = e;
            nextTime = 0f;
        }

        public void Tick()
        {
            if (enemy == null || enemy.Target == null || enemy.Def == null) return;
            if (enemy.State != EnemyState.Attack) return;
            if (Time.time < nextTime) return;

            var dist = Vector2.Distance(transform.position, enemy.Target.position);
            if (dist > enemy.Def.meleeRange) return;

            var player = enemy.Target.GetComponentInParent<_Project.Scripts.Player.PlayerHealth>();
            if (player != null)
                player.TakeDamage(enemy.Def.meleeDamage);

            nextTime = Time.time + enemy.Def.meleeCooldown;
        }

    }
}