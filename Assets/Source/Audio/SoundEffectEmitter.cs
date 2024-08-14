using System;
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
        /// Plays a sound effect once.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="panX">Between -1 and 1, where the sound should play from </param>
        /// <param name="volume">Between 0 to 1, how loud should sound be </param>
        /// <returns></returns>
        public SoundEffect PlaySound(AudioClip clip, float panX = 0, float volume = 1f)
        {
            var name = $"snd-{clip.name} - pan:{panX} - vol:{volume}::{Guid.NewGuid()}";
            
            var inst = Instantiate(soundEffectPrefab, transform);
            var snd = inst.GetComponent<SoundEffect>();
            
            snd.Initialize(clip, panX, volume);
            inst.name = name;
            snd.PlayOnce();
            
            return snd;
        }


    }
}