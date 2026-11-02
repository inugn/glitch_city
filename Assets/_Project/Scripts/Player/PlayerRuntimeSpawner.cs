using _Project.Scripts.Core;
using _Project.Scripts.Weapon;
using _Project.UI;
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
        
        [SerializeField] private WeaponDefinition testWeaponDefinition;


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
                App.Instance.Services.Get<GameSession>().SetPlayer(playerInstance.GetComponent<Player>());
                UIManager.Instance.RegisterPlayer(playerInstance.GetComponent<Player>());
                // ВРЕМЕННО для отладки
                //playerInstance.GetComponent<Player>().Combat.Equip(new RangedWeapon(testWeaponDefinition), testWeaponDefinition);
                //
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
            
            var player = playerInstance.GetComponent<_Project.Scripts.Player.Player>();
            if (player == null)
            {
                Debug.LogError("[PlayerRuntimeSpawner] Spawned object has no Player component.", playerInstance);
                return;
            }

            // 1) GameSession должен знать текущего игрока (если ты это используешь где-то)
            var session = App.Instance?.Services?.Get<GameSession>();
            session?.SetPlayer(player);

            // 2) UI должен привязаться к игроку
            if (UIManager.Instance != null)
                UIManager.Instance.RegisterPlayer(player);
            else
                Debug.LogWarning("[PlayerRuntimeSpawner] UIManager.Instance is null (HUD not created yet?)");

            Debug.Log($"[Spawn] Player={player.name}, UI={(UIManager.Instance!=null)}, PlayerStatsUI={(UIManager.Instance!=null)}");

        }
    }
}
