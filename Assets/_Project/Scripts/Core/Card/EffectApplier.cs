using _Project.Scripts.Core.Actor;
using _Project.Scripts.Core.Arena;
using UnityEngine;

namespace _Project.Scripts.Core.Card
{
    //TODO ДОБАВИТЬ ПОДДЕРЖКУ МОДИФИКАТОРОВ ВОЛНЫ
    public sealed class EffectApplier
    {
        private readonly Stats playerStats;

        public EffectApplier(Stats playerStats)
        {
            if (playerStats == null)
                throw new System.ArgumentNullException(nameof(playerStats));

            if (playerStats.Health == null)
                throw new System.InvalidOperationException("PlayerStats.Health is null. Call Stats.Initialize() first.");

            this.playerStats = playerStats;
        }

        public void Apply(CardEffect effect)
        {
            ApplyPlayerEffect(effect);
        }

        public void ClearRunEffects()
        {
            ClearPlayerEffects();
            ClearWaveEffects();
        }

        public void ClearWaveEffects()
        {
            //TODO сделать очистку модификаторов волны
        }

        private void ClearPlayerEffects()
        {
            playerStats.ModifyDamageMultiplier(1f - playerStats.DamageMultiplier);
            playerStats.ModifyFireRateMultiplier(1f - playerStats.FireRateMultiplier);
            playerStats.ModifyReloadSpeedMultiplier(1f - playerStats.ReloadSpeedMultiplier);
        }

        private void ApplyPlayerEffect(CardEffect effect)
        {
            switch (effect.Type)
            {
                case EffectType.DamageMultiplier:
                    playerStats.ModifyDamageMultiplier(effect.Value);
                    break;

                case EffectType.FireRateMultiplier:
                    playerStats.ModifyFireRateMultiplier(effect.Value);
                    break;

                case EffectType.ReloadSpeedMultiplier:
                    playerStats.ModifyReloadSpeedMultiplier(effect.Value);
                    break;

                case EffectType.MoveSpeedMultiplier:
                    playerStats.ModifyMoveSpeedMultiplier(effect.Value);
                    break;
                
                case EffectType.MaxHealthBonus:
                    playerStats.Health.ModifyMaxHealth((int)effect.Value);
                    break;

                case EffectType.HealInstant:
                    playerStats.Health.Heal((int)effect.Value);
                    break;

                default:
                    Debug.LogWarning($"Unhandled player effect: {effect.Type}");
                    break;
            }
        }
    }
}
