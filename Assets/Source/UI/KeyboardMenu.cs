using System;
using System.Collections.Generic;
using Source.GameData;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Source.UI
{
    public class KeyboardMenu : MonoBehaviour
    {

        [SerializeField]
        private string defaultText = "";

        [SerializeField]
        private TextMeshProUGUI inputText;

        [SerializeField]
        private int minLength = 1;

        [SerializeField]
        private int maxLength = 20;

        [SerializeField]
        private Alert alertBox;
        
        [SerializeField]
        private UnityEvent onEnterEntered = new();
        
        [SerializeField]
        private SaveDataManager saveDataManager;
        
        [SerializeField]
        [Tooltip("Use director to wait until this is called so you don't swap scenes before the data is saved.")]
        private UnityEvent onSaveDataFinished;
        
        private bool _caps = true;
        
        private Menu _menu;
        
        private string _currentText = "";

        private bool _wasInitialized = false;
        private void Start()
        {
            _currentText = defaultText;
            RefreshText();
            _menu = GetComponent<Menu>();
            InitializeKeyboard();
        }

        private void Alert(string text)
        {
            alertBox.Show(text, 3f);
        }

        private void RefreshText()
        {
            inputText.SetText(">" + _currentText + "<");
        }

        private string GetIdForChar(string character)
        {
            return "letter" + character.ToUpper();
        }

        public void InitializeKeyboard()
        {
            if (_wasInitialized)
                return;
            
            _wasInitialized = true;

            var menuItems = new List<Menu.MenuItemData>();
            
            // Add backspace character
            var backspaceItem = new Menu.MenuItemData
            {
                Id = "backspace",
                Text = "<--",
                IsEnabled = true,
                OnItemSelected = new()
            };
            backspaceItem.OnItemSelected.AddListener(OnBackspaceEntered);
            backspaceItem.OnItemAltSelected.AddListener(OnBackspaceEntered);
            menuItems.Add(backspaceItem);

            // Add enter character
            var enterItem = new Menu.MenuItemData
            {
                Id = "enter",
                Text = "OK",
                IsEnabled = true,
                OnItemSelected = new()
            };
            enterItem.OnItemSelected.AddListener(OnEnterEntered);
            enterItem.OnItemAltSelected.AddListener(OnBackspaceEntered);
            menuItems.Add(enterItem);
            
            
            // Add shift character
            var shiftItem = new Menu.MenuItemData
            {
                Id = "shift",
                Text = "CAPS",
                IsEnabled = true,
                OnItemSelected = new()
            };
            shiftItem.OnItemAltSelected.AddListener(OnBackspaceEntered);
            shiftItem.OnItemSelected.AddListener(OnShiftEntered);
            menuItems.Add(shiftItem);
            
            // Add space character
            var spaceItem = new Menu.MenuItemData
            {
                Id = "space",
                Text = "SPC",
                IsEnabled = true,
                OnItemSelected = new()
            };
            spaceItem.OnItemAltSelected.AddListener(OnBackspaceEntered);
            spaceItem.OnItemSelected.AddListener(OnSpaceEntered);
            menuItems.Add(spaceItem);
            

            // Add the alphabet (capitals by default)
            for (int i = 0; i < 26; i++)
            {
                var character = (char)('A' + i);

                var mItem = new Menu.MenuItemData
                {
                    Id = GetIdForChar(character.ToString()),
                    Text = character.ToString(),
                    IsEnabled = true,
                    OnItemSelected = new()
                };
                
                mItem.OnItemSelected.AddListener(OnCharacterEntered);
                mItem.OnItemAltSelected.AddListener(OnBackspaceEntered);

                menuItems.Add(mItem);
            }

            _menu.CreateMenu(menuItems.ToArray());
        }

        private void TypeLetter(string letter)
        {
            if (_currentText.Length >= maxLength)
            {
                Alert($"Must be less than {maxLength} characters.");
                return;
            }

            _currentText += letter;
            RefreshText();
            
            // If the player is typing the first letter and shift is enabled,
            // disable caps so the first letter is capitalized easier 
            if (_currentText.Length == 1 && _caps)
                OnShiftEntered();
        }
        
        private void OnEnterEntered()
        {
            if (_currentText.Length < minLength)
            {
                var pluralCharacters = minLength == 1 ? "character" : "characters";
                Alert($"Must be at least {minLength} {pluralCharacters}.");
                return;
            }
            
            var gameData = new SaveSlotData()
            {
                GameName = _currentText,
                IsEmpty = false,
                LastUpdateDt = DateTimeOffset.UtcNow,
                CreateDt = DateTimeOffset.UtcNow
            };
            
            // begin saving the new data, then invoke the on saving complete
            saveDataManager.BeginSaving(GlobalSaveDataManager.GlobalData.SelectedSaveSlot, gameData, ()=>onSaveDataFinished.Invoke());
            
            _menu.DismissMenu();
            onEnterEntered?.Invoke();
        }

        private void OnBackspaceEntered()
        {
            if (_currentText.Length == 0)
                return;
            
            _currentText = _currentText.Substring(0, _currentText.Length - 1);
            RefreshText();
        }

        private void OnSpaceEntered()
        {
            TypeLetter(" ");
        }

        private void OnShiftEntered()
        {
            // Rename every letter key to the lower of itself
            // I'm trying to win the contest for worlds worst code 
            
            for (int i = 0; i < 26; i++)
            {
                var character = (char)('A' + i);

                _menu.RenameMenuItem(GetIdForChar(character.ToString()),
                    _caps ? character.ToString().ToLower() : character.ToString().ToUpper());

            }
            _caps = !_caps;

        }

        private void OnCharacterEntered()
        {
            var selectedItem = _menu.GetHighlightedItem();
            TypeLetter(selectedItem.data.Text);
        }
    }
}