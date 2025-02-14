using System;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Source.SaveData
{
    
    /// <summary>
    /// This is a very rudimentary game data saver and loader.
    /// Everything happens in static classes which is sloppy but that's just the way it is.
    /// </summary>
    public static class GameDataManager
    {
        private const string SaveDataFilename = "etn.json";
        private static readonly string SaveDataPath = Application.dataPath + "/EnterTheNest/";

        public static GlobalData Data { get; private set; } = new GlobalData();
        
        private static string GetFilePath()
            => SaveDataPath + SaveDataFilename;
        
        /// <summary>
        /// Loads the state of GlobalData from disk. Returns true if successful.
        /// </summary>
        public static bool LoadData()
        {
            var filePath = GetFilePath();
            
            if (!File.Exists(filePath))
                return true;

            try
            {
                Data = JsonConvert.DeserializeObject<GlobalData>(File.ReadAllText(filePath));
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to load game save data");;
                Debug.LogException(ex);
                return false;
            }

        }

        /// <summary>
        /// Dumps the state of GlobalData to disk. Returns true if successful.
        /// </summary>
        public static bool SaveData()
        {
            var filePath = GetFilePath();
            
            try
            {
                File.WriteAllText(filePath, JsonConvert.SerializeObject(Data, Formatting.Indented));
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to save game save data");
                Debug.LogException(ex);
                return false;
            }
        }
        
        


    }
}