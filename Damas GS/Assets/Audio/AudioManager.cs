using UnityEngine;

namespace Damas
{
    public class AudioManager : Singleton<AudioManager>
    {
        public AudioClip loop;

        [field: SerializeField, ReadOnly] public AudioSource musicSource { get; private set; }

        private void Start()
        {
            PlayLoop(loop);
        }

        public void PlayMusic(AudioClip clip)
        {
            if (musicSource != null)
            {
                Destroy(musicSource.gameObject);
            }

            musicSource = new GameObject("Music").AddComponent<AudioSource>();
            musicSource.transform.SetParent(transform);
            musicSource.clip = clip;

            musicSource.Play();
        }

        public void PlayLoop(AudioClip clip)
        {
            PlayMusic(clip);
            musicSource.loop = true;
        }

        public void StopLoop()
        {
            Destroy(musicSource.gameObject);
        }
    }
}
