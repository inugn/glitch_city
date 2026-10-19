using UnityEngine;

namespace _Project.Scripts.Enemies.Core
{
    public enum EnemySubtype { Warrior, Shooter, Turret, Kamikaze }
    public enum MovementType { GroundPatrol, FlyingPatrol, Static }
    public enum AttackType { Melee, Ranged, Explode, None }

    [CreateAssetMenu(menuName = "_Project/Enemies/Enemy Definition", fileName = "EnemyDef_")]
    public sealed class EnemyDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string enemyId = "enemy";
        public EnemySubtype subtype = EnemySubtype.Warrior;

        [Header("Core Stats")]
        public int maxHp = 10;
        public float moveSpeed = 3f;

        [Header("Detection")]
        public float detectRadius = 6f;
        public float chaseRadius = 10f;

        [Header("Movement / Attack")]
        public MovementType movement = MovementType.GroundPatrol;
        public AttackType attack = AttackType.Melee;

        [Header("Melee")]
        public float meleeRange = 1.2f;
        public int meleeDamage = 5;
        public float meleeCooldown = 1.0f;

        [Header("Ranged")]
        public int rangedDamage = 3;
        public float rangedCooldown = 0.8f;
        public float projectileSpeed = 12f;
        public float accuracyDeg = 10f; // 15..1 deg

        [Header("Explode")]
        public int explodeDamage = 10;
        public float explodeRadius = 1.5f;
        
        [Header("AI Tuning")]
        [Tooltip("Задержка перед переходом в преследование/атаку, чтобы не дёргалось (сек).")]
        public float stateRecheckInterval = 0.1f;

        [Header("Ranged AI")]
        public float rangedMaxRange = 8f;              // максимальная дистанция стрельбы
        public float rangedPreferredDistance = 4f;     // “хочу стоять вот так далеко”
        public float rangedDistanceTolerance = 0.6f;   // “окно”, чтобы не дёргался
        public float rangedVerticalOffset = 0.8f;      // для летающих: держаться чуть выше игрока
        

    }
}