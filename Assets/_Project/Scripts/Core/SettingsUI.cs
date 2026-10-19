using _Project.Scripts.Core.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Project.Scripts.Core
{
    public class SettingsUI : MonoBehaviour
    {
        [Header("Sliders")]
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        private AudioSettingsService _audioService;
        private AudioSettingsData _editingCopy;

        private void Start()
        {
            _audioService = App.Instance.Services.Get<AudioSettingsService>();

            _editingCopy = _audioService.Current.Clone();

            musicSlider.value = _editingCopy.MusicVolume;
            sfxSlider.value = _editingCopy.SfxVolume;

            musicSlider.onValueChanged.AddListener(OnMusicChanged);
            sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        }

        private void OnMusicChanged(float value)
        {
            _editingCopy.MusicVolume = value;
        }

        private void OnSfxChanged(float value)
        {
            _editingCopy.SfxVolume = value;
        }

        public void Apply()
        {
            _audioService.Apply(_editingCopy);
            ReturnToPreviousScene();
        }

        public void Cancel()
        {
            ReturnToPreviousScene();
        }
        
        private void ReturnToPreviousScene()
        {
            // тут мог бы быть возврат на прошлую сцену но я так и не поняла как
            // поэтому пока только на меню
            App.Instance.Services.Get<SceneLoader>().Load(SceneNames.Menu);
        }
    }
}