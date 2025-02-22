using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Source.UI
{
    [Tooltip("Finds the active control scheme and swaps certain strings with the appropriate glyph.")]
    public class TextMeshButtonGlyphParser : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI textMesh;

        private PlayerInput _playerInput;
        private string _originalText;
        private string _previousControlScheme;
        private void Start()
        {
            _originalText = new string(textMesh.text);
            // There can only be one of these because otherwise things  would get WILD
            _playerInput = FindObjectOfType<PlayerInput>();
        }

        private void Update()
        {
            var currentControlScheme = _playerInput.currentControlScheme;
            
            // nothing changed
            if (_previousControlScheme?.Equals(currentControlScheme) == true) 
                return;
            
            // swap the text
            if (currentControlScheme is "Keyboard" or null)
            {

                ReplaceWithKeyboardGlyphs();
            }
            else
            {
                ReplaceWithXboxGlyphs();
            }
            
            _previousControlScheme = currentControlScheme;
            
        }

        private void ReplaceWithKeyboardGlyphs()
        {
            var text = _originalText
                .Replace("{menu-up}", " <sprite name=\"glyph-chars_0\"/>")
                .Replace("{menu-down}", " <sprite name=\"glyph-chars_1\"/>")
                .Replace("{menu-enter}", " <sprite name=\"glyph-chars_2\"/>")
                .Replace("{menu-back}", " <sprite name=\"glyph-chars_3\"/>")
                .Replace("{reload}", "[R]")
                .Replace("{anyKey}", "any key")
                .Replace("{shoot}", " <sprite name=\"glyph-chars_5\"/>")
                .Replace("{move}", "W,A,S,D ")
                .Replace("{dash}", "[Shift]");
            
            textMesh.text = text;
        }

        private void ReplaceWithXboxGlyphs()
        {
            var text = _originalText
                .Replace("{menu-up}", " <sprite name=\"glyph-chars_12\"/>")
                .Replace("{menu-down}", " <sprite name=\"glyph-chars_13\"/>")
                .Replace("{menu-enter}", " <sprite name=\"glyph-chars_9\"/>")
                .Replace("{menu-back}", " <sprite name=\"glyph-chars_11\"/>")
                .Replace("{reload}", " <sprite name=\"glyph-chars_10\"/>")
                .Replace("{anyKey}", "any button")
                .Replace("{shoot}", " <sprite name=\"glyph-chars_9\"/>")
                .Replace("{move}", " <sprite name=\"glyph-chars_16\"/>")
                .Replace("{dash}", " <sprite name=\"glyph-chars_11\"/>");

            
            textMesh.text = text;
        }
    }
}