using _Project.Scripts.Enemies.Core;
using UnityEngine;

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
        [SerializeField] private float followSpeed = 6f;  // Скорость следования

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
                case EnemyState.Patrol:
                    Patrol();
                    break;

                case EnemyState.Chase:
                case EnemyState.Attack:
                    Follow();
                    break;

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

            // Получаем текущие координаты игрока и врага
            Vector2 target = enemy.Target.position;
            Vector2 current = rb.position;

            // Рассчитываем расстояние между врагом и игроком
            float distanceToPlayer = Vector2.Distance(current, target);

            // Если враг дальнего боя (Ranged)
            if (enemy.Def.attack == AttackType.Ranged)
            {
                // Минимальное и максимальное расстояние, которое враг может поддерживать от игрока
                float minDistance = 3f;  // Минимальное расстояние (если враг слишком близко, он отходит)
                float maxDistance = 6f;  // Максимальное расстояние (если враг слишком далеко, он приближается)

                // Если враг слишком близко, он отходит
                if (distanceToPlayer < minDistance)
                {
                    rb.MovePosition(current - (target - current).normalized * enemy.Def.moveSpeed * Time.fixedDeltaTime);
                }
                // Если враг слишком далеко, он приближается
                else if (distanceToPlayer > maxDistance)
                {
                    rb.MovePosition(current + (target - current).normalized * enemy.Def.moveSpeed * Time.fixedDeltaTime);
                }
            }
            // Если враг ближнего боя (Melee) или взрывной
            else if (enemy.Def.attack == AttackType.Melee || enemy.Def.attack == AttackType.Explode)
            {
                // Просто идём в сторону игрока
                rb.MovePosition(current + (target - current).normalized * enemy.Def.moveSpeed * Time.fixedDeltaTime);
            }

            // Поворот врага к игроку (если он двигается)
            enemy.FaceX(target.x - current.x);
        }


        
    }
}
