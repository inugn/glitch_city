using UnityEngine;

namespace _Project.Scripts.Core.Audio
{
    public sealed class MusicService : IGameService
    {
        private readonly AudioSource source;

        public MusicService(AudioSource source)
        {
            this.source = source;
            this.source.loop = true;
        }

        public void Play(AudioClip clip)
        {
            if (clip == null) return;
            if (source.clip == clip && source.isPlaying) return;

            source.clip = clip;
            source.Play();
        }

        public void Stop()
        {
            source.Stop();
            source.clip = null;
        }
    }
}