using UnityEngine;

namespace _Project.Scripts.Player
{
    public sealed class PlayerSpawnPoint : MonoBehaviour
    {
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, 0.2f);
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 0.8f);
        }
#endif
    }
}