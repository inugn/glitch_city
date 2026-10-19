namespace _Project.Scripts.Core.Arena
{
    public static class EffectBalance
    {
        public static float GetValue(EffectType type, CardRarity rarity)
        {
            switch (type)
            {
                // Положительные
                case EffectType.DamageMultiplier:
                case EffectType.FireRateMultiplier:
                case EffectType.MoveSpeedMultiplier:
                case EffectType.ReloadSpeedMultiplier:
                    return rarity switch
                    {
                        CardRarity.Common => 0.05f,
                        CardRarity.Rare   => 0.10f,
                        CardRarity.Epic  => 0.15f,
                        _ => 0f
                    };

                case EffectType.MaxHealthBonus:
                case EffectType.HealInstant:
                    return rarity switch
                    {
                        CardRarity.Common => 10,
                        CardRarity.Rare   => 20,
                        CardRarity.Epic  => 30,
                        _ => 0
                    };

                // Отрицательные
                case EffectType.EnemyDamageMultiplier:
                case EffectType.EnemyMaxHealthBonus:
                    return rarity switch
                    {
                        CardRarity.Common => 0.05f,
                        CardRarity.Rare   => 0.10f,
                        CardRarity.Epic  => 0.15f,
                        _ => 0f
                    };

                case EffectType.EnemyCountMultiplier:
                    return rarity switch
                    {
                        CardRarity.Common => 1,
                        CardRarity.Rare   => 2,
                        CardRarity.Epic  => 3,
                        _ => 0
                    };

                default:
                    return 0;
            }
        }
    }

}