using UnityEngine;

namespace _Project.Scripts.Core
{
    public class StartZoneInitializer : MonoBehaviour
    {
        private void Start()
        {
            var session = App.Instance.Services.Get<GameSession>();
            session.StartRun();
        }
    }
}