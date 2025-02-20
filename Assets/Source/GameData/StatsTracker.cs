using System;
using System.Collections;
using System.Collections.Generic;
using Editor;
using Source.Constants;
using UnityEngine;

namespace Source.GameData
{
    /// <summary>
    /// Tracks stats for your current playthrough.
    /// </summary>
    public class StatsTracker : MonoBehaviour
    {
        public GameStats Stats { get; } = new();
        
        [SerializeField]
        [Tooltip("Which scene is this? (saved into the save data when the scene is completed)")]
        private GameScene gameScene;
        
        private SaveDataManager _saveDataManager;
        
        [SerializeField]
        [ReadOnlyField]
        private int baseScore = 0;

        [SerializeField]
        [ReadOnlyField]
        private int bulletsFired = 0;

        [SerializeField]
        [ReadOnlyField]
        private int bulletsHit = 0;

        [SerializeField]
        [ReadOnlyField]
        private int damageTaken = 0;

        [SerializeField]
        [ReadOnlyField]
        private string startDt;

        [SerializeField]
        [ReadOnlyField]
        private string endDt;

        [SerializeField]
        [ReadOnlyField]
        private int damageDealt = 0;

        [SerializeField]
        [ReadOnlyField]
        private int deaths = 0;

        [SerializeField]
        [ReadOnlyField]
        private int bestCombo = 0;

        [SerializeField]
        [ReadOnlyField]
        private int dashes = 0;

        [SerializeField]
        [ReadOnlyField]
        private int lasers = 0;

        [SerializeField]
        [ReadOnlyField]
        private int mines = 0;

        [SerializeField]
        [ReadOnlyField]
        private int missiles = 0;

        [SerializeField]
        [ReadOnlyField]
        private int shields = 0;

        [SerializeField]
        [ReadOnlyField]
        private int forceFields = 0;

        public bool IsCurrentlySaving {get;set;} = false;

        private void Start()
        {
            _saveDataManager = GameObject.FindWithTag(Tags.SaveDataManager)?.GetComponent<SaveDataManager>()
                ?? throw new UnityException("No save data manager in scene");

        }

        private void Update()
        {
            // this is only for display in the inspector
            baseScore = Stats.BaseScore;
            bulletsFired = Stats.BulletsFired;
            bulletsHit = Stats.BulletsHit;
            damageTaken = Stats.DamageTaken;
            startDt = Stats.StartDt.ToString();
            endDt = Stats.EndDt.ToString();
            damageDealt = Stats.DamageDealt;
            deaths = Stats.Deaths;
            bestCombo = Stats.BestCombo;
            dashes = Stats.Dashes;
            lasers = Stats.Lasers;
            mines = Stats.Mines;
            missiles = Stats.Missiles;
            shields = Stats.Shields;
            forceFields = Stats.ForceFields;
        }
        
        
        
        /// <summary>
        /// This will asyncronously begin the autosave process.
        /// Make darn sure that you are using the onCompleted callback to prevent
        /// a scene change or something that would mess up the save.
        /// </summary>
        public void BeginSaving(bool sceneIsCompleted, Action onCompleted = null)
        {
            IsCurrentlySaving = true;
            
            var slot = GlobalSaveDataManager.GlobalData.SelectedSaveSlot;
            var currentScene = gameScene;
            
            _saveDataManager.BeginSaving(slot, Stats, currentScene, sceneIsCompleted,
                ()=>
                    {
                        IsCurrentlySaving = false;
                        onCompleted?.Invoke();
                    });
        }
        
        
    }


}