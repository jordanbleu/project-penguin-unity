using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using Source.Constants;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Source.GameData
{
    /// <summary>
    /// GameObject that is responsible for saving and loading game data.
    /// </summary>
    public class GlobalSaveDataManager : MonoBehaviour
    {
        private const string GlobalDataFileName = "etn-global.json";
        
        /// <summary>
        /// Global Data used across the entire game.
        /// </summary>
        public static GlobalData GlobalData {get;set;} = new();
        
        [SerializeField]
        private UnityEvent onBeginLoadingGlobalData = new();

        [SerializeField]
        private UnityEvent onGlobalDataLoaded = new();
       
        [SerializeField]
        private UnityEvent onBeginSavingGlobalData = new();

        [SerializeField]
        private UnityEvent onGlobalDataSaved = new();

        private string GetDataDirectory()
        {
            return Application.persistentDataPath;
        }
        
        /// <summary>
        /// Should be called on game start.  Will begin an asyncronous loading operation
        /// of user data from disk.
        /// </summary>
        public void BeginLoadingGlobalData()
        {
            onBeginLoadingGlobalData.Invoke();
            StartCoroutine(LoadGlobalDataCoroutine());
        }
        
        private IEnumerator LoadGlobalDataCoroutine()
        {
            var fullPath = Path.Combine(GetDataDirectory(), GlobalDataFileName);
            
            if (!File.Exists(fullPath))
            {
                GlobalData = new GlobalData();
                onGlobalDataLoaded.Invoke();
                yield break;
            }
            
            var json = File.ReadAllText(fullPath);
            GlobalData = JsonConvert.DeserializeObject<GlobalData>(json, SerializerSettings.Default);
            onGlobalDataLoaded.Invoke();
        }
        
        public void BeginSavingGlobalData()
        {
            onBeginSavingGlobalData.Invoke();
            StartCoroutine(SaveGlobalDataCoroutine());
        }

        private IEnumerator SaveGlobalDataCoroutine()
        {
            var fullPath = Path.Combine(GetDataDirectory(), GlobalDataFileName);
            var json = JsonConvert.SerializeObject(GlobalData, SerializerSettings.Default);
            
            File.WriteAllText(fullPath, json);
            
            onGlobalDataSaved.Invoke();
            yield return null;
        }
    }
}