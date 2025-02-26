using System;
using Source.GameData;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Source.UI
{
    public class DeleteSaveSlotConfirmationMenu : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI titleText;
        
        [SerializeField]
        private SaveDataManager _saveDataManager;
        
        [SerializeField]
        private UnityEvent onDeleteCompleted = new();
        
        private SaveSlotData _gameData;
        private void OnEnable()
        {
            var allData = _saveDataManager.GetAllSaveSlotData();
            
            _gameData = allData[GlobalSaveDataManager.GlobalData.SelectedSaveSlot];
            
            titleText.text = $"Delete game data '{_gameData.GameName}'?";
        }
        
        public void DeleteSelectedSlot()
        {
            var slot = GlobalSaveDataManager.GlobalData.SelectedSaveSlot;
            _saveDataManager.DeleteSaveSlot(slot, DeleteCallback);
        }

        private void DeleteCallback()
        {
            onDeleteCompleted?.Invoke();
        }
    }
}