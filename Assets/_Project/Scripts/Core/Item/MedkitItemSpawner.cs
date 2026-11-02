using UnityEngine;

namespace _Project.Scripts.Core.Item
{
    public sealed class MedkitItemSpawner : MonoBehaviour
    {
        [SerializeField] private MedkitItem itemPrefab;
        [SerializeField] private MedkitSpawnEntry[] medkits;

        private void Start()
        {
            foreach (var entry in medkits)
                Spawn(entry);
        }

        private void Spawn(MedkitSpawnEntry entry)
        {
            if (entry.spawnPoint == null || entry.definition == null)
                return;

            var item = Instantiate(
                itemPrefab,
                entry.spawnPoint.position,
                entry.spawnPoint.rotation
            );

            item.name = entry.definition.medkitId;
        }
    }
}