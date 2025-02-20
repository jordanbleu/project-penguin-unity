using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Source.Constants;
using Source.Data;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

namespace Source.GameData
{
    public class SaveDataManager : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent onBeginSaving = new();
            
        [SerializeField]
        private UnityEvent onDoneSaving = new();

        [SerializeField]
        private UnityEvent onBeginLoading = new();

        [SerializeField]
        private UnityEvent<SaveSlotData> onDoneLoading = new();

        
        private const string DataFileName = "etn{0}.json";

        private string GetDataDirectory()
        {
            return Application.persistentDataPath;
        }
        
        private string GetBackupDirectory()
        {
            return Path.Combine(GetDataDirectory(), "deleted");
        }
        
        /// <summary>
        /// Loads all the saved games.  This only needs to be done for the
        /// save slot menu.
        ///
        /// This is currently not an async process because it only happens once
        /// in the whole game.
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, SaveSlotData> GetAllSaveSlotData()
        {
            var dict = new Dictionary<int, SaveSlotData>();
            var dataDir = GetDataDirectory();
            
            for (var i=0; i<=5;i++)
            {
                // Example C:\Users\user-name\AppData\LocalLow\jbleu\Enter-The-Nest\etn0.json on windows
                var fullFilePath = Path.Combine(dataDir, string.Format(DataFileName, i));
                
                if (!File.Exists(fullFilePath))
                {
                    // return a new game data object, which defaults to IsNew = true
                    dict.Add(i, new SaveSlotData());
                    continue;
                }
                
                SaveSlotData saveSlotData;
                try
                {
                    var json = File.ReadAllText(fullFilePath);
                    saveSlotData = JsonConvert.DeserializeObject<SaveSlotData>(json, SerializerSettings.Default);
                }
                catch (Exception)
                {
                    dict.Add(i, new SaveSlotData() { IsCorrupted = true, IsEmpty = false });
                    continue;
                }
                
                // The user was doing something devious by manually editing the save file
                if (!saveSlotData.IsValid())
                {
                    dict.Add(i, new SaveSlotData() { IsCorrupted = true, IsEmpty = false });
                    continue;
                }
                
                // passes the vibe check, able to load this game :) 
                dict.Add(i, saveSlotData);
            }
            
            return dict;
        }

        /// <summary>
        /// Saves data to disk, from GameStats.  This should be called upon commpletion (failure or success)
        /// of a scene, and the GameStats should be dumped after this is done, otherwise data will be counted twice.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="stats"></param>
        /// <param name="scene"></param>
        /// <param name="sceneIsCompleted"></param>
        /// <param name="onComplete"></param>
        public void BeginSaving(int slot, GameStats stats, GameScene currentScene, bool sceneIsCompleted = false, Action onComplete = null)
        {
            StartCoroutine(SaveStatsCoroutine(slot, stats, currentScene, sceneIsCompleted, onComplete));
        }
        
        private IEnumerator SaveStatsCoroutine(int slot, GameStats stats, GameScene currentScene, bool sceneIsCompleted = false, Action onComplete = null)
        {
            // Step One, load data from disk
            var loadDataCoroutine = LoadDataCoroutine(slot);
            yield return loadDataCoroutine;
            
            onBeginSaving?.Invoke();
            
            // Step two, merge the data
            SaveSlotData existingData = loadDataCoroutine.Current;
            var updatedData = MergeData(existingData, stats, currentScene, sceneIsCompleted);
            
            // Step three, save the data, then done.
            StartCoroutine(SaveDataCoroutine(slot, updatedData, onComplete));
            
            yield return new WaitForSeconds(3);
            onDoneSaving?.Invoke();
        }
        
        /// <summary>
        /// Returns an updated SaveSlotData object with the new stats merged in.
        /// Note this method is poorly written and causes side effects with the existing data.
        /// </summary>
        private SaveSlotData MergeData(SaveSlotData existing, GameStats stats, GameScene currentScene, bool sceneIsCompleted)
        {
            //
            // Calculate the 'next scene' and set it here.
            //      Not needed yet because we only have one scene.
            //
            
            existing.LastUpdateDt = DateTimeOffset.UtcNow;
            
            // Since we are overwriting the data we can just call it good
            existing.IsCorrupted = false;
            existing.IsEmpty =  false;
            
            existing.SceneData.TryGetValue(currentScene, out var sceneData);
            
            sceneData ??= new();
            stats.EndDt = DateTimeOffset.UtcNow;
            
            // merge aggregate data
            var myTimeSpent = stats.EndDt - stats.StartDt;
            
            sceneData.TotalTimeSpent += myTimeSpent;
            sceneData.Deaths += stats.Deaths;
            
            // Merge final stats.  This will only save your latest completion.
            if (sceneIsCompleted)
            {
                var finalScore = StatsCalculator.CalculateFinalScore(stats);

                if (finalScore > sceneData.HighestScore)
                    sceneData.HighestScore = finalScore;

                sceneData.FinalStats = stats;
            }
            
            sceneData.FinalStats.SceneIsCompleted = sceneIsCompleted;
            existing.SceneData[currentScene] = sceneData;
            
            return existing;
        }

        /// <summary>
        /// Saves complete SaveSlotData to disk as is.  Less commonly used than the other save method.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="data"></param>
        /// <param name="onComplete"></param>
        public void BeginSaving(int slot, SaveSlotData data, Action onComplete = null)
        {
            Debug.Log($"Saving data to slot {slot} with name {data?.GameName ?? "Null"}");
            onBeginSaving.Invoke();
            StartCoroutine(SaveDataCoroutine(slot, data, onComplete));
        }
        
        private IEnumerator SaveDataCoroutine(int slot, SaveSlotData data, Action onComplete = null)
        {
            var fullPath = Path.Combine(GetDataDirectory(), string.Format(DataFileName, slot));
            var json = JsonConvert.SerializeObject(data, SerializerSettings.Default);
            
            File.WriteAllText(fullPath, json);
            
            // This is an artificial delay, so the user can see the autosave icon and know it is doing something.
            yield return new WaitForSeconds(3);
            onDoneSaving?.Invoke();
            onComplete?.Invoke();
            Debug.Log($"Finished saving data to slot {slot}");
        }
        
        public void BeginLoading(int saveSlot)
        {
            onBeginLoading.Invoke();
            StartCoroutine(LoadDataCoroutine(saveSlot));
        }

        private IEnumerator<SaveSlotData> LoadDataCoroutine(int slot)
        {
            var fullPath = Path.Combine(GetDataDirectory(), string.Format(DataFileName, slot));
            
            SaveSlotData data;
            
            if (!File.Exists(fullPath))
            {
                data = new();
            }
            else 
            {
                var json = File.ReadAllText(fullPath);
                data = JsonConvert.DeserializeObject<SaveSlotData>(json, SerializerSettings.Default);
            }
            
            onDoneLoading?.Invoke(data);
            yield return data;
        }
        
        public void DeleteSaveSlot(int slot, Action callback = null)
        {
            StartCoroutine(DeleteSaveSlotCoroutine(slot, callback));
        }
        
        private IEnumerator DeleteSaveSlotCoroutine(int slot, Action callback = null)
        {
            var fileName = string.Format(DataFileName, slot);
            var fullPath = Path.Combine(GetDataDirectory(), fileName);
            yield return null;
            
            // ensure backup directory exists
            if (!Directory.Exists(GetBackupDirectory()))
            {
                Directory.CreateDirectory(GetBackupDirectory());
            }
            
            // First make a copy of the file
            var backupPath = Path.Combine(GetBackupDirectory(), $"{DateTime.Now.ToString("s").Replace(":", "_")}{fileName}");
            
            // copy the file to a new file
            File.Copy(fullPath, backupPath);
            
            yield return null;
            
            File.Delete(fullPath);
            callback?.Invoke();
        }
    }
}