using UnityEngine;

namespace _Project.Scripts.Core
{
    public sealed class StartZoneController : MonoBehaviour
    {
        public void StartRun()
        {
            var services = App.Instance.Services;
            services.Get<GameSession>().StartRun();
            services.Get<SceneLoader>().Load(SceneNames.Arena);
        }
    }
}