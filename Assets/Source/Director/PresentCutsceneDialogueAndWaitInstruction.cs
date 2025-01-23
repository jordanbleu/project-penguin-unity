using Source.Constants;
using Source.UI;
using UnityEngine;

namespace Source.Director
{
    /// <summary>
    /// This will tell the cutscene presenter to present the specific string of dialogue, then
    /// it will wait until the player has viewed (or skipped) the dialogue before continuing.
    /// </summary>
    public class PresentCutsceneDialogueAndWaitInstruction : GameInstruction
    {
        private CutsceneDialoguePresenter _presenter;

        [SerializeField]
        [Tooltip("The identifier of the dialogue to present, which is the leftmost value in the dialogue text file." +
                 "Multiple lines can have the same dialogue identifier, which will be presented in sequence.")]
        private string dialogueIdentifier;
        
        public override void InstructionBegin()
        {
            var presenterObj = GameObject.FindWithTag(Tags.CutsceneDialoguePresenter);
            _presenter = presenterObj.GetComponent<CutsceneDialoguePresenter>();
            
            _presenter.PresentDialogueById(dialogueIdentifier);
        }

        public override bool IsInstructionComplete()
        {
            if (_presenter == null)
            {
                Debug.LogError("CutsceneDialoguePresenter is null. Did you mean to use the gameplaydialogue instead?");
                return false;
            }

            return _presenter.IsDonePresenting;
        }
    }
}