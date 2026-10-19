using UnityEngine;

namespace _Project.Scripts.Core.World
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class DoorTrigger : MonoBehaviour
    {
        public enum DoorSide { Left, Right }

        [SerializeField] private DoorSide side;
        [SerializeField] private bool isLocked = false;

        private void Reset()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        public void SetLocked(bool locked) => isLocked = locked;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isLocked) return;
            if (!other.CompareTag("Player")) return;

            DoorRouter.TryUseDoor(side);
        }
        
    }
}