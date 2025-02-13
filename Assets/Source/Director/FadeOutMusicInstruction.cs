using Source.Audio;
using Source.Constants;
using UnityEngine;

namespace Source.Director
{
    public class FadeOutMusicInstruction : GameInstruction
    {
        public override void InstructionBegin()
        {
            var musicBoxObj = GameObject.FindWithTag(Tags.MusicBox);
            
            if (!musicBoxObj)
                throw new UnityException("Missing object tagged as music-box in the scene");
            
            var musicBox = musicBoxObj.GetComponent<MusicBox>();
            musicBox.FadeOut();
        }

        public override bool IsInstructionComplete()
        {
            return true;
        }
    }
}