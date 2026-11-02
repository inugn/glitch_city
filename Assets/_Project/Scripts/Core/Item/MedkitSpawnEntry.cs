using UnityEngine;

namespace _Project.Scripts.Core.Item
{
    [System.Serializable]
    public sealed class MedkitSpawnEntry
    {
        public MedkitDefinition definition;
        public Transform spawnPoint;
    }
}