using UnityEngine;

namespace _Project.Scripts.Player
{
    public sealed class GroundCheck : MonoBehaviour
    {
        [SerializeField] private Transform point;
        [SerializeField] private float radius = 0.12f;
        [SerializeField] private LayerMask groundMask;

        public bool IsGrounded { get; private set; }

        private void Update()
        {
            if (point == null)
            {
                IsGrounded = false;
                return;
            }

            IsGrounded = Physics2D.OverlapCircle(point.position, radius, groundMask) != null;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (point == null) return;
            Gizmos.DrawWireSphere(point.position, radius);
        }
#endif
    }
}