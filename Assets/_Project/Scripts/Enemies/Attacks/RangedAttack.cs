using UnityEngine;
using _Project.Scripts.Enemies.Core;

namespace _Project.Scripts.Enemies.Attacks
{
    public sealed class RangedAttack : MonoBehaviour, IEnemyAttack
    {
        [SerializeField] private EnemyProjectile projectilePrefab;
        [SerializeField] private Transform muzzle;

        private EnemyBase enemy;
        private float nextTime;

        public void Setup(EnemyBase e)
        {
            enemy = e;
            nextTime = 0f;
        }

        public void Tick()
        {
            
            if (enemy == null || enemy.Def == null || enemy.Target == null) return;
            if (projectilePrefab == null || muzzle == null) return;

            // Стрелок может стрелять и в Chase, и в Attack, но не в Patrol.
            if (enemy.State == EnemyState.Patrol || enemy.State == EnemyState.Dead) return;

            float dist = enemy.DistanceToTarget();
            if (dist > enemy.Def.rangedMaxRange) return;

            if (Time.time < nextTime) return;

            var toPlayer = (enemy.Target.position - muzzle.position);
            var dir = ((Vector2)toPlayer).normalized;

            // Разброс
            float spread = Random.Range(-enemy.Def.accuracyDeg, enemy.Def.accuracyDeg);
            dir = Quaternion.Euler(0, 0, spread) * dir;

            var p = Instantiate(projectilePrefab, muzzle.position, Quaternion.identity);
            p.Init(enemy.Def.rangedDamage, dir, enemy.Def.projectileSpeed);

            nextTime = Time.time + enemy.Def.rangedCooldown;
        }
    }
}