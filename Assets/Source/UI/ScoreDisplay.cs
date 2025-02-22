using System;
using Source.Constants;
using Source.Data;
using Source.GameData;
using TMPro;
using UnityEngine;

namespace Source.UI
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _scoreText;
        
        private StatsTracker _statsTracker;

        private void Start()
        {
            _statsTracker = GameObject.FindWithTag(Tags.StatsTracker)?.GetComponent<StatsTracker>()
                ?? throw new UnityException("No stats tracker in scene");
        }

        private void Update()
        {
            var score = _statsTracker.Stats.BaseScore;
            _scoreText.SetText(score.ToString().PadLeft(10, '0'));
        }
    }
}