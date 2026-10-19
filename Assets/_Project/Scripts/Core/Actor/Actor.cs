using UnityEngine;

namespace _Project.Scripts.Core.Actor
{
    public class Actor : MonoBehaviour
    {
        [SerializeField] protected Stats stats;

        public Stats Stats => stats;

        protected virtual void Awake()
        {
            stats.Initialize();
        }
    }
}