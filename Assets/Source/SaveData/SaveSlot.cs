using System;
using System.Collections.Generic;
using Source.Data;

namespace Source.SaveData
{
    /// <summary>
    /// Represents data scoped to a single save slot.  Game progress settings / etc go here.
    /// </summary>
    public class SaveSlot
    {
        /// <summary>
        /// Which scene is the player currently on 
        /// </summary>
        public GameScene CurrentScene { get; set; } = GameScene.Beginning;

        /// <summary>
        /// When was this data last updated
        /// </summary>
        public DateTime? LastUpdated { get; set; }
        
        /// <summary>
        /// When was this data first created
        /// </summary>
        public DateTime? CreateDate { get; set; }
        
        /// <summary>
        /// Specific data for each scene.
        /// </summary>
        public Dictionary<GameScene, SceneData> SceneSpecificData { get; set; } = new Dictionary<GameScene, SceneData>();
        
        /// <summary>
        /// Specific Name for the game 
        /// </summary>   
        public string GameName { get; set; } = string.Empty;

        /// <summary>
        /// If true, this save slot should be considered as empty
        /// </summary>
        public bool IsNew { get; set; } = true;

    }
    
    /// <summary>
    /// Statistics and data for a specific scene.
    /// </summary>
    public class SceneData
    {
        public TimeSpan TimeSpent { get; set; } = TimeSpan.Zero;
        
        public int TotalDeaths { get; set; } = 0;
    }
}