using System;
using _Project.Scripts.Core.Actor;
using UnityEngine;

namespace _Project.Scripts.Weapon
{
    public abstract class Weapon
    {
        public Vector2 TargetPosition { get; set; }
        
        public Actor Owner { get; private set; }
        
        public Stats OwnerStats => Owner?.Stats;
        
        public WeaponDefinition definition { get; protected set; }

        public event Action OnAttack;
        
        public void SetOwner(Actor owner)
        {
            Owner = owner;
        }

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