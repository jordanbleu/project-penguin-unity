using System.Collections.Generic;
using System.Linq;
using Source.Constants;
using Source.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
        private bool firstUpdate = true;
        
        private void Start()
        {
            InitStats();
            var gameplayInstructionList = FindGampeplayInstructions();
            _gameInstructions = new Queue<GameInstruction>(gameplayInstructionList.Skip(startAtElement));
        }

        private void InitStats()
        {
            Stats.Current = new();
            Stats.Current.StartDt = System.DateTime.UtcNow;
            Stats.Current.SceneName = SceneManager.GetActiveScene().name;
            Stats.Current.LivesRemaining = GameplayConstants.TotalLives;
        }

        
        public void FinalizeStats()
        {
            Stats.Current.EndDt = System.DateTime.UtcNow;

            var score = Stats.Current.Score;
            Stats.Current.FinalBaseScore = Stats.Current.Score;
            Stats.Current.TimeBonus = Stats.CalculateTimeBonus();
            Stats.Current.AccuracyBonus = Stats.CalculateAccuracyBonus();
            Stats.Current.NoDeathBonus = Stats.CalculateNoDeathBonus();
            Stats.Current.ComboBonus = Stats.CalculateComboBonus();
            Stats.Current.FinalScore = (score + Stats.Current.TimeBonus + Stats.Current.AccuracyBonus + Stats.Current.NoDeathBonus + Stats.Current.ComboBonus);
            
            // save stats to file
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
            // this is done in update so we can ensure every other
            // game object has had their 'start' method run.
            // Otherwise the game behaves unpredictably because 
            // other objects might run 'start' while the director is doing stuff.
            if (firstUpdate)
            {
                BeginNextInstruction();
                firstUpdate = false;
            }

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