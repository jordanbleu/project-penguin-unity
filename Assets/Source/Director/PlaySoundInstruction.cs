using System;
using Source.Audio;
using Source.Constants;
using UnityEngine;

namespace Source.Director
{
    public class PlaySoundInstruction : GameInstruction
    {
        [SerializeField]
        private AudioClip _audioClip;

        public override void InstructionBegin()
        {
            var soundEffectEmitterObj = GameObject.FindWithTag(Tags.SoundEffectEmitter);
            
            if (!soundEffectEmitterObj)
                throw new UnityException("Missing object tagged as music-box in the scene");
            
            var soundEffectEmitter = soundEffectEmitterObj.GetComponent<SoundEffectEmitter>();

            soundEffectEmitter.Play(gameObject, _audioClip);
        }

        public override bool IsInstructionComplete()
        {
            return true;
        }
    }
}