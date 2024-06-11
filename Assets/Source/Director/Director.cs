using System.Collections.Generic;
using System.Linq;
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
        // [SerializeField] 
        // [Tooltip("List of game objects, each containing one component inheriting from `GameInstruction`.")]
        // private List<GameInstruction> gameplayInstructionList; 
        
        [SerializeField] 
        [Tooltip("What to do when all segments are completed")]
        private UnityEvent onSegmentsCompleted = new();

        [SerializeField]
        [Tooltip("Used for local testing, skips to a specific segment")]
        private int startAtElement = 0;
        
        private Queue<GameInstruction> _gameInstructions;
        private GameInstruction _currentInstruction;
        
        private void Start()
        {
            var gameplayInstructionList = FindGampeplayInstructions();
            _gameInstructions = new Queue<GameInstruction>(gameplayInstructionList.Skip(startAtElement));
            BeginNextInstruction();
        }
        
        private List<GameInstruction> FindGampeplayInstructions()
        {
            // Get all GameInstruction components in children
            var instructions = gameObject.GetComponentsInChildren<GameInstruction>(true);

            var index = 0;
            
            // Make the inspector names more helpful
            foreach (var gi in instructions)
            {
                var name = gi.gameObject.name;
                if (!gi.IsEnabled)
                {
                    gi.name = "[[DISABLED]]" + index  + name;
                }
                else
                {
                    gi.name = index + ": " + name;
                }

                index++;
            }
            
            // Sort the array based on the sibling index of each GameObject
            var sortedInstructions = instructions
                .Where(gi=>gi.IsEnabled)
                .OrderBy(instruction => instruction.transform.GetSiblingIndex())
                
                .ToList();

            return sortedInstructions;
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