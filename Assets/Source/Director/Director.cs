using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Source.Director
{
    /// <summary>
    /// The director is in charge of managing the overall game flow.
    ///
    /// Each 'segment' is a gameObject with an ISegment component on it.  The director will
    /// cycle through each wave, deactivating previous game objects.
    ///
    /// Segments can include spawning enemies, time delays, dialogue, etc.
    /// </summary>
    public class Director : MonoBehaviour
    {
        [SerializeField] 
        [Tooltip("List of game objects, each containing one component inheriting from `GameInstruction`.")]
        private List<GameInstruction> gameplayInstructionList; 
        
        [SerializeField] 
        [Tooltip("What to do when all segments are completed")]
        private UnityEvent onSegmentsCompleted = new();

        private Queue<GameInstruction> _gameInstructions;
        private GameInstruction _currentInstruction;
        
        private void Start()
        {
            _gameInstructions = new Queue<GameInstruction>(gameplayInstructionList);
            BeginNextInstruction();
        }

        private void Update()
        {
            if (_currentInstruction is null)
                return;

            if (_currentInstruction.IsInstructionComplete())
                BeginNextInstruction();
            
        }

        private void BeginNextInstruction()
        {
            if (_currentInstruction is not null)
            {
                _currentInstruction.InstructionEnd();
                _currentInstruction.gameObject.SetActive(false);
            }

            if (!_gameInstructions.TryDequeue(out _currentInstruction))
            {
                _currentInstruction = null;
                onSegmentsCompleted.Invoke();
                return;
            }

            _currentInstruction.gameObject.SetActive(true);
            _currentInstruction.InstructionBegin();
            
        }




    }
}