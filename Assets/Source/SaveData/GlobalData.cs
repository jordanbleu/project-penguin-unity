using System;
using System.Collections.Generic;
using Source.Data;

namespace Source.SaveData
{
    /// <summary>
    /// Represents save data that is scoped for all save slots.  Settings / etc go here. 
    /// </summary>
    [Serializable]
    public class GlobalData
    {
        public ColorFilterOption ColorFilterOption { get; set; } = ColorFilterOption.Charcoal;

        /// <summary>
        /// Individual game data for all save slots.  ETN supports 5 slots.
        /// </summary>
        public Dictionary<int, SaveSlot> SaveSlots { get; set; } = new Dictionary<int, SaveSlot>()
        {
            {
                // todo: this is to test loading data, and should be removed
                0, new()
                {
                    GameName = "Jordan Test",
                    LastUpdated = DateTime.Now,
                    IsNew = false,
                    CurrentScene = GameScene.Beginning
                }
            },
            { 1, new() },
            { 2, new() },
            { 3, new() },
            { 4, new() },
        };
        
        /// <summary>
        /// Which save slot is currently being used.
        /// </summary>
        public int SaveSlotIndex { get; set; } = 0;

        /// <summary>
        /// Version of the save file in case we need to update the json format
        /// </summary>
        public int Version { get; set; } = 0;
        
        /// <summary>
        /// This should be set to false as soon as literally anything is saved.
        /// </summary>
        public bool IsFirstLoad { get; set; } = true;

    }
}