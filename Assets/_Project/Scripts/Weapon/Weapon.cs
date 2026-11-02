using System;

namespace _Project.Scripts.Weapon
{
    public abstract class Weapon
    {
        public event Action OnAttack;

        public abstract bool CanAttack { get; }

        public virtual void Tick(float deltaTime)
        {
        }

        public abstract void Attack();

        protected void RaiseAttack()
        {
            OnAttack?.Invoke();
        }
    }
}