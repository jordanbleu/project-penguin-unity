using System;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;

namespace Source.GameData
{
    /// <summary>
    /// Represents data that is scoped to a save slot.
    /// </summary>
    [Serializable]
    public class SaveSlotData
    {
        /// <summary>
        /// Which scene is the player currently on.
        /// </summary>
        public GameScene CurrentScene { get; set; } = GameScene.Beginning;
        
        public DateTimeOffset LastUpdateDt { get; set; }
        
        public DateTimeOffset CreateDt { get; set; }
        
        public string GameName { get; set; } = "TestGame";
        
        /// <summary>
        /// If this is set to true, the save slot should be considered as empty, even if there is data.
        /// We don't delete files around here :) 
        /// </summary>
        public bool IsEmpty {get;set;} = true;
        
        /// <summary>
        /// If the file fails to deserialize, this will be set to true.
        /// This field shouldn't be serialized or deserialized, only set in code.
        /// </summary>
        [JsonIgnore]
        public bool IsCorrupted {get;set;} = false;
        
        /// <summary>
        /// Final stats for each scene (if any)
        /// </summary>
        public Dictionary<GameScene, GameSceneData> SceneData { get; set; } = new();
        
        public bool IsValid()
        {
            if (IsEmpty)
                return true;
            
            if (GameName.Length > 16)
                return false;
            
            if (!Enum.IsDefined(typeof(GameScene), CurrentScene))
                return false;
            
            if (SceneData.Count > Enum.GetValues(typeof(GameScene)).Length)
                return false;
            
            foreach (var kvp in SceneData)
            {
                if (!kvp.Value.IsValid())
                    return false;
            }
            
            return true;
        }

        
    }
}