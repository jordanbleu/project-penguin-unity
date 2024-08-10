using System;
using Source.Actors;
using Source.Constants;
using Source.Data;
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
            var current = Stats.Current;
            baseScoreText.SetText("Base Score.......... " + current.FinalBaseScore.ToString());
            timeBonusText.SetText("Time Bonus.......... " + current.TimeBonus.ToString());
            accuracyBonusText.SetText("Accuracy Bonus.......... " + current.AccuracyBonus.ToString());
            noDeathBonusText.SetText("No Death Bonus.......... " + current.NoDeathBonus.ToString());
            comboBonusText.SetText("Combo Bonus.......... " + current.ComboBonus.ToString());
            finalScoreText.SetText(current.FinalScore.ToString("n0"));
        }
        
        
        public void Show()
        {
            _isReady = false;
            gameObject.SetActive(true);
            transform.localPosition = new Vector2(0, -200);
            
            LeanTween.moveLocalY(gameObject, 0, 1).setEaseInOutQuart().setOnComplete(()=> _isReady = true);
        }
        
        public void Hide()
        {
            LeanTween.moveLocalY(gameObject, -200, 1).setEaseInOutQuart().setOnComplete(OnDismissed);
        }

        private void OnDismissed()
        {
            onDismissed.Invoke();
            gameObject.SetActive(false);
        }


    }
}