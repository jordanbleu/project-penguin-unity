using System;
using Source.Data;
using TMPro;
using UnityEngine;

namespace Source.UI
{
    public class ComboDisplay : MonoBehaviour
    {
        private Animator _animator;
        private static readonly int IsComboActive = Animator.StringToHash("is-displayed");

        [SerializeField]
        private TextMeshProUGUI _comboText;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            var combo = Stats.Current?.BulletsFiredCombo;
            
            if (combo is null)
                return;
            
            if (combo > 5)
            {
                _animator.SetBool(IsComboActive, true);
            }
            else
            {
                _animator.SetBool(IsComboActive, false);
            }

            _comboText.text = GetDisplayString(combo.Value);
        }

        private static string GetDisplayString(int combo)
        {
            var formattedCombo = combo.ToString("n0");

            if (combo <= 10)
                return $"{formattedCombo} Hit Combo!";

            if (combo <= 20)
                return $"{formattedCombo} Hit Combo!!!!";

            if (combo <= 25)
                return $"{formattedCombo} Hit Mega-Combo!";

            if (combo <= 30)
                return $"{formattedCombo} Hit THICK Combo!";

            if (combo <= 35)
                return $"{formattedCombo} Hit Extra Juicy Combo!";

            if (combo <= 40)
                return $"{formattedCombo} Hit Crispy Combo!";

            if (combo <= 45)
                return $"{formattedCombo} Hit Mildly Spicy Combo!";

            if (combo <= 50)
                return $"{formattedCombo} Hit Incredibly Gorgeous Combo!";

            if (combo <= 55)
                return $"{formattedCombo} Hit Somewhat Suspicious Combo!";

            if (combo <= 60)
                return $"{formattedCombo} Hit Alarmingly Massive Combo!";

            if (combo <= 65)
                return $"{formattedCombo} Hit Creamy Combo!";

            if (combo <= 70)
                return $"{formattedCombo} Hit Voluptuous Combo!";

            if (combo <= 75)
                return $"{formattedCombo} Hit Tear-Jerking Combo!";

            if (combo <= 80)
                return $"{formattedCombo} Hit Emotionally Stimulating Combo!";

            if (combo <= 85)
                return $"{formattedCombo} Hit Nightmare-Inducing Combo!";

            if (combo <= 90)
                return $"{formattedCombo} Hit Physically Stunning Combo!";

            if (combo <= 95)
                return $"{formattedCombo} Hit Mind-Shattering Combo!";

            return $"{formattedCombo} Hit Insanely Sexy Combo!";
        }
    }
}