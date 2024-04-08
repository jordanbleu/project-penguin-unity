using Source.UI;
using UnityEngine;

namespace Source.Director
{
    public class ShowDialogueInstruction : GameInstruction
    {
        [SerializeField]
        private GameplayDialogueBox dialogueBox;

        public override void InstructionBegin()
        {
            dialogueBox.Open();
        }

        public override bool IsInstructionComplete()
        {
            return false;
        }

        public override void InstructionEnd()
        {
            dialogueBox.Close();
        }
    }
}