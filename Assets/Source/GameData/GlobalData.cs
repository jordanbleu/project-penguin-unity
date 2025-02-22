using System;
using Source.Data;
using Unity.Plastic.Newtonsoft.Json;

namespace Source.GameData
{
    /// <summary>
    /// Represents data that is agnostic of any save slot.
    /// </summary>
    [Serializable]
    public class GlobalData
    {
        /// <summary>
        /// Which color filter to use
        /// </summary>
        public ColorFilterOption ColorFilter { get;set; } = ColorFilterOption.Charcoal;
        
        /// <summary>
        /// Loudness of music
        /// </summary>
        public float MusicVolume { get; set; } = 1f;
        
        /// <summary>
        /// Loudness of SFX
        /// </summary>
        public float SfxVolume { get; set; } = 1f;
        
        /// <summary>
        /// The last used save slot.  Used to tell the saver / loader which slot to load / save from.
        /// I haven't decided if this should be saved into json or not.  The user can easily  corrupt the value
        /// obviously which is bad, but it would come in handy so we can add a "Continue" option from the main menu.
        /// </summary>
        [JsonIgnore]
        public int SelectedSaveSlot { get; set; } = 0;

    }
}