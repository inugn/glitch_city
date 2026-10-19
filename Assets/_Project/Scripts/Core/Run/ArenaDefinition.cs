using UnityEngine;
using _Project.Scripts.Waves;

namespace _Project.Scripts.Core.Run
{
    [CreateAssetMenu(menuName = "_Project/Run/Arena Definition", fileName = "ArenaDef_")]
    public sealed class ArenaDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string arenaId = "arena_01";

        [Header("Waves (1..N)")]
        public WaveDefinition[] waves;

        [Header("Cards")]
        public bool showCardChoiceBeforeFirstWave = true;
        public bool showCardChoiceBetweenWaves = true;
    }
}