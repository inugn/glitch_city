using System.Collections.Generic;

namespace _Project.Scripts.Core
{
    public sealed class MetaProgress : IGameService
    {
        public int PlayerLevel { get; private set; } = 1;
        public int CurrentXp { get; private set; } = 0;

        // Заглушка под “открыто оружие”
        private readonly HashSet<string> unlockedWeapons = new();

        public void AddXp(int amount)
        {
            if (amount <= 0) return;
            CurrentXp += amount;
        }

        public void LevelUp()
        {
            PlayerLevel++;
            CurrentXp = 0;
        }

        public bool IsWeaponUnlocked(string weaponId) => unlockedWeapons.Contains(weaponId);

        public void UnlockWeapon(string weaponId)
        {
            if (!string.IsNullOrWhiteSpace(weaponId))
                unlockedWeapons.Add(weaponId);
        }
    }
}