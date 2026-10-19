using UnityEngine;

namespace _Project.Scripts.Core.Run
{
    [CreateAssetMenu(menuName = "_Project/Run/Run Definition", fileName = "RunDef_")]
    public sealed class RunDefinition : ScriptableObject
    {
        [Header("Arenas before boss (MVP: 3)")]
        public ArenaDefinition[] arenas;

        [Header("Boss arena (one wave, one boss)")]
        public ArenaDefinition bossArena;

        public int ArenasCount => arenas != null ? arenas.Length : 0;

        public bool TryGetArena(int arenaIndex1Based, out ArenaDefinition arena)
        {
            arena = null;
            if (arenas == null) return false;

            int idx = arenaIndex1Based - 1;
            if (idx < 0 || idx >= arenas.Length) return false;

            arena = arenas[idx];
            return arena != null;
        }
    }
}