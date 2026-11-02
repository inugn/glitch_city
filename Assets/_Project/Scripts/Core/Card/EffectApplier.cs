using _Project.Scripts.Core.Actor;
using _Project.Scripts.Core.Arena;
using UnityEngine;

namespace _Project.Scripts.Core.Card
{
    //TODO ДОБАВИТЬ ПОДДЕРЖКУ МОДИФИКАТОРОВ ВОЛНЫ
    public sealed class EffectApplier
    {
        private readonly GameSession session;

        public EffectApplier(GameSession session)
        {
            this.session = session;
        }

        public void ApplyCard(CardData card)
        {
            if (card == null)
            {
                Debug.LogWarning("[EffectApplier] Card is null.");
                return;
            }

            var player = session.Player;
            if (player == null)
            {
                Debug.LogError("[EffectApplier] Player not found in GameSession.");
                return;
            }

            ApplyEffect(player.Stats, card.PositiveEffect);
            ApplyEffect(player.Stats, card.NegativeEffect);
        }

        public void ClearRunEffects()
        {
            ClearEffects();
            ClearWaveEffects();
        }

        public void ClearWaveEffects()
        {
            //TODO сделать очистку модификаторов волны
        }

        private void ClearEffects()
        {
            var player = session.Player;
            if (player == null)
            {
                Debug.LogError("[EffectApplier] Player not found in GameSession.");
                return;
            }
            player.Stats.ModifyDamageMultiplier(1f - player.Stats.DamageMultiplier);
            player.Stats.ModifyFireRateMultiplier(1f - player.Stats.FireRateMultiplier);
            player.Stats.ModifyReloadSpeedMultiplier(1f - player.Stats.ReloadSpeedMultiplier);
        }

        private void ApplyEffect(Stats stats, CardEffect effect)
        {
            switch (effect.Type)
            {
                case EffectType.DamageMultiplier:
                    stats.ModifyDamageMultiplier(effect.Value);
                    break;

                case EffectType.FireRateMultiplier:
                    stats.ModifyFireRateMultiplier(effect.Value);
                    break;

                case EffectType.ReloadSpeedMultiplier:
                    stats.ModifyReloadSpeedMultiplier(effect.Value);
                    break;

                case EffectType.MoveSpeedMultiplier:
                    stats.ModifyMoveSpeedMultiplier(effect.Value);
                    break;
                
                case EffectType.MaxHealthBonus:
                    stats.Health.ModifyMaxHealth((int)effect.Value);
                    break;

                case EffectType.HealInstant:
                    stats.Health.Heal((int)effect.Value);
                    break;

                default:
                    Debug.LogWarning($"Unhandled player effect: {effect.Type}");
                    break;
            }
        }
    }
}
