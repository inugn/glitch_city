using UnityEngine;

namespace _Project.Scripts.Player
{
    public sealed class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform spawnPoint;

        private void Start()
        {
            if (playerPrefab == null || spawnPoint == null) return;

            // Если игрок уже есть (например, DontDestroy), можно усложнить позже.
            Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}