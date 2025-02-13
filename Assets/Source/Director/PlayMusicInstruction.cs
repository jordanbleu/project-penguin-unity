using System;
using Source.Audio;
using Source.Constants;
using UnityEngine;

namespace Source.Director
{
    public class PlayMusicInstruction : GameInstruction
    {
        [SerializeField]
        private AudioClip song;
        
        [SerializeField]
        [Tooltip("How to handle fading in the new music")]
        private FadeType fade = FadeType.None;
            
        [SerializeField]
        [Tooltip("Whether the music should loop")]
        private bool loop = true;
        
        
        public enum FadeType
        {
            None,
            FadeOutButNotIn,
            FadeOutAndFadeIn,
            FadeInButNotOut
        }
        
        public override void InstructionBegin()
        {
            var musicBoxObj = GameObject.FindWithTag(Tags.MusicBox);
            
            if (!musicBoxObj)
                throw new UnityException("Missing object tagged as music-box in the scene");
            
            var musicBox = musicBoxObj.GetComponent<MusicBox>();
            
            switch (fade)
            {
                case FadeType.None:
                    musicBox.PlayImmediate(song, loop);
                    break;
                case FadeType.FadeOutButNotIn:
                    musicBox.FadeOutThenPlayImmediate(song, loop);
                    break;
                case FadeType.FadeOutAndFadeIn:
                    musicBox.FadeToSong(song, loop);
                    break;
                case FadeType.FadeInButNotOut:
                    musicBox.PlayWithFadeIn(song, loop);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        public override bool IsInstructionComplete()
        {
            return true;
        }
    }
}