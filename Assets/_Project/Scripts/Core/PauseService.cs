using UnityEngine;

namespace _Project.Scripts.Core
{
    public sealed class PauseService : IGameService
    {
        private readonly EventBus bus;
        private float prevTimeScale = 1f;

        public bool IsPaused { get; private set; }

        public PauseService(EventBus bus)
        {
            this.bus = bus;
        }

        public void Toggle()
        {
            if (IsPaused) Resume();
            else Pause();
        }

        public void Pause()
        {
            if (IsPaused) return;

            IsPaused = true;
            prevTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            bus.RaisePaused();
        }

        public void Resume()
        {
            if (!IsPaused) return;

            IsPaused = false;
            Time.timeScale = prevTimeScale <= 0f ? 1f : prevTimeScale;
            bus.RaiseResumed();
        }
    }
}