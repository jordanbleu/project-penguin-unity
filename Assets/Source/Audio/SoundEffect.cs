using System;
using UnityEngine;

namespace Source.Audio
{
    /// <summary>
    /// Represents a sound effect that is currently playing.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SoundEffect : MonoBehaviour
    {
        private AudioSource _audioSource;
        private AudioClip _clip;
        
        public void Initialize(AudioClip clip, float panX = 0f, float volume = 1f)
        {
            _audioSource = gameObject.GetComponent<AudioSource>();
            _audioSource.clip = clip;
            _audioSource.panStereo = panX;
            _audioSource.volume = volume;
            _clip = clip;
        }

        public void PlayOnce()
        {
            _audioSource.PlayOneShot(_clip);
        }

        private void Update()
        {
            // destroy self after sound effect is done 
            if (!_audioSource.isPlaying)
            {
                Destroy(gameObject);
            }
        }
    }
}