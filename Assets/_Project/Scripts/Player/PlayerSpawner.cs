using _Project.UI;
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
            Player player = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity).GetComponent<Player>();
            UIManager.Instance.RegisterPlayer(player);
        }
    }
}