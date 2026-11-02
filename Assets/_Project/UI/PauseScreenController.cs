using _Project.Scripts.Core;
using UnityEngine;

namespace _Project.UI
{
    public sealed class PauseScreenController : MonoBehaviour
    {
        [SerializeField] private PauseScreenView view;

        private EventBus bus;

        private void Start()
        {
            bus = App.Instance.Services.Get<EventBus>();

            bus.Paused += OnPaused;
            bus.Resumed += OnResumed;

            view.Hide();
        }

        private void OnDestroy()
        {
            if (bus == null) return;

            bus.Paused -= OnPaused;
            bus.Resumed -= OnResumed;
        }

        private void OnPaused()
        {
            view.Show();
        }

        private void OnResumed()
        {
            view.Hide();
        }
        
        //knopki
        
        public void BackToGame()
        {
            App.Instance.Services.Get<PauseService>().Resume();
        }
        
        public void OpenSettings()
        {
            App.Instance.Services.Get<SceneLoader>().Load(SceneNames.Settings);
        }
        
        public void BackToMenu()
        {
            App.Instance.Services.Get<SceneLoader>().Load(SceneNames.Menu);
        }
        
        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}