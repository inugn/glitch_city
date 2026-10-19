using System;

namespace _Project.Scripts.Core.Arena
{
    [Serializable]
    public class CardEffect
    {
        public EffectType Type;
        public float Value;

        public string GetDescription()
        {
            return Type switch
            {
                // бонусы
                EffectType.DamageMultiplier =>
                    $"+{Value * 100f}% к урону игрока",

                EffectType.FireRateMultiplier =>
                    $"+{Value * 100f}% к скорострельности",

                EffectType.ReloadSpeedMultiplier =>
                    $"-{Value * 100f}% к перезарядке",

                EffectType.MoveSpeedMultiplier =>
                    $"+{Value * 100f}% к скорости передвижения",

                EffectType.MaxHealthBonus =>
                    $"+{Value} к макс. и текущему здоровью",

                EffectType.HealInstant =>
                    $"+{Value} к текущему здоровью",

                // дебаффы
                EffectType.EnemyDamageMultiplier =>
                    $"+{Value * 100f}% к урону врагов",

                EffectType.EnemyMaxHealthBonus =>
                    $"+{Value * 100f}% к здоровью врагов",

                EffectType.EnemyCountMultiplier =>
                    $"+{Value} враг(а/ов) в следующей волне",

                _ => "Неизвестный эффект"
            };
        }
    }
}