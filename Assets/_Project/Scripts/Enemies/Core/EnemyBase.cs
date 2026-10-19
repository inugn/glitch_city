using System;
using UnityEngine;

namespace _Project.Scripts.Enemies.Core
{
    [RequireComponent(typeof(Collider2D))]
    public class EnemyBase : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer sprite;

        public EnemyDefinition Def { get; private set; }
        public Transform Target { get; private set; }

        public int Hp { get; private set; }
        public bool IsDead { get; private set; }

        public EnemyState State { get; private set; } = EnemyState.Patrol;
        public event Action<EnemyBase> Died;

        private float hitTintUntil;

        public virtual void Init(EnemyDefinition def, Transform target)
        {
            Def = def;
            Target = target;
            Hp = def != null ? def.maxHp : 1;

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

            Hp -= amount;
            FlashHit();

            if (Hp <= 0)
                Die();
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
