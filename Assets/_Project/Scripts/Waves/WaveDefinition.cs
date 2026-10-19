using System;
using UnityEngine;
using _Project.Scripts.Enemies.Core;

namespace _Project.Scripts.Waves
{
    [CreateAssetMenu(menuName = "_Project/Waves/Wave Definition", fileName = "WaveDef_")]
    public sealed class WaveDefinition : ScriptableObject
    {
        [Header("Timing")]
        [Min(0f)] public float startDelay = 0f;

        [Header("Spawns")]
        public SpawnEntry[] spawns = Array.Empty<SpawnEntry>();

        [Serializable]
        public struct SpawnEntry
        {
            public EnemyDefinition enemy;
            [Min(1)] public int count;
            [Min(0f)] public float spawnInterval; // 0 = сразу пачкой
        }
    }
}