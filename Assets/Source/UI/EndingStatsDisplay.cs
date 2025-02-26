using System;
using Source.Actors;
using Source.Constants;
using Source.Data;
using Source.GameData;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Source.UI
{
    public class EndingStatsDisplay : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI shotsFiredStat;
        
        [SerializeField]
        private TextMeshProUGUI accuracyStat;
        
        [SerializeField]
        private TextMeshProUGUI biggestComboStat;

        [SerializeField]
        private TextMeshProUGUI damageTakenStat;
        
        [SerializeField]
        private TextMeshProUGUI damageDealtStat;

        [SerializeField]
        private TextMeshProUGUI playTimeStat;

        [SerializeField]
        private TextMeshProUGUI deathsStat;

        [SerializeField]
        private TextMeshProUGUI rankStat;

        [SerializeField]
        private UnityEvent onDismiss = new();
        
        private Animator _animator;
        
        private StatsTracker _statsTracker;
        
        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            _statsTracker = GameObject.FindWithTag(Tags.StatsTracker)?.GetComponent<StatsTracker>() 
                ?? throw new UnityException("No stats tracker in scene.");
            
            var currentStats = _statsTracker.Stats;
            
            shotsFiredStat.SetText("Shots\nFired:\n" + currentStats.BulletsFired);
            accuracyStat.SetText("Accuracy:\n" + FormatAccuracy(StatsCalculator.CalculateShotAccuracy(currentStats.BulletsHit, currentStats.BulletsFired)));
            biggestComboStat.SetText("Biggest\nCombo:\n" + currentStats.BestCombo);
            damageTakenStat.SetText("Damage\nTaken:\n" + currentStats.DamageTaken);
            damageDealtStat.SetText("Damage\nDealt:\n" + currentStats.DamageDealt);
            playTimeStat.SetText("Play Time:\n" + FormatPlayTime(currentStats.EndDt - currentStats.StartDt));
            deathsStat.SetText("Deaths:\n" + currentStats.Deaths);
            rankStat.SetText("Rank:\n" + StatsCalculator.CalculateRank(currentStats.Deaths, currentStats.BulletsFired, currentStats.BulletsHit, currentStats.DamageTaken).ToDisplayString());
            
            var player = GameObject.FindGameObjectWithTag(Tags.Player);

            if (player != null)
            {
                if (player.TryGetComponent<Player>(out var playerComp))
                {
                    playerComp.AddMenuEnterEventListener(Dismiss);
                }
            }
        }

        private void Dismiss()
        {
            _animator.SetTrigger("hide");
            onDismiss?.Invoke();
        }

        private string FormatPlayTime(TimeSpan timeTaken)
        {
            return timeTaken.ToString(@"hh\:mm\:ss\.fff");
        }

        private string FormatAccuracy(float accuracy)
        {
            return (accuracy * 100f).ToString("F2") + "%";
        }
    }
}