using System;
using _Project.Scripts.Player;
using _Project.Scripts.Weapon;
using UnityEngine;

namespace _Project.UI 
{
    public class PlayerStatsUI : MonoBehaviour
    {
        [SerializeField] private HealthBarUI healthBar;

        [SerializeField] private StatRowUI attackRow;
        [SerializeField] private StatRowUI fireRateRow;
        [SerializeField] private StatRowUI reloadSpeedRow;
        [SerializeField] private ReloadBarView reloadBarView;
        [SerializeField] private AmmoCounterUI ammoUI;

        private Player player;

        public void Initialize(Player newPlayer)
        {
            // если уже был игрок — отписываемся
            if (player != null)
            {
                player.Stats.OnStatsChanged -= RefreshStats;
                player.Stats.Health.OnHealthChanged -= RefreshHealth;
                player.Combat.OnWeaponChanged -= OnWeaponChanged;
            }

            player = newPlayer;
            if (player == null) return;
            
            player.Combat.OnWeaponChanged += OnWeaponChanged;
            player.Stats.OnStatsChanged += RefreshStats;
            player.Stats.Health.OnHealthChanged += RefreshHealth;

            RefreshStats();
            RefreshHealth(player.Stats.Health.CurrentHealth, player.Stats.Health.MaxHealth);
            
            BindWeapon();
        }
        
        private void OnWeaponChanged(Weapon weapon)
        {
            reloadBarView.Bind(weapon as RangedWeapon);
            if (weapon is RangedWeapon ranged)
                ammoUI.Bind(ranged);
            else
                ammoUI.Bind(null);
            RefreshStats();
        }

        private void RefreshHealth(int currentHealth, int maxHealth)
        {
            if (healthBar != null)
                healthBar.UpdateHealth(currentHealth, maxHealth);
        }
        
        private void BindWeapon()
        {
            if (player == null)
                return;

            var rangedWeapon = player.Combat.ActiveWeapon as RangedWeapon;
            if (rangedWeapon == null)
                return;

            reloadBarView.Bind(rangedWeapon);
        }

        private void OnDestroy()
        {
            if (player == null) return;

            player.Stats.OnStatsChanged -= RefreshStats;
            player.Stats.Health.OnHealthChanged -= RefreshHealth;
            player.Combat.OnWeaponChanged -= OnWeaponChanged;
        }

        private void RefreshStats()
        {
            if (player == null) return;

            var stats = player.Stats;

            var weaponDmg = 1;
            float reloadSpeed = 1;
            float fireRate = 1;
            try
            {
                weaponDmg = player.Combat.ActiveWeapon.definition.damage;
                reloadSpeed = player.Combat.ActiveWeapon.definition.reloadTime;
                fireRate = player.Combat.ActiveWeapon.definition.fireRate;
            }
            catch (Exception e)
            {
               
            }

            // TODO потом сделаем реальные базовые значения
            attackRow.SetData("Урон",  weaponDmg * stats.DamageMultiplier, stats.DamageMultiplier);
            fireRateRow.SetData("Скорость перезарядки", reloadSpeed * stats.ReloadSpeedMultiplier, stats.ReloadSpeedMultiplier);
            reloadSpeedRow.SetData("Скорострельность", fireRate * stats.FireRateMultiplier, stats.FireRateMultiplier);
        }
    }
}