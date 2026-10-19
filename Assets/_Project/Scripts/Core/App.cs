using _Project.Scripts.Core.Audio;
using _Project.Scripts.Core.Input;
using _Project.Scripts.Core.Run;
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
        
        [Header("Run Config (ScriptableObject)")]
        [SerializeField] private RunDefinition runDefinition;

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

            var bus = new EventBus();
            Services.Register(bus);

            Services.Register(new AudioSettingsService());

            var pause = new PauseService(bus);
            Services.Register(pause);

            Services.Register(new GameSession(bus));
            Services.Register(new MetaProgress());

            Services.Register(sceneLoader);

            if (coreInputReader != null)
                Services.Register<ICoreInputReader>(coreInputReader);
            else
                Debug.LogError("[App] CoreInputReader not found.", this);

            // ✅ Регистрируем конфиг прогрессии
            if (runDefinition != null)
                Services.Register(runDefinition);
            else
                Debug.LogError("[App] RunDefinition is not assigned in Inspector.", this);
        }
    }
}