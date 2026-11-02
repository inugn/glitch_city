using _Project.Scripts.Items;
using _Project.Scripts.Weapon;
using UnityEngine;

namespace _Project.Scripts.Core.Item
{
    public class WeaponItemSpawner : MonoBehaviour
    {
        [SerializeField] private WeaponItem itemPrefab;
        [SerializeField] private WeaponSpawnEntry[] weapons;

        private void Start()
        {
            foreach (var entry in weapons)
            {
                Spawn(entry);
            }
        }

        private void Spawn(WeaponSpawnEntry entry)
        {
            var item = Instantiate(
                itemPrefab,
                entry.spawnPoint.position,
                entry.spawnPoint.rotation
            );
            
            if (item == null || entry.definition == null)
                return;

            item.SetDefinition(entry.definition);
        }
    }
}