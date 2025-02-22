using System;
using System.Collections.Generic;
using Source.Debugging;
using Source.GameData;
using Source.Optimizations;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Source.UI
{
    public class SaveSlotMenu : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI infoText;

        [SerializeField]
        private GameObject keyboardMenu;

        [SerializeField]
        private SceneLoader sceneLoader;
        
        [SerializeField]
        private SaveDataManager saveDataManager;
        
        [SerializeField]
        private GlobalSaveDataManager globalSaveDataManager;
        
        [SerializeField]
        private DeleteSaveSlotConfirmationMenu deleteConfirmationMenu;
        
        private Menu _menu;
        
        private Dictionary<int, SaveSlotData> _saveSlotData;
        
        private void OnEnable()
        {
            _saveSlotData = saveDataManager.GetAllSaveSlotData();
            RenameMenuItems();
        }

        private void Start()
        {
            _menu = GetComponent<Menu>();
        }

        public void RefreshInfoBox()
        {
            var selectedItem = _menu.GetHighlightedItem();

            // Note this won't work if we add new menu items besides save slots
            var saveSlotIndex = selectedItem.index;
            
            var saveSlotData = _saveSlotData[saveSlotIndex];
            
            
            if (saveSlotData is null || saveSlotData.IsEmpty)
            {
                infoText.text = "No Data.\n\nSelect this save slot to begin a new game.";
                return;
            }
            
            // if IsEmpty is false but IsCorrupted is true, we have a problem
            if (saveSlotData.IsCorrupted)
            {
                infoText.text = "This save slot is corrupt, and cannot be loaded. The data must be deleted to use this slot.";
                return;
            }
            
            int highScore = 0;
            
            if (saveSlotData?.SceneData?.TryGetValue(GameScene.Beginning, out var sceneData) == true)
            {
                highScore = sceneData.HighestScore;    
            }

            infoText.text = $"{saveSlotData.GameName}\n\n" +
                            "Last Updated:\n" +
                            $"{saveSlotData.LastUpdateDt.ToLocalTime():g}\n\n" +  
                            "High Score:\n" + 
                            highScore;
        }

        public void RenameMenuItems()
        {
            _menu = GetComponent<Menu>();

            var saveSlot0 = _saveSlotData[0];
            var saveSlot1 = _saveSlotData[1];
            var saveSlot2 = _saveSlotData[2];
            var saveSlot3 = _saveSlotData[3];
            var saveSlot4 = _saveSlotData[4];

            RenameMenuItem(0, "slot1", saveSlot0);
            RenameMenuItem(1, "slot2", saveSlot1);
            RenameMenuItem(2, "slot3", saveSlot2);
            RenameMenuItem(3, "slot4", saveSlot3);
            RenameMenuItem(4, "slot5", saveSlot4);
        }
        
        private void RenameMenuItem(int slot, string menuItemId, SaveSlotData data)
        {
            if (data is null || data.IsEmpty)
            {
                _menu.RenameMenuItem(menuItemId, $"{slot+1} | [New Game]");
                return;
            }
            
            if (data.IsCorrupted)
            {
                _menu.RenameMenuItem(menuItemId, $"{slot+1} | [?????]");
                return;
            }
            
            _menu.RenameMenuItem(menuItemId, $"{slot+1} | {data.GameName}");
        }

        public void MenuEnter()
        {
            var selectedItem = _menu.GetHighlightedItem();

            // Note this won't work if we add new menu items besides save slots
            var saveSlotIndex = selectedItem.index;
            var saveSlotData = _saveSlotData[saveSlotIndex];
            
            if (saveSlotData.IsCorrupted && !saveSlotData.IsEmpty)
            {
                // trying to load a corrupted file
                _menu.PlayErrorSound();
                return;
            }
            
            // public static classes are where it's at.
            GlobalSaveDataManager.GlobalData.SelectedSaveSlot = saveSlotIndex;
            
            if (saveSlotData.IsEmpty)
            {
                // Start a new game
                keyboardMenu.SetActive(true);
                _menu.DismissMenu();
                return;
            }

            var sceneNameToLoad = saveSlotData.CurrentScene.GetUnitySceneName();
            sceneLoader.BeginFadingToScene(sceneNameToLoad);
            _menu.DismissMenu();
            
        }
        
        public void DeleteSlot(int slot)
        {
            var selectedItem = _menu.GetHighlightedItem();
            var saveSlotIndex = selectedItem.index;
            
            if (_saveSlotData[saveSlotIndex].IsEmpty)
            {
                // nothing to delete
                _menu.PlayErrorSound();
                return;
            }
            
            GlobalSaveDataManager.GlobalData.SelectedSaveSlot = saveSlotIndex;
            deleteConfirmationMenu.gameObject.SetActive(true);
            _menu.DismissMenu();
        }

    }
}