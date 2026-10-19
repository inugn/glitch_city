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
        [SerializeField] private CardChoiceStub cardChoice;
        [SerializeField] private WaveManager waveManager;

        private GameSession session;
        private RunDefinition runDef;
        private ArenaDefinition arenaDef;

        private bool isBossScene;

        private void Awake()
        {
            if (rightDoor == null)
                Debug.LogError("[ArenaFlow] rightDoor is not assigned.", this);

            if (waveManager == null)
                waveManager = FindFirstObjectByType<WaveManager>();

            // cardChoice может быть null для BossArena (если без карт)
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

            if (session == null || runDef == null)
            {
                Debug.LogError("[ArenaFlow] GameSession or RunDefinition missing in services.", this);
                enabled = false;
                return;
            }

            var sceneName = SceneManager.GetActiveScene().name;
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

            // “перед первой волной”
            session.SetWave(0);

            StartNextWave();
        }

        private void StartNextWave()
        {
            int nextWave = session.WaveIndex + 1;

            if (!waveManager.HasWave(nextWave))
            {
                // волн больше нет
                rightDoor.SetLocked(false);
                return;
            }

            // BossArena или арена без карточек — стартуем сразу
            bool useCardsNow =
                cardChoice != null &&
                ((nextWave == 1 && arenaDef.showCardChoiceBeforeFirstWave) ||
                 (nextWave > 1 && arenaDef.showCardChoiceBetweenWaves));

            if (useCardsNow)
            {
                cardChoice.Open(() => StartWave(nextWave));
            }
            else
            {
                StartWave(nextWave);
            }
        }

        private void StartWave(int waveIndex)
        {
            session.SetWave(waveIndex);
            waveManager.StartWave(waveIndex);
        }

        private void OnWaveCleared()
        {
            // Если следующей волны нет — открыть дверь
            if (!waveManager.HasWave(session.WaveIndex + 1))
            {
                rightDoor.SetLocked(false);
                return;
            }

            // иначе — следующая волна
            StartNextWave();
        }
    }
}
