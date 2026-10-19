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
            player.Stats.OnStatsChanged += RefreshStats;
            player.Stats.Health.OnHealthChanged += RefreshHealth;
            RefreshStats();
            RefreshHealth(player.Stats.Health.CurrentHealth, player.Stats.Health.MaxHealth);
        }
        
        private void RefreshHealth(int currentHealth, int maxHealth)
        {
            healthBar.UpdateHealth(currentHealth, maxHealth);
        }
        
        private void OnDestroy()
        {
            if (player == null) return;

            player.Stats.OnStatsChanged -= RefreshStats;
            player.Stats.Health.OnHealthChanged -= RefreshHealth;
        }

        private void RefreshStats()
        {
            var stats = player.Stats;

            // TODO сделать получение значений характеристик
            attackRow.SetData("Урон", 12 * stats.DamageMultiplier, stats.DamageMultiplier);
            fireRateRow.SetData("Скорость перезарядки", 12 * stats.ReloadSpeedMultiplier, stats.ReloadSpeedMultiplier);
            reloadSpeedRow.SetData("Скорострельность", 12 * stats.FireRateMultiplier, stats.FireRateMultiplier);
        }
    }
}