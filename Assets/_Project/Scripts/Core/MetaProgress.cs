using System;
using System.Collections.Generic;
using _Project.Scripts.Core.Save;

namespace _Project.Scripts.Core
{
    public sealed class MetaProgress : IGameService
    {
        public int PlayerLevel { get; private set; } = 1;
        public int CurrentXp { get; private set; } = 0;

        private readonly HashSet<string> unlockedWeapons = new();
        
        public int XpToNextLevel => GetXpToNextLevel(PlayerLevel);

        public bool IsDirty { get; private set; }

        public event Action Changed;

        private static int GetXpToNextLevel(int level)
        {
            level = Math.Max(1, level);
            return 100 * level;
            
        }
        public void AddXp(int amount)
        {
            if (amount <= 0) return;

            CurrentXp += amount;

            // Multi-level up без ошибок с изменяющимся порогом
            while (true)
            {
                int need = GetXpToNextLevel(PlayerLevel);
                if (CurrentXp < need)
                    break;

                CurrentXp -= need;
                PlayerLevel++;
            }

            MarkDirty();
        }

        public void LevelUp()
        {
            PlayerLevel++;
            CurrentXp = 0;
            MarkDirty();
        }

        public bool IsWeaponUnlocked(string weaponId) => unlockedWeapons.Contains(weaponId);

        public void UnlockWeapon(string weaponId)
        {
            if (!string.IsNullOrWhiteSpace(weaponId))
                unlockedWeapons.Add(weaponId);
        }

        public MetaProgressSaveData CreateSnapshot()
        {
            var data = new MetaProgressSaveData
            {
                PlayerLevel = PlayerLevel,
                CurrentXp = CurrentXp,
                LastUpdatedUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            data.UnlockedWeapons.Clear();
            foreach (var w in unlockedWeapons)
                data.UnlockedWeapons.Add(w);

            return data;
        }

        public void ApplySnapshot(MetaProgressSaveData data)
        {
            if (data == null) return;

            PlayerLevel = Math.Max(1, data.PlayerLevel);
            CurrentXp = Math.Max(0, data.CurrentXp);

            unlockedWeapons.Clear();
            if (data.UnlockedWeapons != null)
            {
                for (int i = 0; i < data.UnlockedWeapons.Count; i++)
                {
                    var id = data.UnlockedWeapons[i];
                    if (!string.IsNullOrWhiteSpace(id))
                        unlockedWeapons.Add(id);
                }
            }

            IsDirty = false;
            Changed?.Invoke();
        }

        public void ClearDirty() => IsDirty = false;

        private void MarkDirty()
        {
            IsDirty = true;
            Changed?.Invoke();
        }
    }
}
