using System;
using _Project.Scripts.Core.Actor;
using UnityEngine;

namespace _Project.Scripts.Enemies.Core
{
    public class EnemyBase : Actor
    {
        [SerializeField] private SpriteRenderer sprite;

        public EnemyDefinition Def { get; private set; }
        public Transform Target { get; private set; }

        public bool IsDead { get; private set; }
        public EnemyState State { get; private set; } = EnemyState.Patrol;

        public event Action<EnemyBase> Died;

        private float hitTintUntil;

        public void Init(EnemyDefinition def, Transform target)
        {
            Def = def;
            Target = target;

            // Инициализация через экземпляр Stats (это будет работать)
            if (Stats != null) 
                Stats.Initialize();

            SetState(EnemyState.Patrol);
        }

        public void SetState(EnemyState state)
        {
            if (IsDead) state = EnemyState.Dead;
            State = state;
        }

        public float DistanceToTarget()
        {
            if (Target == null) return float.PositiveInfinity;
            return Vector2.Distance(transform.position, Target.position);
        }

        public int GetFacingSign()
        {
            if (sprite == null) return 1;
            return sprite.flipX ? -1 : 1;
        }

        public void FaceX(float dirX)
        {
            if (sprite == null) return;
            if (dirX > 0.01f) sprite.flipX = false;
            else if (dirX < -0.01f) sprite.flipX = true;
        }

        public void TakeDamage(int amount)
        {
            if (IsDead) return;

            // Доступ через экземпляр Stats
            if (Stats != null) 
            {
                Stats.Health.TakeDamage(amount);  // Доступ к экземпляру Health
                FlashHit();

                if (Stats.Health.IsDead)  // Проверка через экземпляр Health
                    Die();
            }
        }

        protected virtual void Die()
        {
            if (IsDead) return;
            IsDead = true;

            SetState(EnemyState.Dead);
            Died?.Invoke(this);

            Destroy(gameObject);
        }

        private void FlashHit()
        {
            if (sprite == null) return;
            hitTintUntil = Time.time + 0.08f;
            sprite.color = new Color(1f, 0.4f, 0.4f, 1f);
        }

        protected virtual void Update()
        {
            if (sprite == null) return;
            if (hitTintUntil > 0 && Time.time > hitTintUntil)
            {
                sprite.color = Color.white;
                hitTintUntil = 0f;
            }
        }
    }
}
