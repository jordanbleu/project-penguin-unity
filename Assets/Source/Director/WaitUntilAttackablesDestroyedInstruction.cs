using System.Collections.Generic;
using Source.Behaviors;
using UnityEngine;

namespace Source.Director
{
    [Tooltip("Waits until all the specified attackable objects are destroyed.")]
    public class WaitUntilAttackablesDestroyedInstruction : GameInstruction
    {
        [SerializeField]
        [Tooltip("List of single attackables to check")]
        private List<Attackable> attackables = new();

        [SerializeField]
        [Tooltip(("Drag any parent objects here to check all children attackables"))]
        private List<GameObject> attackableParents;
        
        protected override void OnComponentStartCalled()
        {
            foreach (var obj in attackableParents)
            {
                attackables.AddRange(obj.GetComponentsInChildren<Attackable>(true));
            }

            base.OnComponentStartCalled();
        }

        public override bool IsInstructionComplete()
        {
            foreach (var attackable in attackables)
            {
                if (attackable.WasDestroyed || attackable == null || attackable.gameObject == null)
                    continue;
                // if we find an object that is still alive, return false
                return false;
            }

            return true;
        }
    }
}