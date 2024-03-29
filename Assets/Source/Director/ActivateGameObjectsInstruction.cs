using System.Collections.Generic;
using UnityEngine;

namespace Source.Director
{
    /// <summary>
    /// This is used to set one or more gameobjects to active when the instruction is run.
    ///
    /// This can be used to 'spawn' enemies, or other events.  The instruction is automatically
    /// set to complete immediately. After the enemies are set to active.  
    /// </summary>
    [Tooltip("Activates the specified gameObject(s)")]
    public class ActivateGameObjectInstruction : GameInstruction
    {
        [SerializeField] 
        [Tooltip("GameObject(s) that will be set to active")]
        private List<GameObject> gameObjectsToActivate = new();

        public override void InstructionBegin()
        {
            foreach (var obj in gameObjectsToActivate)
            {
                obj.SetActive(true);
            }
        }

        public override bool IsInstructionComplete()
        {
            return true;
        }

        public override void InstructionEnd()
        {
        }
    }
}