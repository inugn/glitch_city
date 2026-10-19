using UnityEngine.SceneManagement;

namespace _Project.Scripts.Core.World
{
    public static class DoorRouter
    {
        public static void TryUseDoor(DoorTrigger.DoorSide side)
        {
            // По требованиям: левая дверь всегда недоступна (назад нельзя)
            if (side == DoorTrigger.DoorSide.Left)
                return;

            var services = App.Instance.Services;
            var session = services.Get<GameSession>();
            var loader = services.Get<SceneLoader>();

            var current = SceneManager.GetActiveScene().name;

            // MVP-логика переходов (без сложных условий, потом расширим):
            if (current == SceneNames.StartZone)
            {
                // Правая дверь из стартовой зоны -> первая арена
                session.StartRun();
                loader.Load(SceneNames.Arena);
                return;
            }

            if (current == SceneNames.Arena)
            {
                // После арены -> убежище
                loader.Load(SceneNames.Shelter);
                return;
            }

            if (current == SceneNames.Shelter)
            {
                // После убежища: если уже 3 арены пройдены -> босс, иначе -> следующая арена
                if (session.ArenaIndex >= 3)
                    loader.Load(SceneNames.BossArena);
                else
                    loader.Load(SceneNames.Arena);

                return;
            }

            if (current == SceneNames.BossArena)
            {
                // После босса -> стартовая зона (конец забега)
                session.EndRun();
                loader.Load(SceneNames.StartZone);
                return;
            }
        }
    }
}