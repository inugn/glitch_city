using _Project.Scripts.Core.Actor;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public class Player : Actor
    {
        // TODO слоты оружия

        [SerializeField] private PlayerInputReader inputReader;
        [SerializeField] private PlayerController2D controller;

        protected override void Awake()
        {
            base.Awake();

            if (inputReader == null)
                inputReader = GetComponent<PlayerInputReader>();
            if (controller == null)
                controller = GetComponent<PlayerController2D>();
        }

        private void Update()
        {
            Stats.Health.Regenerate(Time.deltaTime);
        }

        public void TakeDamage(int damage)
        {
            Stats.Health.TakeDamage(damage);
        }
    }
}