using UnityEngine;

namespace _Project.Scripts.Core.Actor
{
    public class Actor : MonoBehaviour
    {
        [SerializeField] protected Stats stats;

        public Stats Stats => stats;

        protected virtual void Awake()
        {
            if (stats == null)
            {
                Debug.LogError($"[{name}] Stats is not assigned in Inspector.", this);
                enabled = false;
                return;
            }

            stats.Initialize();
        }
    }
}