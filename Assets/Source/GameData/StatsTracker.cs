using System;
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
        private int baseScore = 0;

        [SerializeField]
        private int bulletsFired = 0;

        [SerializeField]
        private int bulletsHit = 0;

        [SerializeField]
        private int damageTaken = 0;

        [SerializeField]
        private string startDt;

        [SerializeField]
        private string endDt;

        [SerializeField]
        private int damageDealt = 0;

        [SerializeField]
        private int deaths = 0;

        [SerializeField]
        private int bestCombo = 0;

        [SerializeField]
        private int dashes = 0;

        [SerializeField]
        private int lasers = 0;

        [SerializeField]
        private int mines = 0;

        [SerializeField]
        private int missiles = 0;

        [SerializeField]
        private int shields = 0;

        [SerializeField]
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