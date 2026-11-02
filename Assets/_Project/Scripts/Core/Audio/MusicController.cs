using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Core.Audio
{
    public sealed class MusicController : MonoBehaviour
    {
        [Header("Music")]
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip gameplayMusic;

        private MusicService music;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            music = App.Instance.Services.Get<MusicService>();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            switch (scene.name)
            {
                case SceneNames.Menu:
                case SceneNames.Settings:
                case SceneNames.StartZone:
                case SceneNames.Shelter:
                    music.Play(menuMusic);
                    break;

                case SceneNames.Arena:
                case SceneNames.BossArena:
                    music.Play(gameplayMusic);
                    break;
            }
        }
    }

}