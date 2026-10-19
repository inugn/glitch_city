using _Project.Scripts.Enemies.Core;

namespace _Project.Scripts.Enemies.Movement
{
    public interface IEnemyMovement
    {
        void Setup(EnemyBase enemy);
        void Tick();
    }
}