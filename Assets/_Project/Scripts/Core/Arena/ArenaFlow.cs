using UnityEngine;
using UnityEngine.SceneManagement;
using _Project.Scripts.Core.Run;
using _Project.Scripts.Core.World;
using _Project.Scripts.Waves;

namespace _Project.Scripts.Core.Arena
{
    public sealed class ArenaFlow : MonoBehaviour
    {
        [Header("Scene refs")]
        [SerializeField] private DoorTrigger rightDoor;
        [SerializeField] private CardChoiceStub cardChoice; // может быть null на BossArena
        [SerializeField] private WaveManager waveManager;

        private GameSession session;
        private RunDefinition runDef;
        private ArenaDefinition arenaDef;

        private bool isBossScene;
        private int pendingWaveIndex;

        private void Awake()
        {
            if (rightDoor == null)
                Debug.LogError("[ArenaFlow] rightDoor is not assigned.", this);

            if (waveManager == null)
                waveManager = FindFirstObjectByType<WaveManager>();

            if (waveManager == null)
                Debug.LogError("[ArenaFlow] WaveManager not found in scene.", this);
        }

        private void OnEnable()
        {
            if (waveManager != null)
                waveManager.WaveCleared += OnWaveCleared;
        }

        private void OnDisable()
        {
            if (waveManager != null)
                waveManager.WaveCleared -= OnWaveCleared;
        }

        private void Start()
        {
            if (rightDoor == null || waveManager == null)
            {
                enabled = false;
                return;
            }

            if (App.Instance?.Services == null)
            {
                Debug.LogError("[ArenaFlow] App/Services not ready.", this);
                enabled = false;
                return;
            }

            session = App.Instance.Services.Get<GameSession>();
            runDef = App.Instance.Services.Get<RunDefinition>();

            if (session == null)
            {
                Debug.LogError("[ArenaFlow] GameSession not found in services.", this);
                enabled = false;
                return;
            }

            if (runDef == null)
            {
                Debug.LogError("[ArenaFlow] RunDefinition not found in services. Assign it in App and register in Services.", this);
                enabled = false;
                return;
            }

            string sceneName = SceneManager.GetActiveScene().name;
            isBossScene = sceneName == SceneNames.BossArena;

            if (isBossScene)
            {
                arenaDef = runDef.bossArena;
            }
            else
            {
                if (!runDef.TryGetArena(session.ArenaIndex, out arenaDef))
                {
                    Debug.LogError($"[ArenaFlow] No ArenaDefinition for ArenaIndex={session.ArenaIndex}. Check RunDefinition.arenas.", this);
                    enabled = false;
                    return;
                }
            }

            if (arenaDef == null)
            {
                Debug.LogError("[ArenaFlow] ArenaDefinition is null.", this);
                enabled = false;
                return;
            }

            // Настроить волны текущей арены
            waveManager.SetWaves(arenaDef.waves);

            // Дверь закрыта пока не пройдены все волны этой арены
            rightDoor.SetLocked(true);

            // Перед первой волной: 0 -> next = 1
            session.SetWave(0);

            StartNextWave();
        }

        private void StartNextWave()
        {
            int nextWave = session.WaveIndex + 1;

            // Волн больше нет -> открыть дверь
            if (!waveManager.HasWave(nextWave))
            {
                rightDoor.SetLocked(false);
                return;
            }

            bool useCardsNow =
                !isBossScene &&
                cardChoice != null &&
                ((nextWave == 1 && arenaDef.showCardChoiceBeforeFirstWave) ||
                 (nextWave > 1 && arenaDef.showCardChoiceBetweenWaves));

            if (useCardsNow)
            {
                pendingWaveIndex = nextWave;

                // Вариант без передачи CardData (полностью совместим с текущим Stub)
                cardChoice.Open(OnCardChoiceClosed);
            }
            else
            {
                StartWave(nextWave);
            }
        }

        private void OnCardChoiceClosed()
        {
            // Карточку выбрали (и UI закрылся) -> стартуем волну
            StartWave(pendingWaveIndex);
        }

        private void StartWave(int waveIndex)
        {
            session.SetWave(waveIndex);
            waveManager.StartWave(waveIndex);
        }

        private void OnWaveCleared()
        {
            // Следующей волны нет -> открыть дверь
            if (!waveManager.HasWave(session.WaveIndex + 1))
            {
                rightDoor.SetLocked(false);
                return;
            }

            StartNextWave();
        }

        // Debug: можно дернуть кнопкой чтобы перейти к следующему шагу
        public void DebugCompleteWave()
        {
            OnWaveCleared();
        }
    }
}
