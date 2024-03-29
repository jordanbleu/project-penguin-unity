using System.Collections.Generic;
using Source.Behaviors;
using UnityEngine;

namespace Source.Director
{
    [Tooltip("Waits until all the specified attackable objects are destroyed.")]
    public class WaitUntilAttackablesDestroyedInstruction : GameInstruction
    {
        [SerializeField]
        [Tooltip("List of attackables to check")]
        private List<Attackable> attackables = new();

        public override bool IsInstructionComplete()
        {
            foreach (var attackable in attackables)
            {
                if (attackable.WasDestroyed)
                    continue;
                // if we find an object that is still alive, return false
                return false;
            }

            return true;
        }
    }
}