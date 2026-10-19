using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Player
{
    public sealed class PlayerRuntimeSpawner : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private GameObject playerPrefab;

        [Header("Behaviour")]
        [SerializeField] private bool keepPlayerBetweenScenes = true;

        private GameObject playerInstance;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Start()
        {
            // На случай, если сцена уже загружена и sceneLoaded не сработает как ожидаешь
            TrySpawnOrMoveForActiveScene();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            TrySpawnOrMoveForActiveScene();
        }

        private void TrySpawnOrMoveForActiveScene()
        {
            var spawnPoint = FindFirstObjectByType<PlayerSpawnPoint>();
            if (spawnPoint == null)
            {
                // В этой сцене игрок не нужен (меню/настройки) — ничего не делаем.
                return;
            }

            if (playerPrefab == null)
            {
                Debug.LogError("[PlayerRuntimeSpawner] playerPrefab is null. Assign it in Inspector.", this);
                return;
            }

            if (playerInstance == null)
            {
                playerInstance = Instantiate(playerPrefab);
                playerInstance.name = "Player";

                // очень важно: чтобы DoorTrigger и WaveManager находили игрока
                playerInstance.tag = "Player";

                if (keepPlayerBetweenScenes)
                    DontDestroyOnLoad(playerInstance);
            }

            // Переносим игрока в точку спавна
            playerInstance.transform.SetPositionAndRotation(spawnPoint.transform.position, spawnPoint.transform.rotation);

            // Сбросить физику, чтобы не “улетал” при телепорте
            var rb = playerInstance.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;
        }
    }
}
