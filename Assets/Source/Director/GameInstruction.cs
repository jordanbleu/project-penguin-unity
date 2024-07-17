using System;
using UnityEngine;

namespace Source.Director
{
    public abstract class GameInstruction : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("If unchecked, the instruction will be skipped.  Used for local testing.")]
        private bool isEnabled = true;

        public bool IsEnabled => isEnabled;

        private void Start()
        {
            OnComponentStartCalled();
        }

        protected virtual void OnComponentStartCalled()
        {
        }

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