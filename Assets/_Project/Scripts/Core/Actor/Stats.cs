using UnityEngine;

namespace _Project.Scripts.Core.Actor
{
    public class Stats
    {
        [SerializeField] private Health health;

        [SerializeField] private float fireRateMultiplier = 1f;
        [SerializeField] private float reloadSpeedMultiplier = 1f;
        [SerializeField] private float damageMultiplier = 1f;

        public Health Health => health;

        public float FireRateMultiplier => fireRateMultiplier;
        public float ReloadSpeedMultiplier => reloadSpeedMultiplier;
        public float DamageMultiplier => damageMultiplier;

        public void Initialize()
        {
            if (health == null)
                health = new Health();

            health.Initialize();
        }

        // изменение множителей

        public void ModifyDamageMultiplier(float value)
        {
            damageMultiplier += value;
        }

        public void ModifyFireRateMultiplier(float value)
        {
            fireRateMultiplier += value;
        }

        public void ModifyReloadSpeedMultiplier(float value)
        {
            reloadSpeedMultiplier += value;
        }

    }
}