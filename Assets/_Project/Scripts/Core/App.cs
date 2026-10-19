using _Project.Scripts.Core.Audio;
using _Project.Scripts.Core.Input;
using UnityEngine;

namespace _Project.Scripts.Core
{
    public sealed class App : MonoBehaviour
    {
        public static App Instance { get; private set; }
        public ServiceRegistry Services { get; private set; }

        [Header("Scene Loader (component)")]
        [SerializeField] private SceneLoader sceneLoader;

        [Header("Systems (component)")]
        [SerializeField] private CoreInputReader coreInputReader; // лежит в Systems

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            Compose();
        }

        private void OnValidate()
        {
            if (sceneLoader == null)
                sceneLoader = GetComponentInChildren<SceneLoader>(true);

            if (coreInputReader == null)
                coreInputReader = GetComponentInChildren<CoreInputReader>(true);
        }

        private void Compose()
        {
            Services = new ServiceRegistry();

            // Core services
            var bus = new EventBus();
            Services.Register(bus);

            Services.Register(new AudioSettingsService());
            
            var pause = new PauseService(bus);
            Services.Register(pause);

            // Run / Meta
            Services.Register(new GameSession(bus));
            Services.Register(new MetaProgress());

            // SceneLoader как MonoBehaviour-сервис
            Services.Register(sceneLoader);

            // ✅ Ввод как сервис: регистрируем по интерфейсу
            if (coreInputReader == null)
            {
                Debug.LogError("[App] CoreInputReader not found. Assign it in Inspector or keep it as child of App.", this);
            }
            else
            {
                Services.Register<ICoreInputReader>(coreInputReader);
            }
        }
    }
}