using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Serialization;

namespace Source.UI
{
    public class Menu : MonoBehaviour
    {
        // how many items to display per page
        private const int PageSize = 5;
        
        // positioning variables
        private const float MenuItemTopPosition = 50;
        private const float MenuItemSpacing = 25;
        
        [SerializeField]
        private GameObject menuItemPrefab;
        
        [SerializeField]
        private MenuItemData[] menuItemData;

        private TextMeshProUGUI[] _textMeshes;

        [SerializeField]
        private GameObject selector;
        
        [SerializeField]
        private TMP_FontAsset fontAsset;
        
        // this is the selectors position on screen, so would be anywhere from 0 to pageSize
        private int _selectorPosition = 0;
        
        // this is the items that the menu is currently offset by.
        private int _currentPageOffset = 0;
        
        private TextMeshProUGUI[] _menuItems;
        
        private void Start()
        {
            _textMeshes = new TextMeshProUGUI[PageSize];
            
            for (var i=0;i<PageSize;i++)
            {
                var menuItem = Instantiate(menuItemPrefab, transform);
                menuItem.transform.localPosition = new Vector3(0, MenuItemTopPosition - (i * MenuItemSpacing), 0);
                _textMeshes[i] = menuItem.GetComponentInChildren<TextMeshProUGUI>();
            }

            RefreshPage();
            RefreshMenuItems();
        }

        // applies proper styles to the selected item and unstyles the rest
        private void RefreshMenuItems()
        {
            selector.transform.localPosition = new(0, MenuItemTopPosition - _selectorPosition*MenuItemSpacing,0);
            
            for (var i=0;i<PageSize;i++)
            {
                if (i == _selectorPosition)
                {
                    // if i am the selected item, bold me 
                    _textMeshes[i].fontStyle = FontStyles.Bold;
                }
                else
                {
                    _textMeshes[i].fontStyle = FontStyles.Normal;
                }
            }   
        }

        // refreshes what each item is displaying
        private void RefreshPage()
        {
            for (var i = 0; i < PageSize; i++)
            {
                var startIndex = _currentPageOffset + i;
                
                if (startIndex < menuItemData.Length)
                {
                    _textMeshes[i].text = menuItemData[startIndex].Text;
                }
                else
                {
                    _textMeshes[i].text = "";
                }
            }
        }
        
        public void MoveCursorUp(InputAction.CallbackContext context)
        {
            // if the button is pressed.
            if (!context.started)
                return;
            
            if (_selectorPosition > 0)
            {
                _selectorPosition--;
            }
            else
            {
                if (_currentPageOffset > 0)
                {
                    _currentPageOffset--;
                    RefreshPage();
                }
            }
            RefreshMenuItems();
        }
        
        public void MoveCursorDown(InputAction.CallbackContext context)
        {
            // if the button is pressed.
            if (!context.started)
                return;

            if (_selectorPosition < PageSize-1)
            {
                _selectorPosition++;
            }
            else
            {
                if (_currentPageOffset + PageSize < menuItemData.Length)
                {
                    _currentPageOffset++;
                    RefreshPage();
                }
            }
            RefreshMenuItems();
        }

        public void MenuEnter(InputAction.CallbackContext context)
        {
            // if the button is pressed.
            if (!context.started)
                return;
            
            menuItemData[_currentPageOffset + _selectorPosition].OnItemSelected.Invoke();
            
        }

        public void MenuBack()
        {
        }


        [Serializable]
        public class MenuItemData
        {
            [SerializeField]
            public string Id;

            [SerializeField]
            public string Text;

            [SerializeField]
            public UnityEvent OnItemSelected;

            [SerializeField]
            public bool IsEnabled = true;
        }
    }

}