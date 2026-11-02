using _Project.Scripts.Weapon;
using UnityEngine;

namespace _Project.Scripts.Core.Item
{
    [System.Serializable]
    public class WeaponSpawnEntry
    {
        public WeaponDefinition definition;
        public Transform spawnPoint;
    }
}