using Source.Constants;
using Source.UI;
using UnityEngine;

namespace Source.Director
{
    public class WaitForHardDialogueInstruction : GameInstruction
    {
        private bool _isComplete = false;
        
        private InGameDialoguePresenter _dialoguePresenter;
        
        public override void InstructionBegin()
        {
            var dialoguePresenterObject = GameObject.FindWithTag(Tags.DialoguePresenter);
            _dialoguePresenter = dialoguePresenterObject.GetComponent<InGameDialoguePresenter>();
            _dialoguePresenter.AddOnCompleteListener(OnDialogueComplete);
        }

        private void OnDialogueComplete()
        {
            _isComplete = true;
        }

        public override void InstructionEnd()
        {
            _dialoguePresenter.RemoveOnCompleteListener(OnDialogueComplete);
        }


        public override bool IsInstructionComplete() => _isComplete;
    }
}