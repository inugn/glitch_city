namespace _Project.Scripts.Core.Audio
{
    public class AudioSettingsService
    {
        public AudioSettingsData Current { get; private set; }

        public AudioSettingsService()
        {
            Current = new AudioSettingsData();
        }

        public void Apply(AudioSettingsData data)
        {
            Current.MusicVolume = data.MusicVolume;
            Current.SfxVolume = data.SfxVolume;

            // TODO применение к AudioMixer
        }
    }
}