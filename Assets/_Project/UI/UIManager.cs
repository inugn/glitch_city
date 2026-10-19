using _Project.Scripts.Player;
using UnityEngine;

namespace _Project.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [SerializeField] private PlayerStatsUI playerStatsUI;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void RegisterPlayer(Player player)
        {
            playerStatsUI.Initialize(player);
        }

    }
}