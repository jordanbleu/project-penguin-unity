using UnityEngine;
using UnityEngine.Events;

namespace Source.Director
{
    public class TriggerEventInstruction : GameInstruction
    {
        [SerializeField]
        private UnityEvent eventsToTrigger = new();
        
        public override void InstructionBegin()
        {
            eventsToTrigger?.Invoke();
        }

        public override bool IsInstructionComplete()
        {
            return true;
        }
    }
}