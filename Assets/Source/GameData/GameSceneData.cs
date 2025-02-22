using System;

namespace Source.GameData
{
    
    /// <summary>
    /// Data for a save slot for a specific scene.
    /// </summary>
    [Serializable]
    public class GameSceneData
    {
        /// <summary>
        /// How long across every attempt, the player spent on this chapter.
        /// </summary>
        public TimeSpan TotalTimeSpent { get; set; } = TimeSpan.Zero;
        
        /// <summary>
        /// How many total times the player died on this chapter.
        /// </summary>
        public int Deaths {get;set;} = 0;
        
        public int HighestScore {get;set;} = 0;
        
        
        /// <summary>
        /// The final stats for the level upon completion
        /// </summary>
        public GameStats FinalStats { get; set; } = new();
        
        public bool IsValid()
        {
            // try to detect very sus data
            
            if (TotalTimeSpent.TotalSeconds < 1)
            {
                // No level can be beaten in 1 second :) 
                if (FinalStats is not null)
                    return false;
            }
            
            return FinalStats?.IsValid() ?? true;
                
        
        }
        
    }
}