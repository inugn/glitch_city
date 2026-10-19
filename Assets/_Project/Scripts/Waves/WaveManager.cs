using System;
using System.Collections;
using UnityEngine;
using _Project.Scripts.Enemies.Core;

namespace _Project.Scripts.Waves
{
    public sealed class WaveManager : MonoBehaviour
    {
        [SerializeField] private EnemySpawner spawner;

        [Header("Wave sequence (1..N)")]
        [SerializeField] private WaveDefinition[] waves = Array.Empty<WaveDefinition>();

        [Header("Player (optional). If empty, will auto-resolve by tag 'Player'.")]
        [SerializeField] private Transform player;

        public int TotalWaves => waves != null ? waves.Length : 0;
        public int CurrentWaveIndex { get; private set; } // 1-based, 0 = none

        public event Action WaveCleared;

        private readonly EnemyBase[] aliveBuffer = new EnemyBase[256]; // MVP safety buffer
        private int aliveCount;

        private Coroutine spawnRoutine;
        private bool spawning;

        public void SetPlayer(Transform playerTransform) => player = playerTransform;

        public void SetWaves(WaveDefinition[] newWaves)
        {
            // безопасно остановить текущую волну/подписки
            StopAllCoroutines();
            CurrentWaveIndex = 0;

            // подчистить alive/подписки — если у тебя есть StopWaveInternal(), вызови его вместо StopAllCoroutines()
            // (если ты вставляла мой WaveManager ранее — там есть StopWaveInternal())

            waves = newWaves ?? Array.Empty<WaveDefinition>();
        }
        
        public bool HasWave(int waveIndex)
        {
            if (waves == null) return false;
            int idx = waveIndex - 1;
            return idx >= 0 && idx < waves.Length && waves[idx] != null;
        }

        public void StartWave(int waveIndex)
        {
            if (spawner == null)
            {
                Debug.LogError("[WaveManager] Spawner is null.", this);
                return;
            }

            if (!EnsurePlayerResolved())
            {
                Debug.LogError("[WaveManager] Player not resolved. Ensure scene has Player with tag 'Player'.", this);
                return;
            }

            if (!HasWave(waveIndex))
            {
                Debug.LogError($"[WaveManager] Wave index {waveIndex} is invalid or WaveDefinition is null.", this);
                return;
            }

            StopWaveInternal();

            CurrentWaveIndex = waveIndex;
            spawnRoutine = StartCoroutine(SpawnWaveRoutine(waves[waveIndex - 1]));
        }

        private void StopWaveInternal()
        {
            if (spawnRoutine != null)
            {
                StopCoroutine(spawnRoutine);
                spawnRoutine = null;
            }

            spawning = false;

            // Отписываемся от всех живых, чтобы не текли подписки при рестарте.
            for (int i = 0; i < aliveCount; i++)
            {
                var e = aliveBuffer[i];
                if (e != null)
                    e.Died -= OnEnemyDied;
                aliveBuffer[i] = null;
            }
            aliveCount = 0;
        }

        private IEnumerator SpawnWaveRoutine(WaveDefinition def)
        {
            spawning = true;

            // Delay без WaitForSeconds (меньше мусора)
            if (def.startDelay > 0f)
            {
                float t = 0f;
                while (t < def.startDelay)
                {
                    t += Time.deltaTime;
                    yield return null;
                }
            }

            var spawns = def.spawns;
            if (spawns == null || spawns.Length == 0)
            {
                spawning = false;
                TryCompleteIfFinished();
                yield break;
            }

            for (int i = 0; i < spawns.Length; i++)
            {
                var entry = spawns[i];
                if (entry.enemy == null) continue;

                int count = Mathf.Max(1, entry.count);
                float interval = Mathf.Max(0f, entry.spawnInterval);

                for (int j = 0; j < count; j++)
                {
                    var e = spawner.Spawn(entry.enemy, player);
                    if (e != null)
                    {
                        if (aliveCount < aliveBuffer.Length)
                        {
                            aliveBuffer[aliveCount++] = e;
                            e.Died += OnEnemyDied;
                        }
                        else
                        {
                            Debug.LogError("[WaveManager] Alive buffer overflow. Increase buffer size or reduce spawn count.", this);
                        }
                    }

                    if (interval > 0f)
                    {
                        float tt = 0f;
                        while (tt < interval)
                        {
                            tt += Time.deltaTime;
                            yield return null;
                        }
                    }
                }
            }

            spawning = false;
            TryCompleteIfFinished();
        }

        private void OnEnemyDied(EnemyBase e)
        {
            if (e != null)
                e.Died -= OnEnemyDied;

            // Удаляем из буфера swap-last (O(1))
            for (int i = 0; i < aliveCount; i++)
            {
                if (aliveBuffer[i] != e) continue;
                int last = aliveCount - 1;
                aliveBuffer[i] = aliveBuffer[last];
                aliveBuffer[last] = null;
                aliveCount--;
                break;
            }

            TryCompleteIfFinished();
        }

        private void TryCompleteIfFinished()
        {
            if (spawning) return;
            if (aliveCount > 0) return;

            WaveCleared?.Invoke();
        }

        private bool EnsurePlayerResolved()
        {
            if (IsValidSceneTransform(player))
                return true;

            player = null;

            var go = GameObject.FindGameObjectWithTag("Player");
            if (go == null) return false;

            player = go.transform;
            return true;
        }

        private static bool IsValidSceneTransform(Transform t)
        {
            return t != null && t.gameObject != null && t.gameObject.scene.IsValid() && t.gameObject.activeInHierarchy;
        }

        private void OnDisable()
        {
            // если сцену выгружаем — подчистим
            StopWaveInternal();
        }
    }
}
