using UnityEngine;

namespace Source.Director
{
    [Tooltip("Waits for something external to call the SetComplete() method.   ")]
    public class WaitForExternalTriggerInstruction : GameInstruction
    {
        
        private bool _isComplete = false;
        
        public void SetComplete()
        {
            _isComplete = true;
        }
        
        public override bool IsInstructionComplete()
        {
            return _isComplete;
        }
    }
}