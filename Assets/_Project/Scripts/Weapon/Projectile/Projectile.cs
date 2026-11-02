using _Project.Scripts.Core.Actor;
using UnityEngine;

namespace _Project.Scripts.Weapon.Projectile
{
    [DisallowMultipleComponent]
    public sealed class Projectile : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Collider2D col;

        private float speed;
        private int damage;
        private Vector2 direction;
        private Actor owner;

        private float lifeUntil;
        private Vector2 spawnPos;
        private float maxDistance;
        private LayerMask hitMask;

        private bool initialized;

        private void Awake()
        {
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (col == null) col = GetComponent<Collider2D>();
        }

        /// <summary>
        /// Важно: damage обязателен, иначе пуля всегда будет наносить 0.
        /// </summary>
        public void Init(Vector2 dir, ProjectileDefinition definition, int damage, Actor owner)
        {
            if (definition == null)
            {
                Debug.LogError("[Projectile] Init called with null definition.", this);
                Destroy(gameObject);
                return;
            }

            this.damage = Mathf.CeilToInt(Mathf.Max(0, damage) * owner.Stats.DamageMultiplier);
            this.owner = owner;

            speed = definition.speed;
            direction = dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector2.right;

            hitMask = definition.hitMask;
            maxDistance = definition.maxDistance;

            spawnPos = transform.position;
            lifeUntil = Time.time + Mathf.Max(0.05f, definition.maxLifetime);

            // Рекомендуемая настройка: триггер + кинематик, либо динамик без гравитации.
            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.freezeRotation = true;

                // Для быстрых пуль уменьшает "пролет сквозь"
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            }

            if (col != null)
            {
                // Триггер обычно проще для пуль
                col.isTrigger = true;
            }

            initialized = true;
        }

        private void Update()
        {
            if (!initialized) return;

            // Удаляем по времени
            if (Time.time >= lifeUntil)
            {
                Destroy(gameObject);
                return;
            }

            // Удаляем по дальности (имитация "улетела за карту")
            if (maxDistance > 0f)
            {
                float dist = Vector2.Distance(spawnPos, transform.position);
                if (dist >= maxDistance)
                {
                    Destroy(gameObject);
                    return;
                }
            }

            // Если нет Rigidbody2D — двигаем трансформом (менее физично, но работает)
            if (rb == null)
                transform.position += (Vector3)(direction * speed * Time.deltaTime);
        }

        private void FixedUpdate()
        {
            if (!initialized) return;
            if (rb == null) return;

            rb.linearVelocity = direction * speed;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            HandleHit(other);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            HandleHit(collision.collider);
        }

        private void HandleHit(Collider2D other)
        {
            if (!initialized) return;
            if (other == null) return;

            // Если слой НЕ входит в hitMask — игнорим (чтобы не уничтожаться о всякую ерунду)
            int otherLayer = other.gameObject.layer;
            if ((hitMask.value & (1 << otherLayer)) == 0)
                return;

            // Ищем цель (часто коллайдер на дочернем объекте)
            var targetActor = other.GetComponentInParent<Actor>();
            if (targetActor != null)
            {
                if (owner != null && targetActor == owner)
                    return;

                targetActor.Stats.Health.TakeDamage(damage);

                var flash = targetActor.GetComponentInParent<DamageFlash2D>();
                if (flash != null)
                    flash.Flash();
            }

            // Уничтожаем при любом попадании в hitMask (стены/враги/игрок)
            Destroy(gameObject);
        }
    }
}
