using UnityEngine;
using UnityEngine.Audio;

namespace _Project.Scripts.Core.Audio
{
    public sealed class SfxService
    {
        private readonly AudioSource source;
        private readonly AudioMixer mixer;

        public SfxService(AudioSource source, AudioMixer mixer)
        {
            this.source = source;
            this.mixer = mixer;

            source.loop = false;
            source.playOnAwake = false;
        }

        public void Play(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;

            source.PlayOneShot(clip, volume);
        }

        public void ApplyVolume(float value)
        {
            float db = value <= 0.001f ? -80f : Mathf.Log10(value) * 20f;
            mixer.SetFloat("SfxVolume", db);
        }
    }

}