using System;
using UnityEngine;

namespace _Project.Scripts.Core.Actor
{
    [System.Serializable]
    public class Stats
    {
        [Header("Health")]
        [SerializeField] private Health health;
        [Header("Multipliers")]
        [SerializeField] private float fireRateMultiplier = 1f;
        [SerializeField] private float reloadSpeedMultiplier = 1f;
        [SerializeField] private float damageMultiplier = 1f;
        [SerializeField] private float moveSpeedMultiplier = 1f;
        
        public event Action OnStatsChanged;

        public Health Health => health;

        public float FireRateMultiplier => fireRateMultiplier;
        public float ReloadSpeedMultiplier => reloadSpeedMultiplier;
        public float DamageMultiplier => damageMultiplier;
        public float MoveSpeedMultiplier => moveSpeedMultiplier;

        public void Initialize()
        {
            if (health == null)
            {
                Debug.LogError("[Stats] Health is null!");
                return;
            }
            health.Initialize();
        }

        // изменение множителей

        public void ModifyDamageMultiplier(float value)
        {
            damageMultiplier += value;
            OnStatsChanged?.Invoke();
        }

        public void ModifyFireRateMultiplier(float value)
        {
            fireRateMultiplier += value;
            OnStatsChanged?.Invoke();
        }

        public void ModifyReloadSpeedMultiplier(float value)
        {
            reloadSpeedMultiplier += value;
            OnStatsChanged?.Invoke();
        }
        
        public void ModifyMoveSpeedMultiplier(float value)
        {
            moveSpeedMultiplier += value;
            OnStatsChanged?.Invoke();
        }
    }
}