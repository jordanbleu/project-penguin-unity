using System;
using System.Collections.Generic;

namespace Source.Data
{
    public static class Stats
    {
        public static GameStats Current { get; set;  } = new();

        // List of scenes and the time it should take to play them
        private static readonly Dictionary<string, TimeSpan> ScenePlayTimes = new()
        {
            { "30_Opening", new TimeSpan(0, 20, 00) }
        };
            
        /// <summary>
        /// Adds points to the current score if applicable.
        /// </summary>
        /// <param name="points"></param>
        public static void AddToScore(int points)
        {
            if (Current is not null)
                Current.Score += points;
        }

        public static void TrackBulletFired()
        {
            if (Current is null)
                return;
            
            Current.BulletsFired++;
            Current.BulletsFiredCombo++;
            
            if (Current.BulletsFiredCombo > Current.BiggestCombo)
                Current.BiggestCombo = Current.BulletsFiredCombo;
        }

        public static void ResetBulletsFiredCombo()
        {
            if (Current is null)
                return;
            
            Current.BulletsFiredCombo = 0;
        }

        public static void TrackPlayerDash()
        {
            if (Current is null)
                return;
            
            Current.TotalDashes++;
        }
        
        public static void TrackLaser()
        {
            if (Current is null)
                return;
            
            Current.TotalLasers++;
        }
        
        public static void TrackMissile()
        {
            if (Current is null)
                return;
            
            Current.TotalMissiles++;
        }
        
        public static void TrackMine()
        {
            if (Current is null)
                return;
            
            Current.TotalMines++;
        }

        public static void TrackShield()
        {
            if (Current is null)
                return;
            
            Current.TotalShields++;
        }
        
        public static void TrackForceField()
        {
            if (Current is null)
                return;
            
            Current.TotalForceFields++;
        }
        
        public static void TrackDamageTaken(int damage)
        {
            if (Current is null)
                return;
            
            Current.DamageTaken += damage;
        }
        
        public static void TrackDamageBlockedByShield(int damage)
        {
            if (Current is null)
                return;
            
            Current.DamageBlockedByShield += damage;
        }
        
        public static void TrackDamageDealt(int damage)
        {
            if (Current is null)
                return;
            
            Current.DamageDealt += damage;
        }

        public static float CalculateShotAccuracy()
        {
            if (Current is null || Current.BulletsFired == 0)
                return 0f;
            
            return (float)Current.BulletsHit / Current.BulletsFired;
        }

        public static void TrackBulletHit()
        {
            if (Current is null)
                return;

            Current.BulletsHit++;
        }

        public static int CalculateTimeBonus()
        {
            if (Current is null)
                return 0;
            var sceneId = Current.SceneName;
            var standardTime = ScenePlayTimes[sceneId];
            var myTime = (Current.EndDt - Current.StartDt);

            var bonus = 10000;

            if (myTime < standardTime)
                return bonus;

            var percent = standardTime.TotalSeconds / myTime.TotalSeconds;

            return (int)(percent * bonus);
        }

        public static int CalculateAccuracyBonus()
        {
            if (Current is null)
                return 0;
            
            var bonus = 10000;

            return (int)(bonus * CalculateShotAccuracy());
        }

        public static int CalculateNoDeathBonus()
        {
            if (Current is null)
                return 0;
                
            var bonus = 30000;

            return Current.Deaths == 0 ? bonus : 0;
        }

        public static int CalculateComboBonus()
        {
            if (Current is null)
                return 0;

            var maxComboPercent = Math.Min(Current.BiggestCombo, 100) / 100f;
            
            var bonus = 100000;

            return (int)(bonus * maxComboPercent);
        }


    }

    public class GameStats
    {

        public int TimeBonus { get; set; }
        public int AccuracyBonus { get; set; }
        public int NoDeathBonus { get; set; }
        public int ComboBonus { get; set; }
        

        /// <summary>
        /// Bullets that were fired by the player, regardless if they hit anything 
        /// </summary>
        public int BulletsFired { get; set; }

        public int TotalDashes { get; set; }

        public int TotalLasers { get; set; }
        public int TotalMissiles { get; set; }
        public int TotalMines { get; set; }
        public int TotalShields { get; set; }

        public int TotalForceFields { get; set; }

        /// <summary>
        /// How many bullets were fired in a row without missing
        /// </summary>
        public int BulletsFiredCombo { get; set; }

        public int BiggestCombo { get; set; }

        /// <summary>
        /// Bullets that were fired by the player that hit an enemy
        /// </summary>
        public int BulletsHit { get; set; }
        
        /// <summary>
        /// The total damage taken by the player
        /// </summary>
        public int DamageTaken { get; set; }

        public int DamageBlockedByShield { get; set; }

        public DateTime StartDt { get; set; }
        public DateTime EndDt { get; set; }

        public int Score { get; set; }
        
        /// <summary>
        /// The base score once stats are finalized 
        /// </summary>
        public int FinalBaseScore { get; set; }
        public int FinalScore { get; set; }

        /// <summary>
        /// How much damadge was dealt to any Attackable.
        /// </summary>
        public int DamageDealt { get; set; }

        public int Deaths { get; set; }

        public string SceneName { get; set; }
    }


}