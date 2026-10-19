using UnityEngine.SceneManagement;
using _Project.Scripts.Core.Run;

namespace _Project.Scripts.Core.World
{
    public static class DoorRouter
    {
        public static void TryUseDoor(DoorTrigger.DoorSide side)
        {
            if (side == DoorTrigger.DoorSide.Left)
                return;

            var services = App.Instance.Services;
            var session = services.Get<GameSession>();
            var loader = services.Get<SceneLoader>();
            var runDef = services.Get<RunDefinition>();

            var current = SceneManager.GetActiveScene().name;

            if (current == SceneNames.StartZone)
            {
                session.StartRun();
                loader.Load(SceneNames.Arena);
                return;
            }

            if (current == SceneNames.Arena)
            {
                // ✅ арена пройдена -> двигаем индекс на следующую
                session.SetArena(session.ArenaIndex + 1);
                loader.Load(SceneNames.Shelter);
                return;
            }

            if (current == SceneNames.Shelter)
            {
                int arenasCount = runDef != null ? runDef.ArenasCount : 3;

                // После прохождения 3 арен ArenaIndex станет 4
                if (session.ArenaIndex > arenasCount)
                    loader.Load(SceneNames.BossArena);
                else
                    loader.Load(SceneNames.Arena);

                return;
            }

            if (current == SceneNames.BossArena)
            {
                // После босса -> конец забега
                session.EndRun();
                loader.Load(SceneNames.StartZone);
                return;
            }
        }
    }
}