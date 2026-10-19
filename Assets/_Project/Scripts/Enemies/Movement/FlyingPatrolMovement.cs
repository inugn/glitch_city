using UnityEngine;
using _Project.Scripts.Enemies.Core;

namespace _Project.Scripts.Enemies.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class FlyingPatrolMovement : MonoBehaviour, IEnemyMovement
    {
        [SerializeField] private Rigidbody2D rb;

        [Header("Patrol")]
        [SerializeField] private float amplitude = 0.6f;
        [SerializeField] private float frequency = 1.0f;
        [SerializeField] private float driftSpeed = 1.2f;

        [Header("Follow")]
        [SerializeField] private float followSpeed = 6f;

        private EnemyBase enemy;
        private Vector2 startPos;

        public void Setup(EnemyBase e)
        {
            enemy = e;
            if (rb == null) rb = GetComponent<Rigidbody2D>();

            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.angularVelocity = 0f;

            startPos = rb.position;
        }

        public void Tick()
        {
            if (enemy == null || enemy.Def == null || rb == null) return;

            rb.freezeRotation = true;
            rb.angularVelocity = 0f;

            switch (enemy.State)
            {
                case EnemyState.Patrol: Patrol(); break;
                case EnemyState.Chase:
                case EnemyState.Attack: Follow(); break;
                case EnemyState.Dead:
                    rb.linearVelocity = Vector2.zero;
                    break;
            }
        }

        private void Patrol()
        {
            float t = Time.time;
            float y = Mathf.Sin(t * frequency) * amplitude;
            float x = Mathf.Sin(t * 0.4f) * driftSpeed;

            var next = startPos + new Vector2(x, y);
            rb.MovePosition(Vector2.Lerp(rb.position, next, Time.fixedDeltaTime * 4f));

            enemy.FaceX(next.x - rb.position.x);
        }

        private float lastSide = 1f;

        private void Follow()
        {
            if (enemy.Target == null) return;

            Vector2 target = enemy.Target.position;
            Vector2 current = rb.position;

            if (enemy.Def.attack == AttackType.Ranged)
            {
                float dx = current.x - target.x;
                if (Mathf.Abs(dx) > 0.2f)
                    lastSide = dx >= 0f ? 1f : -1f;

                float preferred = Mathf.Max(0.5f, enemy.Def.rangedPreferredDistance);
                float yOff = enemy.Def.rangedVerticalOffset;

                // хотим быть на стороне lastSide на preferred дистанции, чуть выше
                Vector2 desired = new Vector2(target.x + lastSide * preferred, target.y + yOff);

                var next = Vector2.MoveTowards(current, desired, followSpeed * Time.fixedDeltaTime);
                rb.MovePosition(next);

                // всегда смотрим на игрока
                enemy.FaceX(target.x - current.x);
                return;
            }

            // melee/explode — прямо в игрока
            var next2 = Vector2.MoveTowards(current, target, followSpeed * Time.fixedDeltaTime);
            rb.MovePosition(next2);
            enemy.FaceX(target.x - current.x);
        }

    }
}
