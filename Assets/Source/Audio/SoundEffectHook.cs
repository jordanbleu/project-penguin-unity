using System;
using System.Collections.Generic;
using System.Linq;
using Source.Constants;
using UnityEngine;

namespace Source.Audio
{
    [Tooltip("Generic way to emit sound effects without needing custom code")]
    public class SoundEffectHook : MonoBehaviour
    {
        [SerializeField]
        private SoundEffectData[] soundEffects;
        
        private SoundEffectEmitter _soundEmitter;
        
        private Dictionary<string, SoundEffectData> soundEffectDict;
        
        private void Start()
        {
            _soundEmitter = GameObject.FindWithTag(Tags.SoundEffectEmitter).GetComponent<SoundEffectEmitter>()
                ?? throw new UnityException("SoundEffectEmitter not found in scene");
            
            soundEffectDict = soundEffects.ToDictionary(x=>x.soundEffectId, x=>x);
        }
        
        public void Play(string soundEffectName)
        {
            if (!soundEffectDict.TryGetValue(soundEffectName, out var clips))
                throw new UnityException($"Sound effect {soundEffectName} not found in dictionary");
            _soundEmitter.PlayRandom(gameObject, clips.soundEffects);
        }
        
        [Serializable]
        public struct SoundEffectData
        {
            
            [Tooltip("The identifier of the sound effect group.")]
            public string soundEffectId;
            
            [Tooltip("Sound effects to play, one of these will be chosen at random each time.")]
            public AudioClip[] soundEffects;
        }
    }
}