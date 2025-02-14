using System;
using Source.Optimizations;
using Source.SaveData;
using TMPro;
using UnityEngine;

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
        
        
        private Menu _menu;

        private void Start()
        {
            _menu = GetComponent<Menu>();
        }

        public void RefreshInfoBox()
        {
            var selectedItem = _menu.GetHighlightedItem();

            // Note this wont work if we add new menu items besides save slots
            var saveSlotIndex = selectedItem.index;
            
            var saveSlotData = GameDataManager.Data.SaveSlots[saveSlotIndex];

            if (saveSlotData is null || saveSlotData.IsNew)
            {
                infoText.text = "No Data.\n\nSelect this save slot to begin a new game.";
                return;
            } 

            infoText.text = $"{saveSlotData.GameName}\n\n" +
                            $"{saveSlotData.LastUpdated:g}";

        }

        public void RenameMenuItems()
        {
            _menu = GetComponent<Menu>();

            var saveSlot0 = GameDataManager.Data.SaveSlots[0];
            var saveSlot1 = GameDataManager.Data.SaveSlots[1];
            var saveSlot2 = GameDataManager.Data.SaveSlots[2];
            var saveSlot3 = GameDataManager.Data.SaveSlots[3];
            var saveSlot4 = GameDataManager.Data.SaveSlots[4];

            if (saveSlot0 is not null && !saveSlot0.IsNew)
            {
                _menu.RenameMenuItem("slot1", $"1 | {saveSlot0.GameName}");
            }
            
            if (saveSlot1 is not null && !saveSlot1.IsNew)
            {
                _menu.RenameMenuItem("slot2", $"2 | {saveSlot1.GameName}");
            }
            
            if (saveSlot2 is not null && !saveSlot2.IsNew)
            {
                _menu.RenameMenuItem("slot3", $"3 | {saveSlot2.GameName}");
            }
            
            if (saveSlot3 is not null && !saveSlot3.IsNew)
            {
                _menu.RenameMenuItem("slot4", $"4 | {saveSlot3.GameName}");
            }
            
            if (saveSlot4 is not null && !saveSlot4.IsNew)
            {
                _menu.RenameMenuItem("slot5", $"5 | {saveSlot4.GameName}");
            }
            
        }

        public void MenuEnter()
        {
            var selectedItem = _menu.GetHighlightedItem();

            // Note this wont work if we add new menu items besides save slots
            var saveSlotIndex = selectedItem.index;
            
            GameDataManager.Data.SaveSlotIndex = saveSlotIndex;
            
            var saveSlotData = GameDataManager.Data.SaveSlots[saveSlotIndex];

            if (saveSlotData is null || saveSlotData.IsNew)
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

    }
}