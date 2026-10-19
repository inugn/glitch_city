using UnityEngine;
using _Project.Scripts.Enemies.Core;

namespace _Project.Scripts.Enemies.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class GroundPatrolMovement : MonoBehaviour, IEnemyMovement
    {
        [SerializeField] private Rigidbody2D rb;

        [Header("Patrol")]
        [SerializeField] private float patrolDir = 1f;
        [SerializeField] private Transform wallCheck;
        [SerializeField] private float wallCheckDist = 0.2f;

        [SerializeField] private Transform edgeCheck;
        [SerializeField] private float edgeCheckDepth = 0.4f;

        [SerializeField] private LayerMask groundMask;

        private EnemyBase enemy;
        private float lastNonZeroDir = 1f;

        public void Setup(EnemyBase e)
        {
            enemy = e;
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            rb.freezeRotation = true;           // ✅ чтобы не крутился физикой
            rb.angularVelocity = 0f;

            if (edgeCheck == null) edgeCheck = wallCheck;
        }

        public void Tick()
        {
            if (enemy == null || enemy.Def == null || rb == null) return;

            rb.freezeRotation = true; // на случай, если кто-то в префабе/рантайме сбил
            rb.angularVelocity = 0f;

            switch (enemy.State)
            {
                case EnemyState.Patrol: Patrol(); break;
                case EnemyState.Chase:  Chase(); break;
                case EnemyState.Attack: AttackMove(); break;
                case EnemyState.Dead:
                    rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                    break;
            }
        }

        private void Patrol()
        {
            float speed = enemy.Def.moveSpeed;
            rb.linearVelocity = new Vector2(patrolDir * speed, rb.linearVelocity.y);
            enemy.FaceX(patrolDir);

            if (ShouldTurnAround())
                patrolDir *= -1f;
        }

        private bool ShouldTurnAround()
        {
            // ⚠️ Если wallCheck/edgeCheck стоят внутри земли, raycast будет "всегда попадать"
            // и враг начнёт дрожать/крутиться.
            if (wallCheck != null)
            {
                var hit = Physics2D.Raycast(wallCheck.position, Vector2.right * patrolDir, wallCheckDist, groundMask);
                if (hit.collider != null) return true;
            }

            if (edgeCheck != null)
            {
                var downHit = Physics2D.Raycast(edgeCheck.position, Vector2.down, edgeCheckDepth, groundMask);
                if (downHit.collider == null) return true;
            }

            return false;
        }

        private void Chase()
        {
            if (enemy.Target == null) return;

            float dx = enemy.Target.position.x - transform.position.x;
            float dir = dx >= 0f ? 1f : -1f; // ✅ никогда не 0, меньше дрожания
            lastNonZeroDir = dir;

            rb.linearVelocity = new Vector2(dir * enemy.Def.moveSpeed, rb.linearVelocity.y);
            enemy.FaceX(dir);
        }

        private void AttackMove()
        {
            if (enemy.Target == null) return;

            float dx = enemy.Target.position.x - transform.position.x;
            float dirToPlayer = dx >= 0f ? 1f : -1f;

            if (enemy.Def.attack == AttackType.Ranged)
            {
                float dist = enemy.DistanceToTarget();
                float preferred = Mathf.Max(0.5f, enemy.Def.rangedPreferredDistance);
                float tol = Mathf.Max(0.05f, enemy.Def.rangedDistanceTolerance);

                float vx = 0f;

                if (dist < preferred - tol)
                    vx = -dirToPlayer * enemy.Def.moveSpeed;        // слишком близко -> отходим
                else if (dist > preferred + tol)
                    vx = dirToPlayer * enemy.Def.moveSpeed;         // слишком далеко -> подходим
                else
                    vx = 0f;                                        // в окне -> стоим

                rb.linearVelocity = new Vector2(vx, rb.linearVelocity.y);

                // лицом к игроку даже если стоим
                enemy.FaceX(dirToPlayer);
                return;
            }

            // melee/explode — поджимаемся
            rb.linearVelocity = new Vector2(dirToPlayer * enemy.Def.moveSpeed, rb.linearVelocity.y);
            enemy.FaceX(dirToPlayer);
        }

    }
}
