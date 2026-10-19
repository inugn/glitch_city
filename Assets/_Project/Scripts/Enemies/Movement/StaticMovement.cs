using _Project.Scripts.Enemies.Core;

namespace _Project.Scripts.Enemies.Movement
{
    public sealed class StaticMovement : UnityEngine.MonoBehaviour, IEnemyMovement
    {
        public void Setup(EnemyBase enemy) { }
        public void Tick() { }
    }
}