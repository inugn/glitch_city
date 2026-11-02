using UnityEngine;

namespace _Project.Scripts.Core.Item
{
    public abstract class Item : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject interactHint;
        public GameObject InteractHint => interactHint;

        public bool IsPlayerNearby { get; private set; }

        protected virtual void Awake()
        {
            if (interactHint != null)
                interactHint.SetActive(false);
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;

            IsPlayerNearby = true;
            ShowHint(true);
            OnPlayerEnter(other);
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;

            IsPlayerNearby = false;
            ShowHint(false);
            OnPlayerExit(other);
        }

        protected virtual void ShowHint(bool value)
        {
            if (interactHint != null)
                interactHint.SetActive(value);
        }

        protected virtual void OnPlayerEnter(Collider2D player) { }
        protected virtual void OnPlayerExit(Collider2D player) { }

        public abstract void Interact(Player.Player player);
    }
}