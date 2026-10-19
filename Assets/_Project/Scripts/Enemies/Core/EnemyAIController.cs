using UnityEngine;
using _Project.Scripts.Enemies.Movement;
using _Project.Scripts.Enemies.Attacks;

namespace _Project.Scripts.Enemies.Core
{
    [RequireComponent(typeof(EnemyBase))]
    public sealed class EnemyAIController : MonoBehaviour
    {
        [SerializeField] private EnemyBase enemy;

        [Header("Movement components")]
        [SerializeField] private GroundPatrolMovement groundMovement;
        [SerializeField] private FlyingPatrolMovement flyingMovement;
        [SerializeField] private StaticMovement staticMovement;

        [Header("Attack components")]
        [SerializeField] private MeleeAttack meleeAttack;
        [SerializeField] private RangedAttack rangedAttack;
        [SerializeField] private ExplodeAttack explodeAttack;

        [Header("AI")]
        [Tooltip("Как часто пересчитывать состояния. 0.1 = ок для MVP.")]
        [SerializeField] private float stateTickInterval = 0.1f;

        [Tooltip("Гистерезис, чтобы не дёргалось на границе дистанции атаки.")]
        [SerializeField] private float attackExitExtra = 0.4f;

        private IEnemyMovement movement;
        private IEnemyAttack attack;

        private float nextStateTime;

        private void Awake()
        {
            if (enemy == null) enemy = GetComponent<EnemyBase>();
        }

        public void ConfigureFromDefinition()
        {
            if (enemy.Def == null)
            {
                Debug.LogError("[EnemyAIController] EnemyDefinition is null.", this);
                enabled = false;
                return;
            }

            movement = enemy.Def.movement switch
            {
                MovementType.GroundPatrol => (IEnemyMovement)groundMovement,
                MovementType.FlyingPatrol => (IEnemyMovement)flyingMovement,
                MovementType.Static => (IEnemyMovement)staticMovement,
                _ => staticMovement
            };

            attack = enemy.Def.attack switch
            {
                AttackType.Melee => (IEnemyAttack)meleeAttack,
                AttackType.Ranged => (IEnemyAttack)rangedAttack,
                AttackType.Explode => (IEnemyAttack)explodeAttack,
                AttackType.None => null,
                _ => null
            };

            movement?.Setup(enemy);
            attack?.Setup(enemy);

            enemy.SetState(EnemyState.Patrol);
            nextStateTime = 0f;
        }

        private void Update()
        {
            if (enemy.IsDead || enemy.Def == null) return;

            // Пересчёт состояния редко (без дрожания)
            if (Time.time >= nextStateTime)
            {
                nextStateTime = Time.time + Mathf.Max(0.02f, stateTickInterval);
                EvaluateState();
            }

            // Атаки в Update
            if (enemy.State == EnemyState.Chase || enemy.State == EnemyState.Attack)
                attack?.Tick();
        }

        private void FixedUpdate()
        {
            if (enemy.IsDead || enemy.Def == null) return;
            movement?.Tick();
        }

        private void EvaluateState()
        {
            if (enemy.Target == null)
            {
                enemy.SetState(EnemyState.Patrol);
                return;
            }

            float dist = enemy.DistanceToTarget();

            // 1) Логика “вход/выход”:
            // - В патруле мы начинаем агриться только в detectRadius
            // - Из агро (chase/attack) выходим только когда ушли за chaseRadius
            if (enemy.State == EnemyState.Patrol)
            {
                if (dist > enemy.Def.detectRadius)
                    return; // остаёмся в Patrol

                // вошли в агро
                enemy.SetState(CanAttack(dist) ? EnemyState.Attack : EnemyState.Chase);
                return;
            }

            // мы уже в агро (Chase/Attack)
            if (dist > enemy.Def.chaseRadius)
            {
                enemy.SetState(EnemyState.Patrol);
                return;
            }

            // внутри агро зоны — выбираем Attack/Chase
            if (enemy.State == EnemyState.Attack)
            {
                // гистерезис: выход из атаки чуть позже
                float exitRange = GetAttackEnterRange() + attackExitExtra;
                if (dist > exitRange)
                    enemy.SetState(EnemyState.Chase);
                return;
            }

            // если не в атаке — можем войти в атаку
            if (CanAttack(dist))
                enemy.SetState(EnemyState.Attack);
            else
                enemy.SetState(EnemyState.Chase);
        }

        private bool CanAttack(float dist)
        {
            if (enemy.Def == null) return false;

            return enemy.Def.attack switch
            {
                AttackType.Melee => dist <= enemy.Def.meleeRange,
                AttackType.Ranged => dist <= enemy.Def.rangedMaxRange,
                AttackType.Explode => dist <= enemy.Def.explodeRadius,
                _ => false
            };
        }

        private float GetAttackEnterRange()
        {
            return enemy.Def.attack switch
            {
                AttackType.Melee => enemy.Def.meleeRange,
                AttackType.Ranged => enemy.Def.rangedMaxRange,
                AttackType.Explode => enemy.Def.explodeRadius,
                _ => 0f
            };
        }
        
    }
}
