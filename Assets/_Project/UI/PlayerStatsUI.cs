using _Project.Scripts.Player;
using UnityEngine;

namespace _Project.UI 
{
    public class PlayerStatsUI : MonoBehaviour
    {
        [SerializeField] private HealthBarUI healthBar;

        [SerializeField] private StatRowUI attackRow;
        [SerializeField] private StatRowUI fireRateRow;
        [SerializeField] private StatRowUI reloadSpeedRow;

        private Player player;

        public void Initialize(Player player)
        {
            this.player = player;
            RefreshStats();
        }

        private void Update()
        {
            if (player == null) return;

            healthBar.UpdateHealth(
                player.Stats.Health.CurrentHealth,
                player.Stats.Health.MaxHealth
            );
        }

        private void RefreshStats()
        {
            var stats = player.Stats;

            // TODO сделать получение значений характеристик
            attackRow.SetData("Атака", 12 * stats.DamageMultiplier, stats.DamageMultiplier);
            fireRateRow.SetData("Скорость перезарядки", 12 * stats.FireRateMultiplier, stats.FireRateMultiplier);
            reloadSpeedRow.SetData("Скорострельность", 12 * stats.ReloadSpeedMultiplier, stats.ReloadSpeedMultiplier);
        }
    }
}