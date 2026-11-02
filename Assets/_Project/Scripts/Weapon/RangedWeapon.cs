using System;
using _Project.Scripts.Core;
using _Project.Scripts.Core.Audio;
using UnityEngine;

namespace _Project.Scripts.Weapon
{
    public class RangedWeapon : Weapon
    {
        public event Action OnShot;
        public event Action OnReloadStarted;
        public event Action OnReloadFinished;
        public event Action<int> OnAmmoChanged;
        
        protected int ammoInMagazine;
        protected float fireCooldown;
        protected bool isReloading;
        public int CurrentAmmo => ammoInMagazine;
        
        private float reloadTimer;

        public override bool CanAttack =>
            !isReloading &&
            ammoInMagazine > 0 &&
            fireCooldown <= 0f;

        public bool IsReloading => isReloading;

        protected internal RangedWeapon(WeaponDefinition definition)
        {
            this.definition = definition;
            ammoInMagazine = definition.magazineSize;
        }

        public override void Tick(float deltaTime)
        {
            if (fireCooldown > 0f)
                fireCooldown -= deltaTime;

            if (isReloading)
            {
                float speedMultiplier = OwnerStats != null ? OwnerStats.ReloadSpeedMultiplier : 1f;
                reloadTimer -= deltaTime * speedMultiplier;
                if (reloadTimer <= 0f)
                    FinishReload();
            }
        }

        public override void Attack()
        {
            if (isReloading)
                return;

            if (ammoInMagazine <= 0)
            {
                Reload();
                return;
            }

            if (fireCooldown > 0f)
                return;

            PerformShot();
            
            if (ammoInMagazine <= 0)
                Reload();
        }

        public virtual void Reload()
        {
            if (isReloading)
                return;

            isReloading = true;
            float speedMultiplier = OwnerStats != null ? OwnerStats.ReloadSpeedMultiplier : 1f;
            reloadTimer = definition.reloadTime / speedMultiplier;
            OnReloadStarted?.Invoke();
            OnAmmoChanged?.Invoke(ammoInMagazine);
        }
        
        public float ReloadProgress
        {
            get
            {
                if (!isReloading)
                    return 1f;

                return 1f - (reloadTimer / definition.reloadTime);
            }
        }

        protected virtual void FinishReload()
        {
            ammoInMagazine = definition.magazineSize;
            isReloading = false;
            OnReloadFinished?.Invoke();
            OnAmmoChanged?.Invoke(ammoInMagazine);
        }

        protected virtual void PerformShot()
        {
            ammoInMagazine--;
            float rate = definition.fireRate;
            if (OwnerStats != null)
                rate *= OwnerStats.FireRateMultiplier;

            fireCooldown = 1f / rate;
            var sfx = App.Instance.Services.Get<SfxService>();
            sfx.Play(definition.shotSfx);
            RaiseAttack();
            OnShot?.Invoke();
            OnAmmoChanged?.Invoke(ammoInMagazine);
        }
        
        public void CancelReload()
        {
            if (!isReloading)
                return;

            isReloading = false;
            reloadTimer = 0f;
            OnAmmoChanged?.Invoke(ammoInMagazine);
        }
    }
}