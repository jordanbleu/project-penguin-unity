using Source.Actors;
using UnityEngine;

namespace Source.Director
{
    public class WaitForNPCMovementInstruction : GameInstruction
    {
        [SerializeField]
        private NPC npc;
        
        public override bool IsInstructionComplete()
        {
            return npc.HasArrived();
        }
    }
}