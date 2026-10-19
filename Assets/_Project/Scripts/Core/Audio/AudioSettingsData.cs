namespace _Project.Scripts.Core.Audio
{
    [System.Serializable]
    public class AudioSettingsData
    {
        public float MusicVolume = 1f;
        public float SfxVolume = 1f;

        public AudioSettingsData Clone()
        {
            return new AudioSettingsData
            {
                MusicVolume = MusicVolume,
                SfxVolume = SfxVolume
            };
        }
    }
}