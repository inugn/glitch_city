using UnityEngine;
using UnityEngine.Audio;

namespace _Project.Scripts.Core.Audio
{
    public class AudioSettingsService
    {
        private const string MusicParam = "Music";
        private const string SfxParam = "Sfx";

        private readonly AudioMixer mixer;

        public AudioSettingsData Current { get; private set; }

        public AudioSettingsService(AudioMixer mixer)
        {
            this.mixer = mixer;
            Current = new AudioSettingsData();
            Apply(Current);
        }

        public void Apply(AudioSettingsData data)
        {
            Current.MusicVolume = data.MusicVolume;
            Current.SfxVolume = data.SfxVolume;

            mixer.SetFloat(MusicParam, LinearToDb(Current.MusicVolume));
            mixer.SetFloat(SfxParam, LinearToDb(Current.SfxVolume));
        }

        private static float LinearToDb(float value)
        {
            if (value <= 0.0001f) return -80f;
            return Mathf.Log10(value) * 20f;
        }
    }
}