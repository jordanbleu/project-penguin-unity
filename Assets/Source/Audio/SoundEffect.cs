using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Source.Audio
{
    /// <summary>
    /// Represents a sound effect that is currently playing.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SoundEffect : MonoBehaviour
    {
        private AudioSource _audioSource;
        
        public AudioClip Clip => _clip;
        private AudioClip _clip;
        private GameObject _sourceGameObject;
        private string _sourceObjectName;
        private bool _isLooped = false;
        
        
        
        /// <summary>
        /// This NEEDS to be called before any other method here is called.  There are no assertions that check this,
        /// so you will just get a null ref. This is a design choice to make the code more efficient.
        /// </summary>
        public void Initialize(GameObject source, AudioClip clip, float panX = 0f, float volume = 1f, bool enableLowPassReverbEffect = false)
        {
            _sourceGameObject = source;
            _audioSource = gameObject.GetComponent<AudioSource>();
            _audioSource.clip = clip;
            _audioSource.panStereo = panX;
            _audioSource.volume = volume;
            _clip = clip;
            _sourceObjectName = source.name;
            
            if (enableLowPassReverbEffect)
            {
                var reverbFilter = gameObject.GetComponent<AudioReverbFilter>();
                reverbFilter.enabled = true;
                var lowPassFilter = gameObject.GetComponent<AudioLowPassFilter>();
                lowPassFilter.enabled = true;
                lowPassFilter.cutoffFrequency = 800;
            }

        }

        public void RandomizeStartTime()
        {
            var len = _clip.length;
            var startTime = Random.Range(0, len);
            _audioSource.time = startTime;
        }

        public void SetTime(float time)
        {
            _audioSource.time = time;
        }
        

        public void PlayOnce()
        {
            // Make sure you call Initialize first 
            _audioSource.PlayOneShot(_clip);
        }
        
        public void PlayLooped()
        {
            // Make sure you call Initialize first 
            _isLooped = true;
            _audioSource.loop = true;
            _audioSource.Play();
        }

        public void Destroy()
        {
            _audioSource.Stop();
            Destroy(gameObject);
        }

        private void Update()
        {
            if (_clip is null)
                return;
            
            
            // destroy self after sound effect is done 
            if (!_audioSource.isPlaying && !_isLooped)
            {
                Destroy(gameObject);
                return;
            }

            // sound is still playing but source object is destroyed.
            if (_isLooped && (!_sourceGameObject.activeSelf || _sourceGameObject == null))
            {
                // consider removing this debuyg, this might be fine.
                Debug.LogWarning($"A looped sound effect '{_clip.name}' is still playing even though the source object '{_sourceObjectName}' was destroyed, so we just abruptly stopped the sound effect.");
                _audioSource.Stop();
                Destroy(gameObject);
                return;
            }

            if (_sourceGameObject != null)
                transform.position = _sourceGameObject.transform.position;
        }
    }
}