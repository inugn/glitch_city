using UnityEngine;
using _Project.Scripts.Enemies.Core;
using _Project.Scripts.Player;

namespace _Project.Scripts.Enemies.Attacks
{
    public sealed class ExplodeAttack : MonoBehaviour, IEnemyAttack
    {
        private EnemyBase enemy;
        private bool armed;

        public void Setup(EnemyBase e)
        {
            enemy = e;
            armed = true;
        }

        public void Tick()
        {
            if (!armed || enemy == null || enemy.Def == null || enemy.Target == null) return;
            if (enemy.State == EnemyState.Patrol || enemy.State == EnemyState.Dead) return;

            float dist = Vector2.Distance(transform.position, enemy.Target.position);
            if (dist > enemy.Def.explodeRadius) return;

            Explode();
        }

        private void Explode()
        {
            if (!armed || enemy == null) return;
            armed = false;

            var player = enemy.Target.GetComponentInParent<PlayerHealth>();
            if (player != null)
                player.TakeDamage(enemy.Def.explodeDamage);

            enemy.TakeDamage(int.MaxValue);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // fallback, если всё же используешь отдельный trigger-коллайдер
            if (!armed || enemy == null) return;

            var player = other.GetComponentInParent<PlayerHealth>();
            if (player != null)
                Explode();
        }
    }
}