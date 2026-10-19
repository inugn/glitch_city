using UnityEngine;
using _Project.Scripts.Player;

namespace _Project.Scripts.Enemies.Attacks
{
    public sealed class EnemyProjectile : MonoBehaviour
    {
        private int damage;
        private Vector2 dir;
        private float speed;
        private float lifeUntil;

        public void Init(int dmg, Vector2 direction, float spd)
        {
            damage = dmg;
            dir = direction.normalized;
            speed = spd;
            lifeUntil = Time.time + 3.0f;
        }

        private void Update()
        {
            transform.position += (Vector3)(dir * speed * Time.deltaTime);
            if (Time.time > lifeUntil) Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.GetComponentInParent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}