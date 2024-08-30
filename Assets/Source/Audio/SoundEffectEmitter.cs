using System;
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
        /// <returns></returns>
        public SoundEffect Play(GameObject source, AudioClip clip, float panX = 0, float volume = 1f)
        {
            var name = BuildName(source, clip, panX, volume, false);
            
            var inst = Instantiate(soundEffectPrefab, transform);
            var snd = inst.GetComponent<SoundEffect>();
            
            snd.Initialize(source, clip, panX, volume);
            inst.name = name;
            snd.PlayOnce();
            
            return snd;
        }

        public SoundEffect PlayLooped(GameObject source, AudioClip clip, float panX = 0, float volume = 1f)
        {
            var name = BuildName(source, clip, panX, volume, true);
            
            var inst = Instantiate(soundEffectPrefab, transform);
            var snd = inst.GetComponent<SoundEffect>();
            
            snd.Initialize(source, clip, panX, volume);
            inst.name = name;
            snd.PlayLooped();
            
            return snd;
        }
        
        public SoundEffect Play(GameObject source, AudioClip clip, SoundEffectSettings settings)
        {
            var name = BuildName(source, clip, settings.Pan, settings.Volume, settings.IsLooped);
            
            var inst = Instantiate(soundEffectPrefab, transform);
            var snd = inst.GetComponent<SoundEffect>();
            
            snd.Initialize(source, clip, settings.Pan, settings.Volume);
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