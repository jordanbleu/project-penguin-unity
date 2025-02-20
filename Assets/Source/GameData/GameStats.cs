using System;

namespace Source.GameData
{
    [Serializable]
    public class GameStats
    {
        /// <summary>
        /// Arbitrary base score, gained by kills or whatever the frick I want.
        /// </summary>
        public int BaseScore {get;set;} = 0;
        
        /// <summary>
        /// How many standard bullets the player shot.
        /// </summary>
        public int BulletsFired {get;set;} =0;
        
        
        /// <summary>
        /// How many bullets landed on an enemy
        /// </summary>
        public int BulletsHit {get;set;} = 0;
        
        /// <summary>
        /// How many hit points of damage the player took
        /// </summary>
        public int DamageTaken {get;set;} = 0;
        
        /// <summary>
        /// How many hit points did you block with your shield?
        /// </summary>
        public int DamageBlockedByShield{get;set;} =0;
        
        public DateTimeOffset StartDt {get;set;} = DateTimeOffset.UtcNow;
        
        /// <summary>
        /// The time the player finished the level.
        /// </summary>
        public DateTimeOffset EndDt {get;set;}
        
        /// <summary>
        /// How much damage was dealt to enemies.
        /// </summary>
        public int DamageDealt {get;set;} = 0;
        
        /// <summary>
        /// How many times the player died (just for the session)
        /// </summary>
        public int Deaths {get;set;} =0;
        
        /// <summary>
        /// The highest combo the player has gotten.
        /// </summary>
        public int BestCombo {get;set;} = 0;
        
        /// <summary>
        /// How many times the player has dashed.
        /// </summary>
        public int Dashes {get;set;} = 0;
        
        /// <summary>
        /// How many times the player used the laser
        /// </summary>
        public int Lasers {get;set;} = 0;
        
        /// <summary>
        /// How many times the player launched a mine
        /// </summary>
        public int Mines {get;set;} = 0;
        
        /// <summary>
        /// How many missiles the player launched.
        /// </summary>
        public int Missiles {get;set;} = 0;
        
        /// <summary>
        /// How many shields the palyer used.
        /// </summary>
        public int Shields {get;set;} = 0;
        
        /// <summary>
        /// How many forcefields the player launched.
        /// </summary>
        public int ForceFields {get;set;} = 0;

        public bool SceneIsCompleted {get;set;} = false;

        public bool IsValid()
        {
            // You hit more enemies than you even shot bullets? Nice try hacker.
            if (BulletsHit > BulletsFired)
                return false;
            
            // Can't end the level before you even started (this would break stuff)
            if (EndDt < StartDt)
                return false;
            
            // no levels can be beaten without KILLING
            if (DamageDealt <=0)
                return false;
           
            return true;
        }
    }
}