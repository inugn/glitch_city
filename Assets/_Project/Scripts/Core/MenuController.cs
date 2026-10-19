using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Project.Scripts.Core
{
    public sealed class MenuController : MonoBehaviour
    {
        [SerializeField] private Button continueButton;
        
        public bool HasActiveRun { get; private set; }
        public string LastScene { get; private set; }
        
        public void StartRun()
        {
            
            continueButton.interactable = false; // TODO заменить когда будут сейвы
            
            var services = App.Instance.Services;
            services.Get<GameSession>().StartRun();
            services.Get<SceneLoader>().Load(SceneNames.Arena);
        }
        
        public void ContinueRun()
        {
            // TODO написать логику продолжения забега когда будет реализация сейвов
        }

        public void OpenSettings()
        {
            App.Instance.Services.Get<SceneLoader>().Load(SceneNames.Settings);
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