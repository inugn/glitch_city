namespace _Project.Scripts.Core
{
    public sealed class GameSession : IGameService
    {
        private readonly EventBus _bus;

        public bool RunInProgress { get; private set; }
        public int ArenaIndex { get; private set; } = 1; // 1..3
        public int WaveIndex { get; private set; } = 1;  // 1..3

        public GameSession(EventBus bus)
        {
            this._bus = bus;
        }

        public void StartRun()
        {
            RunInProgress = true;
            ArenaIndex = 1;
            WaveIndex = 1;

            _bus.RaiseRunStarted();
            _bus.RaiseArenaIndexChanged(ArenaIndex);
            _bus.RaiseWaveIndexChanged(WaveIndex);
        }

        public void EndRun()
        {
            RunInProgress = false;
            ArenaIndex = 1;
            WaveIndex = 1;

            _bus.RaiseRunEnded();
            _bus.RaiseArenaIndexChanged(ArenaIndex);
            _bus.RaiseWaveIndexChanged(WaveIndex);
        }

        public void SetWave(int wave)
        {
            WaveIndex = wave;
            _bus.RaiseWaveIndexChanged(WaveIndex);
        }

        public void SetArena(int arena)
        {
            ArenaIndex = arena;
            _bus.RaiseArenaIndexChanged(ArenaIndex);
        }
    }
}