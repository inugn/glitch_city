using _Project.Scripts.Enemies.Core;

namespace _Project.Scripts.Enemies.Attacks
{
    public interface IEnemyAttack
    {
        void Setup(EnemyBase enemy);
        void Tick();
    }
}