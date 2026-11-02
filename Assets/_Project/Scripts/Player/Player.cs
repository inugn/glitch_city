using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Core;
using _Project.Scripts.Core.Actor;
using _Project.Scripts.Weapon;
using _Project.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public class Player : Actor
    {
        [SerializeField] private List<WeaponDefinition> startingWeapons;
        private PlayerCombat combat;
        public PlayerCombat Combat => combat;
        public event Action OnSlotsInitialized;
        
        protected override void Awake()
        {
            base.Awake();
            combat = GetComponent<PlayerCombat>();
        }
        
        private void Start()
        {
            InitializeSlots(startingWeapons);
        }

        private void Update()
        {
            Stats.Health.Regenerate(Time.deltaTime);
        }

        public void TakeDamage(int damage)
        {
            Debug.Log(stats.Health.CurrentHealth + "[" + stats.Health.MaxHealth + "]");
            Stats.Health.TakeDamage(damage);
        }
        
        public void InitializeSlots(List<WeaponDefinition> startingWeapons)
        {
            combat.InitializeSlots(startingWeapons);
            OnSlotsInitialized?.Invoke();
        }
        
        protected override void OnDeath()
        {
            base.OnDeath();

            Combat.enabled = false;

            App.Instance.Services.Get<GameSession>().EndRun();
        }
    }
}