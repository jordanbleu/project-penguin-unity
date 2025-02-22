using Source.Constants;
using Source.GameData;
using UnityEngine;

namespace Source.Director
{
    /// <summary>
    /// Calls the save method, then waits for it to finish before continuing.
    /// </summary>
    public class SaveStatsInstruction : GameInstruction
    {
        [SerializeField]
        [Tooltip("The director will not continue until the save operation is finished.")]
        private bool WaitBeforeContinuing = true;
        
        [SerializeField]
        [Tooltip("Marks the scene as completed in the save data.")]
        private bool IsSceneCompleted = false;
        
        private bool _isComplete = false;

        public override void InstructionBegin()
        {
            var statTracker = GameObject.FindWithTag(Tags.StatsTracker)?.GetComponent<StatsTracker>()
                ?? throw new UnityException("No stat tracker in scene");
            
            statTracker.BeginSaving(IsSceneCompleted, () => _isComplete = true);
        }

        public override bool IsInstructionComplete()
        {
            return _isComplete || !WaitBeforeContinuing;
        }
    }
}