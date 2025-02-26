using System.Collections;
using Source.GameData;
using UnityEngine;

namespace Source.Audio
{
    /// <summary>
    /// This class handles everything related to music, including
    /// fading, volume control, and more.
    /// </summary>
    [Tooltip("Responsible for playing in-game music.")]
    [RequireComponent(typeof(AudioSource))]
    public class MusicBox : MonoBehaviour
    {
        private AudioSource _audioSource;
        
        /// <summary>
        /// Harsh muting effect to signify the players low health
        /// or something bad or dramatic happening
        /// </summary>
        public bool EnableLowHealthEffect {get;set;}
        
        private AudioReverbFilter _reverbFilter;
        private AudioLowPassFilter _lowPassFilter;
        
        public void Start()
        {
            _reverbFilter = GetComponent<AudioReverbFilter>();
            _lowPassFilter = GetComponent<AudioLowPassFilter>();
            _audioSource = GetComponent<AudioSource>();
        }
        
        public void Update()
        {
            // Apply audio effects based on the game situation
            if (EnableLowHealthEffect)
            {
                _reverbFilter.enabled = true;
                _reverbFilter.decayTime = 6.1f;
                _lowPassFilter.enabled = true;
                _lowPassFilter.cutoffFrequency = 800;
            }
            else
            {
                _reverbFilter.enabled = false;
                _lowPassFilter.enabled = false;
            }
            
        
        
        }
        
        /// <summary>
        /// Sets the volume to full and plays the song
        /// </summary>
        public void PlayImmediate(AudioClip clip, bool? loop = null)
        {
            _audioSource.Stop();
            _audioSource.clip = clip;
            
            if (loop.HasValue)
                _audioSource.loop = loop.Value;
            
            var musicVolume = GlobalSaveDataManager.GlobalData.MusicVolume;
            _audioSource.volume = musicVolume;
            
            _audioSource.Play();
        }
        
        public void Stop()
        {
            _audioSource.Stop();
        }
        
        public void PlayWithFadeIn(AudioClip clip, bool loop = true)
        {
            _audioSource.Stop();
            _audioSource.clip = clip;
            _audioSource.volume = 0f;
            _audioSource.loop = loop;
            _audioSource.Play();
            StartCoroutine(FadeInAudioCoroutine());
        }
        
        private IEnumerator FadeInAudioCoroutine()
        {
            do
            {
                _audioSource.volume += 0.015f;
                yield return new WaitForSeconds(0.05f);
            } while (_audioSource.volume < GlobalSaveDataManager.GlobalData.MusicVolume);
            
            // in case something wonky happens
            _audioSource.volume = GlobalSaveDataManager.GlobalData.MusicVolume;
        }
        
        /// <summary>
        ///  Fades the music out then stops it.
        /// </summary>
        public void FadeOut()
        {
            _audioSource.volume = GlobalSaveDataManager.GlobalData.MusicVolume;
            StartCoroutine(FadeOutAudioCoroutine());
        }
        
        private IEnumerator FadeOutAudioCoroutine()
        {
            do
            {
                _audioSource.volume -= 0.015f;
                yield return new WaitForSeconds(0.05f);
            } while (_audioSource.volume > 0);
            
            _audioSource.Stop();
        }
        
        /// <summary>
        /// Fades the current song out, then fades the new song in.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="loop"></param>
        public void FadeToSong(AudioClip clip, bool loop = true)
        {
            _audioSource.loop = loop;
            StartCoroutine(FadeToSongCoroutine(clip));
        }
        
        private IEnumerator FadeToSongCoroutine(AudioClip clip)
        {
            // wait for FadeOutAudioRoutine to finish
            yield return StartCoroutine(FadeOutAudioCoroutine());
            
            // start the new song
            PlayWithFadeIn(clip);
        }

        /// <summary>
        /// Current music will fade out, then the new music will play immediately.
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        public void FadeOutThenPlayImmediate(AudioClip clip, bool loop = true)
        {
            _audioSource.loop = loop;
            StartCoroutine(FadeOutThenPlayImmediateCoroutine(clip));
        }
        
        private IEnumerator FadeOutThenPlayImmediateCoroutine(AudioClip clip)
        {
            yield return FadeOutAudioCoroutine();
            PlayImmediate(clip);
        }
    }
}