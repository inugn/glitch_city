using Unity.VisualScripting;

namespace _Project.Scripts.Core
{
    public sealed class GameSession : IGameService
    {
        private readonly EventBus _bus;
        
        public Player.Player Player { get; private set; }
        public bool RunInProgress { get; private set; }
        public int ArenaIndex { get; private set; } = 1; // 1..N (какую арену играем следующей)
        public int WaveIndex { get; private set; } = 0;  // 0..N (0 = ещё не стартовали)
        public int Credits { get; private set; } = 0;

        public GameSession(EventBus bus) => _bus = bus;
        
        public void SetPlayer(Player.Player player)
        {
            Player = player;
        }

        public void StartRun()
        {
            if (Player != null)
            {
                Player.Respawn();
                Player.Stats.ResetMultipliers();
            }
            RunInProgress = true;
            ArenaIndex = 1;
            WaveIndex = 0;
            
            Credits = 0;

            _bus.RaiseCreditsChanged(Credits);
            _bus.RaiseRunStarted();
            _bus.RaiseArenaIndexChanged(ArenaIndex);
            _bus.RaiseWaveIndexChanged(WaveIndex);
        }

        public void EndRun()
        {
            RunInProgress = false;
            ArenaIndex = 1;
            WaveIndex = 0;
            
            Credits = 0;

            _bus.RaiseCreditsChanged(Credits);
            _bus.RaiseRunEnded();
            if (Player.Stats.Health.IsDead)
                _bus.RaisePlayerDead();
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
        
        public void AddCredits(int amount) 
        {
            if (amount < 0 && Credits + amount < 0) return;

            Credits += amount;
            _bus.RaiseCreditsChanged(Credits);
        }
    }
}