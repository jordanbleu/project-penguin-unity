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
    public class EndingFinalScoreDisplay : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI baseScoreText;
        
        [SerializeField]
        private TextMeshProUGUI timeBonusText;
        
        [SerializeField]
        private TextMeshProUGUI accuracyBonusText;
        
        [SerializeField]
        private TextMeshProUGUI noDeathBonusText;
        
        [SerializeField]
        private TextMeshProUGUI comboBonusText;
        
        [SerializeField]
        private TextMeshProUGUI finalScoreText;

        [SerializeField]
        private UnityEvent onDismissed = new();

        private bool _isReady = false;

        private StatsTracker _statsTracker;
        
        private void Start()
        {
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
            if (!_isReady)
                return;
            
            Hide();
            _isReady = false;
        }

        private void OnEnable()
        {
                        
            _statsTracker = GameObject.FindWithTag(Tags.StatsTracker)?.GetComponent<StatsTracker>() 
                            ?? throw new UnityException("No stats tracker in scene.");
            
            var stats = _statsTracker.Stats;

            // time bonus
            var timeSpent = stats.EndDt - stats.StartDt;
            var timeBonus = StatsCalculator.CalculateTimeBonus(timeSpent);
            
            // accuracy bonus
            var accuracy = StatsCalculator.CalculateShotAccuracy(stats.BulletsHit, stats.BulletsFired);
            var accuracyBonus = StatsCalculator.CalculateAccuracyBonus(accuracy);
            
            // no deaths
            var noDeathBonus = StatsCalculator.CalculateNoDeathBonus(stats.Deaths);
            
            // combonus
            var comboBonus = StatsCalculator.CalculateComboBonus(stats.BestCombo);
            
            // final score, which literally does everything above again lol
            var finalScore = StatsCalculator.CalculateFinalScore(stats);
            
            baseScoreText.SetText("Base Score.......... " + stats.BaseScore.ToString("N0"));
            timeBonusText.SetText("Time Bonus.......... " + timeBonus.ToString("N0"));
            accuracyBonusText.SetText("Accuracy Bonus.......... " + accuracyBonus.ToString("N0"));
            noDeathBonusText.SetText("No Death Bonus.......... " + noDeathBonus.ToString("N0"));
            comboBonusText.SetText("Combo Bonus.......... " + comboBonus.ToString("N0"));
            
            finalScoreText.SetText(finalScore.ToString("n0"));
        }
        
        
        public void Show()
        {
            _isReady = false;
            gameObject.SetActive(true);
            transform.localPosition = new Vector2(0, -200);
            
            LeanTween.moveLocalY(gameObject, 0, 0.5f).setEaseOutBack().setOnComplete(()=> _isReady = true);
        }
        
        public void Hide()
        {
            LeanTween.moveLocalY(gameObject, -200, 0.5f).setEaseInBack().setOnComplete(OnDismissed);
        }

        private void OnDismissed()
        {
            onDismissed.Invoke();
            gameObject.SetActive(false);
        }


    }
}