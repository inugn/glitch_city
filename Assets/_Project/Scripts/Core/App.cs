using _Project.Scripts.Core.Audio;
using _Project.Scripts.Core.Card;
using _Project.Scripts.Core.Input;
using _Project.Scripts.Core.Run;
using _Project.Scripts.Core.Save;
using UnityEngine;
using UnityEngine.Audio;

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
        
        [Header("Audio")]
        [SerializeField] private AudioMixer masterMixer;
        
        [SerializeField] private MusicController audioController;
        
        private MetaProgressPersistence metaPersistence;

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

            var audioSource = audioController.GetComponent<AudioSource>();
            Services.Register(new MusicService(audioSource));

            Services.Register(new AudioSettingsService(masterMixer));
            
            var sfxSourceGO = new GameObject("SFX AudioSource");
            sfxSourceGO.transform.SetParent(transform);

            var sfxSource = sfxSourceGO.AddComponent<AudioSource>();
            sfxSource.outputAudioMixerGroup = masterMixer.FindMatchingGroups("SFX")[0];

            Services.Register(new SfxService(sfxSource, masterMixer));


            var pause = new PauseService(bus);
            Services.Register(pause);

            var session = new GameSession(bus);
            Services.Register(session);
            
            var meta = new MetaProgress();
            Services.Register(meta);

            Services.Register(sceneLoader);

            if (coreInputReader != null)
                Services.Register<ICoreInputReader>(coreInputReader);
            else
                Debug.LogError("[App] CoreInputReader not found.", this);
            
            Services.Register(new EffectApplier(session));
            
            if (runDefinition != null)
                Services.Register(runDefinition);
            else
                Debug.LogError("[App] RunDefinition is not assigned in Inspector.", this);
            
            var storage = new JsonFileProgressStorage();
            metaPersistence = new MetaProgressPersistence(meta, storage, bus);
            Services.Register(metaPersistence);
            
            var deathScreenGO = new GameObject("DeathScreenController");
            var deathScreenCtrl = deathScreenGO.AddComponent<DeathScreenController>();
            Services.Register(deathScreenCtrl);

            metaPersistence.Load();
            
        }
        
        private void OnApplicationQuit()
        {
            metaPersistence?.SaveIfDirty();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
                metaPersistence?.SaveIfDirty();
        }
    }
}