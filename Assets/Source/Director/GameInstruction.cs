using UnityEngine;

namespace Source.Director
{
    public abstract class GameInstruction : MonoBehaviour
    {

        /// <summary>
        /// Called when the instruction first begins
        /// </summary>
        public virtual void InstructionBegin() { }
        /// <summary>
        /// Used to report if the instruction has completed or not
        /// </summary>
        /// <returns></returns>
        public abstract bool IsInstructionComplete();
        /// <summary>
        /// Called when the instruction has ended, and the next one is about to begin.
        /// </summary>
        public virtual void InstructionEnd() { }

    }
}