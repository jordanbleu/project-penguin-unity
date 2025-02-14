using Source.Audio;
using Source.Constants;
using UnityEngine;

namespace Source.Director
{
    public class PlayAmbienceInstruction : GameInstruction
    {
        [SerializeField]
        private AudioClip ambience;

        public override void InstructionBegin()
        {
            var ambBox = GameObject.FindWithTag(Tags.AmbienceBox);
            
            if (!ambBox)
                throw new UnityException("Missing object tagged as ambience-box in the scene");
            
            var musicBox = ambBox.GetComponent<MusicBox>();
            
            musicBox.PlayWithFadeIn(ambience, true);
        }

        public override bool IsInstructionComplete()
        {
            return true;
        }
    }
}