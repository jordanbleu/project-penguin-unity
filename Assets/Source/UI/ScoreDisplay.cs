using System;
using Source.Data;
using TMPro;
using UnityEngine;

namespace Source.UI
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _scoreText;
        
        private void Update()
        {
            _scoreText.SetText(Stats.Current.Score.ToString().PadLeft(10, '0'));
        }
    }
}