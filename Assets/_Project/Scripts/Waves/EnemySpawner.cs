using UnityEngine;
using _Project.Scripts.Enemies.Core;

namespace _Project.Scripts.Waves
{
    public sealed class EnemySpawner : MonoBehaviour
    {
        [System.Serializable]
        private struct EnemyPrefabEntry
        {
            public EnemyDefinition Definition;
            public EnemyBase Prefab;
        }

        [SerializeField] private Transform[] spawnPoints;

        [Header("Fallback prefab (used if no match)")]
        [SerializeField] private EnemyBase defaultPrefab;

        [Header("Per-definition prefabs (optional)")]
        [SerializeField] private EnemyPrefabEntry[] prefabsByDefinition;
        
        private const float WorldZ = 0f;

        public EnemyBase Spawn(EnemyDefinition def, Transform player)
        {
            
            if (def == null || player == null)
                return null;

            if (!player.gameObject.scene.IsValid())
            {
                Debug.LogError("[EnemySpawner] Player transform is not a scene instance (prefab reference?).", this);
                return null;
            }
            
            if (def == null || player == null)
                return null;

            var prefab = ResolvePrefab(def);
            if (prefab == null)
            {
                Debug.LogError($"[EnemySpawner] No prefab mapped for '{def.name}'.", this);
                return null;
            }

            var pos = PickSpawnPoint();
            pos.z = WorldZ; 

            var enemy = Instantiate(prefab, pos, Quaternion.identity);

            enemy.Init(def, player);

            var ai = enemy.GetComponent<EnemyAIController>();
            if (ai != null) ai.ConfigureFromDefinition();

            return enemy;
        }

        private EnemyBase ResolvePrefab(EnemyDefinition def)
        {
            if (prefabsByDefinition != null)
            {
                for (int i = 0; i < prefabsByDefinition.Length; i++)
                {
                    if (prefabsByDefinition[i].Definition == def && prefabsByDefinition[i].Prefab != null)
                        return prefabsByDefinition[i].Prefab;
                }
            }

            return defaultPrefab;
        }

        private Vector3 PickSpawnPoint()
        {
            if (spawnPoints == null || spawnPoints.Length == 0)
                return transform.position;

            return spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        }
    }
}
