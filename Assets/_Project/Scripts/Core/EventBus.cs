using System;

namespace _Project.Scripts.Core
{
    public sealed class EventBus : IGameService
    {
        public event Action RunStarted;
        public event Action RunEnded;

        public event Action Paused;
        public event Action Resumed;

        public event Action<int> ArenaIndexChanged; // 1..3
        public event Action<int> WaveIndexChanged;  // 1..3

        public void RaiseRunStarted() => RunStarted?.Invoke();
        public void RaiseRunEnded() => RunEnded?.Invoke();

        public void RaisePaused() => Paused?.Invoke();
        public void RaiseResumed() => Resumed?.Invoke();

        public void RaiseArenaIndexChanged(int value) => ArenaIndexChanged?.Invoke(value);
        public void RaiseWaveIndexChanged(int value) => WaveIndexChanged?.Invoke(value);
    }
}