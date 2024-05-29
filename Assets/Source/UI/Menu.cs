using System;
using Source.Extensions;
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
        
        // positioning
        private const float OffScreenPosition = -200f;
        private const float OnScreenPosition = 0f;
        
        // positioning variables
        private const float MenuItemTopPosition = 50;
        private const float MenuItemSpacing = 25;
        
        // Animation times
        private const float MenuAnimationTime = 1f;
        
        
        [SerializeField]
        private GameObject menuItemPrefab;
        
        [SerializeField]
        private MenuItemData[] menuItemData;

        private TextMeshProUGUI[] _textMeshes;

        [SerializeField]
        private GameObject selector;
        
        [SerializeField]
        private TMP_FontAsset fontAsset;

        [SerializeField]
        private UnityEvent onGoBack = new();
        
        // this is the selectors position on screen, so would be anywhere from 0 to pageSize
        private int _selectorPosition = 0;
        
        // this is the items that the menu is currently offset by.
        private int _currentPageOffset = 0;
        private bool _ready = false;
        private TextMeshProUGUI[] _menuItems;
        
        private void Start()
        {
            _textMeshes = new TextMeshProUGUI[PageSize];
            
            for (var i=0;i<PageSize;i++)
            {
                var menuItem = Instantiate(menuItemPrefab, transform).WithName("MenuItem" + i); 
                menuItem.transform.localPosition = new Vector3(0, MenuItemTopPosition - (i * MenuItemSpacing), 0);
                _textMeshes[i] = menuItem.GetComponentInChildren<TextMeshProUGUI>();
            }

            RefreshPage();
            RefreshMenuItems();
        }

        private void OnEnable()
        {
            transform.localPosition = new Vector3(0, OffScreenPosition, 0);

            // animate menu in
            LeanTween.moveLocalY(gameObject, OnScreenPosition, MenuAnimationTime)
                .setEase(LeanTweenType.easeInOutBack)
                .setOnComplete(Ready);
        }
        
        private void Ready()
        {
            _ready = true;
        }

        // applies proper styles to the selected item and unstyles the rest
        private void RefreshMenuItems()
        {
            if (LeanTween.isTweening(selector))
            {
                LeanTween.cancel(selector);
            }

            var oldY = selector.transform.localPosition.y;
            var newY = MenuItemTopPosition - _selectorPosition * MenuItemSpacing;

            if ((int)newY != (int)oldY)
            {
                LeanTween.moveLocalY(selector, newY, 0.5f).setEase(LeanTweenType.easeOutElastic);
            }
            else
            {
                // do a little bounce
                
                var bounceDirection = _selectorPosition == 0 ? 1 : -1;
                
                LeanTween.moveLocalY(selector, oldY + 3 * bounceDirection, 0.25f).setEase(LeanTweenType.easeOutElastic)
                    .setOnComplete(() =>
                        LeanTween.moveLocalY(selector, oldY, 0.25f));

            }


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
            if (!_ready)
                return;
            
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
            if (!_ready)
                return;
            
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
            if (!_ready)
                return;
            
            // if the button is pressed.
            if (!context.started)
                return;

            // selector does the squeeze animation 
            var oldScaleY = selector.transform.localScale.y;
            LeanTween.scaleY(selector, 0.8f, 0.1f).setOnComplete(()=>LeanTween.scaleY(selector, oldScaleY, 0.1f));
            
            
            menuItemData[_currentPageOffset + _selectorPosition].OnItemSelected.Invoke();
        }

        public void DismissMenu()
        {
            _ready = false;
            
            LeanTween.moveLocalY(gameObject, OffScreenPosition, MenuAnimationTime)
                .setEase(LeanTweenType.easeInOutBack)
                .setOnComplete(() => gameObject.SetActive(false));
        }

        public void MenuBack()
        {
            if (!_ready)
                return;
            
            onGoBack.Invoke();
            DismissMenu();
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