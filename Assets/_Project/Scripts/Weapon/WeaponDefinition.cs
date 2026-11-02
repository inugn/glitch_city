using _Project.Scripts.Weapon.Projectile;
using UnityEngine;

namespace _Project.Scripts.Weapon
{
    [CreateAssetMenu(menuName = "Game/Weapons/Ranged Weapon")]
    public sealed class WeaponDefinition : ScriptableObject
    {
        [Header("Visual")]
        public GameObject weaponPrefab;
        public Sprite icon;
        
        [Header("General")]
        public string weaponId;

        [Header("Damage")]
        public int damage;

        [Header("Fire")]
        public float fireRate; 

        [Header("Magazine")]
        public int magazineSize;
        public float reloadTime;

        [Header("Ranged specific")]
        // для двух пистолетов тут будет два
        public int bulletsPerShot = 1;
        public float spreadAngle;
        
        [Header("Projectile")]
        public ProjectileDefinition projectileDefinition;
        
        [Header("Unlock")]
        public int requiredLevel = 1;
    }
}