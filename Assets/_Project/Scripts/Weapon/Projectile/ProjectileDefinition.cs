using UnityEngine;

namespace _Project.Scripts.Weapon.Projectile
{
    [CreateAssetMenu(menuName = "Game/Weapons/Projectile")]
    public sealed class ProjectileDefinition : ScriptableObject
    {
        [Header("Prefab")]
        public GameObject prefab;

        [Header("Movement")]
        [Min(0.1f)] public float speed = 10f;

        [Header("Lifetime / Bounds")]
        [Min(0.1f)] public float maxLifetime = 4f;

        [Tooltip("0 = отключить проверку дистанции. Иначе пуля удалится, когда улетит дальше этого значения от точки спавна.")]
        [Min(0f)] public float maxDistance = 30f;

        [Header("Hit")]
        [Tooltip("Слои, при контакте с которыми пуля должна уничтожаться (стены, враги, игрок и т.д.)")]
        public LayerMask hitMask;
    }
}