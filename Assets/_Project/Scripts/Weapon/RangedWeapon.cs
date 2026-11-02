using System;

namespace _Project.Scripts.Weapon
{
    public class RangedWeapon : Weapon
    {
        public event Action OnShot;
        public event Action OnReloadStarted;
        public event Action OnReloadFinished;

        protected readonly WeaponDefinition definition;

        protected int ammoInMagazine;
        protected float fireCooldown;
        protected bool isReloading;
        
        private float reloadTimer;

        public override bool CanAttack =>
            !isReloading &&
            ammoInMagazine > 0 &&
            fireCooldown <= 0f;

        public bool IsReloading => isReloading;

        protected RangedWeapon(WeaponDefinition definition)
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
                reloadTimer -= deltaTime;
                if (reloadTimer <= 0f)
                    FinishReload();
            }
        }

        public override void Attack()
        {
            if (!CanAttack)
                return;

            PerformShot();

            ammoInMagazine--;
            fireCooldown = 1f / definition.fireRate;

            RaiseAttack();
            OnShot?.Invoke();

            if (ammoInMagazine <= 0)
                Reload();
        }

        public virtual void Reload()
        {
            if (isReloading)
                return;

            isReloading = true;
            reloadTimer = definition.reloadTime;
            OnReloadStarted?.Invoke();
        }

        protected virtual void FinishReload()
        {
            ammoInMagazine = definition.magazineSize;
            isReloading = false;
            OnReloadFinished?.Invoke();
        }

        protected virtual void PerformShot()
        {
            // TODO создание пули
        }
    }
}