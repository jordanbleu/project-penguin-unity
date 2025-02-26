using System;
using Source.Actors;
using Source.Data;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace Source.UI
{
    public class ComboDisplay : MonoBehaviour
    {
        private Animator _animator;
        private static readonly int IsComboActive = Animator.StringToHash("is-displayed");

        [SerializeField]
        private TextMeshProUGUI _comboText;

        private Player _player;
        
        private void Start()
        {
            _animator = GetComponent<Animator>();
            
            _player = GameObject.FindWithTag("Player")?.GetComponent<Player>()
                ?? throw new UnityException("Missing player in scene");
        }

        private void Update()
        {
            var combo = _player.CurrentCombo;
            
            if (combo > 5)
            {
                _animator.SetBool(IsComboActive, true);
            }
            else
            {
                _animator.SetBool(IsComboActive, false);
            }

            _comboText.text = GetDisplayString(combo);
        }

        private static string GetDisplayString(int combo)
        {
            var formattedCombo = combo.ToString("n0");

            if (combo <= 10)
                return $"{formattedCombo} Hit Combo!";

            if (combo <= 20)
                return $"{formattedCombo} Hit Super Combo!";

            if (combo <= 25)
                return $"{formattedCombo} Hit Mega-Combo!";

            if (combo <= 30)
                return $"{formattedCombo} Hit THICK Combo!";

            if (combo <= 35)
                return $"{formattedCombo} Hit Juicy Combo!";

            if (combo <= 40)
                return $"{formattedCombo} Hit Crispy Combo!";

            if (combo <= 45)
                return $"{formattedCombo} Hit Spicy Combo!";

            if (combo <= 50)
                return $"{formattedCombo} Hit Gorgeous Combo!";

            if (combo <= 55)
                return $"{formattedCombo} Hit Toasty Combo!";

            if (combo <= 60)
                return $"{formattedCombo} Hit Alarmingly Massive Combo!";

            if (combo <= 65)
                return $"{formattedCombo} Hit Creamy Combo!";

            if (combo <= 70)
                return $"{formattedCombo} Hit Voluptuous Combo!";

            if (combo <= 75)
                return $"{formattedCombo} Hit Stanky Combo!";

            if (combo <= 80)
                return $"{formattedCombo} Hit Galaxy-Sized Combo!";

            if (combo <= 85)
                return $"{formattedCombo} Hit Hauntingly Big Combo!";

            if (combo <= 90)
                return $"{formattedCombo} Hit Unbelievable Combo!";

            if (combo <= 95)
                return $"{formattedCombo} Hit Mind-Shattering Combo!";
            
            if (combo <= 100)
                return $"{formattedCombo} Four Dimensional Combo that could break space-time!";
            
            if (combo <= 110)
                return $"{formattedCombo} Hit Ultra-Colossal Combo of gigantic proportions!";

            if (combo <= 120)
                return $"{formattedCombo} Hit Combo that is approaching an integer overflow!";
            
            if (combo <= 130)
                return $"{formattedCombo} Hit Combo that is simply absurd!";
            
            if (combo <= 140)
                return $"{formattedCombo} Hit Combo that is making me cry tears of pleasure!";
            
            if (combo <= 150)
                return $"{formattedCombo} Hit Combo that is beyond human comprehension!";
            
            if (combo <= 160)
                return $"{formattedCombo} Hit Combo that is making me question my whole life!";
            
            if (combo <= 170)
                return $"{formattedCombo} Hit Combo that expands beyond the scope of the universe!";
            
            if (combo <= 180)
                return $"{formattedCombo} Hit Combo that probably required some sort of glitch to achieve!";
            
            if (combo <= 190)
                return $"{formattedCombo} Hit Combo that could cure global warming!";

            if (combo <= 200)
                return $"{formattedCombo} Hit Combo that has crinkled the fabric of reality!";
            
            return $"{formattedCombo} Hit Small and Tiny Combo!";
        }
    }
}