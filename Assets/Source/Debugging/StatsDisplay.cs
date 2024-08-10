using System;
using Source.Data;
using UnityEngine;

namespace Source.Debugging
{
    public class StatsDisplay : MonoBehaviour
    {
        [SerializeField] private int totalMines;
        [SerializeField] private int totalShields;
        [SerializeField] private int totalForceFields;
        [SerializeField] private int bulletsFiredCombo;
        [SerializeField] private int biggestCombo;
        [SerializeField] private int bulletsHit;
        [SerializeField] private int damageTaken;
        [SerializeField] private int damageBlockedByShield;
        [SerializeField] private DateTime startDt;
        [SerializeField] private DateTime endDt;
        [SerializeField] private int baseScore;
        [SerializeField] private int damageDealt;

        [SerializeField]
        private int totalDashes;

        public void Update()
        {
            if (Stats.Current == null) return;

            totalMines = Stats.Current.TotalMines;
            totalDashes = Stats.Current.TotalDashes;
            totalShields = Stats.Current.TotalShields;
            totalForceFields = Stats.Current.TotalForceFields;
            bulletsFiredCombo = Stats.Current.BulletsFiredCombo;
            biggestCombo = Stats.Current.BiggestCombo;
            bulletsHit = Stats.Current.BulletsHit;
            damageTaken = Stats.Current.DamageTaken;
            damageBlockedByShield = Stats.Current.DamageBlockedByShield;
            startDt = Stats.Current.StartDt;
            endDt = Stats.Current.EndDt;
            baseScore = Stats.Current.Score;
            damageDealt = Stats.Current.DamageDealt;
        }
    }
}