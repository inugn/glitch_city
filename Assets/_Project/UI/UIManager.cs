using _Project.Scripts.Core.Actor;
using UnityEngine;
using UnityEngine.SceneManagement;
using _Project.Scripts.Player;
using _Project.Scripts.Core.Arena;

namespace _Project.UI
{
    public sealed class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("HUD Prefab (Canvas root)")]
        [SerializeField] private GameObject hudPrefab;

        private GameObject hudInstance;
        private DeathScreenView deathScreenView;
        private BossHealthPresenter bossHealthPresenter;
        private PauseScreenView pauseScreenView;
        public PlayerStatsUI PlayerStatsUI { get; private set; }
        public CardChoiceStub CardChoice { get; private set; }

        private Player pendingPlayer;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            EnsureHud();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void ShowDeathScreen()
        {
            if (deathScreenView != null)
                deathScreenView.Show();
        }
        
        public void RegisterBoss(Stats bossStats)
        {
            bossHealthPresenter.Bind(bossStats);
        }

        public void UnregisterBoss()
        {
            bossHealthPresenter.Unbind();
        }
        
        private void OnDestroy()
        {
            if (Instance == this)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
                Instance = null;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (hudInstance == null) EnsureHud();
            else if (PlayerStatsUI == null || CardChoice == null) ResolveHudRefs();

            TryBindPendingPlayer();
        }

        private void EnsureHud()
        {
            if (hudInstance != null) return;

            if (hudPrefab == null)
            {
                Debug.LogError("[UIManager] hudPrefab is not assigned.", this);
                return;
            }

            hudInstance = Instantiate(hudPrefab);
            hudInstance.name = hudPrefab.name;
            DontDestroyOnLoad(hudInstance);

            if (!hudInstance.activeSelf)
                hudInstance.SetActive(true);
            
            deathScreenView = hudInstance.GetComponentInChildren<DeathScreenView>(true);
            pauseScreenView = hudInstance.GetComponentInChildren<PauseScreenView>(true);
            bossHealthPresenter = hudInstance.GetComponentInChildren<BossHealthPresenter>(true);
            
            ResolveHudRefs();
            TryBindPendingPlayer();
        }

        private void ResolveHudRefs()
        {
            PlayerStatsUI = hudInstance != null
                ? hudInstance.GetComponentInChildren<PlayerStatsUI>(true)
                : null;

            CardChoice = hudInstance != null
                ? hudInstance.GetComponentInChildren<CardChoiceStub>(true)
                : null;

            if (PlayerStatsUI == null)
                Debug.LogError("[UIManager] PlayerStatsUI not found inside HUD prefab.", this);

            if (CardChoice == null)
                Debug.LogWarning("[UIManager] CardChoiceStub not found inside HUD prefab.", this);
        }

        public void RegisterPlayer(Player player)
        {
            pendingPlayer = player;
            TryBindPendingPlayer();
        }

        private void TryBindPendingPlayer()
        {
            if (pendingPlayer == null) return;
            if (PlayerStatsUI == null) return;

            PlayerStatsUI.Initialize(pendingPlayer);
            pendingPlayer = null;
        }
    }
}
