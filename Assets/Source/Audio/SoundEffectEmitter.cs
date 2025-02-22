using System;
using System.Collections.Generic;
using System.Linq;
using Source.Utilities;
using UnityEngine;

namespace Source.Audio
{
    /// <summary>
    /// This object should be the central source of all sound effects played in the game.
    ///
    /// The main purpose is so sounds effects can continue to play even while the emitting object is destroyed.
    /// </summary>
    public class SoundEffectEmitter : MonoBehaviour
    {
        [SerializeField]
        private GameObject soundEffectPrefab;

        /// <summary>
        /// Harsh muting effect to signify the players low health
        /// or something bad or dramatic happening
        /// </summary>
        public bool EnableLowHealthEffect {get;set;}
        
        public SoundEffect PlayRandom(GameObject source, AudioClip[] clips, float panX = 0, float volume = 1f)
        {
            if (!clips.Any())
                return null; 
            
            var clip = RandomUtils.Choose(clips);
            return Play(source, clip, panX, volume);
        }
        
        /// <summary>
        /// Plays a sound effect once.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="panX">Between -1 and 1, where the sound should play from </param>
        /// <param name="volume">Between 0 to 1, how loud should sound be </param>
        /// <param name="enableRepeatLimiter">If false, sound can be played as many time as the method is called.  If false, will only let one instance of the sound play.</param>
        /// <returns></returns>
        public SoundEffect Play(GameObject source, AudioClip clip, float panX = 0, float volume = 1f, bool enableRepeatLimiter = true)
        {
            var existing = GetExistingClips(clip);
            if (enableRepeatLimiter && existing)
                return existing;
            
            var name = BuildName(source, clip, panX, volume, false);
            
            var inst = Instantiate(soundEffectPrefab, transform);
            var snd = inst.GetComponent<SoundEffect>();
            
            snd.Initialize(source, clip, panX, volume, enableLowPassReverbEffect: EnableLowHealthEffect);
            inst.name = name;
            snd.PlayOnce();
            
            return snd;
        }

        public SoundEffect PlayLooped(GameObject source, AudioClip clip, float panX = 0, float volume = 1f)
        {
            var existing = GetExistingClips(clip);
            if (existing)
                return existing;
            
            var name = BuildName(source, clip, panX, volume, true);
            
            var inst = Instantiate(soundEffectPrefab, transform);
            var snd = inst.GetComponent<SoundEffect>();
            
            snd.Initialize(source, clip, panX, volume, enableLowPassReverbEffect: EnableLowHealthEffect);
            inst.name = name;
            snd.PlayLooped();
            
            return snd;
        }

        /// <summary>
        /// Plays a simple sound effect with default settings.
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        public SoundEffect Play(AudioClip clip,bool enableRepeatLimiter = true)
        {
            var existing = GetExistingClips(clip);
            if (enableRepeatLimiter && existing)
                return existing;
            
            var name = BuildName(gameObject, clip, 0, 1, false);
            
            var inst = Instantiate(soundEffectPrefab, transform);
            var snd = inst.GetComponent<SoundEffect>();
            inst.name = name;
            snd.Initialize(gameObject, clip, enableLowPassReverbEffect: EnableLowHealthEffect);
            
            snd.PlayOnce();
            return snd;
        }
        
        public SoundEffect Play(GameObject source, AudioClip clip, SoundEffectSettings settings)
        {
            var existing = GetExistingClips(clip);
            if (existing)
                return existing;
            
            var name = BuildName(source, clip, settings.Pan, settings.Volume, settings.IsLooped);
            
            var inst = Instantiate(soundEffectPrefab, transform);
            var snd = inst.GetComponent<SoundEffect>();
            
            snd.Initialize(source, clip, settings.Pan, settings.Volume, enableLowPassReverbEffect: EnableLowHealthEffect);
            inst.name = name;
            
            if (settings.RandomizeStartTime)
            {
                snd.RandomizeStartTime();
            }
            
            if (settings.IsLooped)
            {
                snd.PlayLooped();
            }
            else
            {
                snd.PlayOnce();
            }
            
            return snd;
        }
        
        private SoundEffect GetExistingClips(AudioClip clip)
        {
            var soundEffects = GetComponentsInChildren<SoundEffect>();
            return soundEffects.FirstOrDefault(s => s.Clip == clip);
        }

        private string BuildName(GameObject source, AudioClip clip, float panX, float volume, bool isLooped)
        {
            var sourceString = source == null ? "???" : source.name;
            var id = Guid.NewGuid();
            return $"src={sourceString} - clip={clip.name} - isLoop={isLooped} - pan={panX} - vol={volume} <{id}>";
        }

        public class SoundEffectSettings
        {
            public float Pan { get; set; } = 0;
            public float Volume { get; set; } = 1f;
            public bool IsLooped { get; set; }
            
            /// <summary>
            /// If true the sound will start at a random time between 0 and the length of the sound.
            /// </summary>
            public bool RandomizeStartTime { get; set; }

        }

    }
}