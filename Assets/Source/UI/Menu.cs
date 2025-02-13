using System;
using System.Linq;
using Source.Audio;
using Source.Constants;
using Source.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Source.UI
{
    /// <summary>
    /// Note to future me - the input events need to be wrired up in the 'InputHandler' game object otherwise this weirdly wont work lol
    /// </summary>
    public class Menu : MonoBehaviour
    {

        // positioning
        private const float OffScreenPosition = -250f;
        private const float OnScreenPosition = 0f;
        
        // positioning variables
        private const float MenuItemTopPosition = 50;
        private const float MenuItemSpacing = 25;
        
        // Animation times
        private const float MenuAnimationTime = 1f;

        [SerializeField]
        [Tooltip("How many items to display before user has to scroll")]
        private int pageSize = 5;
        
        [SerializeField]
        private GameObject menuItemPrefab;
        
        [SerializeField]
        private MenuItemData[] menuItemData;

        private TextMeshProUGUI[] _textMeshes;

        [SerializeField]
        private GameObject selector;
        
        [SerializeField]
        private UnityEvent onGoBack = new();

        [SerializeField]
        [Tooltip("Called whenever the user changes the selected item")]
        private UnityEvent onChange = new();
        
        [SerializeField]
        [Tooltip("Called when the menu is ready, after the initial animation")]
        private UnityEvent onReady = new();
        
        [SerializeField]
        [Tooltip("Called before the menu opens.  The menu items will be created but not yet visible.")]
        private UnityEvent afterMenuStart = new();

        [SerializeField]
        [Tooltip("Fired when any menu item is selected.")]
        private UnityEvent onAnyMenuItemSelected = new();
        
        [SerializeField]
        private AudioClip menuOkSound;
        
        [SerializeField]
        private AudioClip menuChangeSound;
        
        [SerializeField]
        private AudioClip menuCancelSound;
        
        // this is the selectors position on screen, so would be anywhere from 0 to pageSize
        private int _selectorPosition = 0;
        
        // this is the items that the menu is currently offset by.
        private int _currentPageOffset = 0;
        private bool _ready;
        private TextMeshProUGUI[] _menuItems;

        private SoundEffectEmitter _soundEmitter;
        
        private int GetPageSize() => Math.Min(pageSize, menuItemData.Length);
        
        private void Start()
        {
            var soundEmitterObj = GameObject.FindWithTag(Tags.SoundEffectEmitter);
            
            if (!soundEmitterObj)
                throw new UnityException("Missing object tagged as sound effect emitter");
            
            _soundEmitter = soundEmitterObj.GetComponent<SoundEffectEmitter>();
            
            if (menuItemData.Any())
                CreateMenu(menuItemData);
            
            // be careful, the menu component isn't available if you use this 
            afterMenuStart?.Invoke();
        }
        
        public void CreateMenu(MenuItemData[] items)
        {
            if (_textMeshes is { Length: > 0 })
            {
                foreach (var existingTMesh in _textMeshes)
                {
                    // NOTE - this will fail if the menu item prefabs are altered
                    Destroy(existingTMesh.transform.parent.gameObject); 
                }
            }

            menuItemData = items;
            var pageSize = GetPageSize();
            _textMeshes = new TextMeshProUGUI[pageSize];
            
            for (var i=0;i<pageSize;i++)
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
            var positionX = transform.localPosition.x;

            transform.localPosition = new Vector3(positionX, OffScreenPosition, 0);

            // animate menu in
            LeanTween.moveLocalY(gameObject, OnScreenPosition, MenuAnimationTime)
                .setEase(LeanTweenType.easeInOutBack)
                .setOnComplete(Ready);
        }
        
        private void Ready()
        {
            _ready = true;
            onReady?.Invoke();
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

            var pageSize = GetPageSize();

            for (var i=0;i<pageSize;i++)
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
            var pageSize = GetPageSize();

            for (var i = 0; i < pageSize; i++)
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
            
            _soundEmitter.Play(menuChangeSound);
            
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
            onChange?.Invoke();
            menuItemData[_currentPageOffset + _selectorPosition].onItemHighlighted?.Invoke();
            RefreshMenuItems();
        }

        /// <summary>
        /// Get the menu item data object as well as its index in the overall array
        /// </summary>
        /// <returns></returns>
        public (int index, MenuItemData data) GetHighlightedItem()
        {
            var index = _currentPageOffset + _selectorPosition;
            return (index, menuItemData[index]);    
        }

        public void RenameMenuItem(string id, string menuText)
        {
            var item = Array.Find(menuItemData, x => x.Id == id);
            
            if (item != null)
            {
                item.Text = menuText;
                RefreshPage();
            }
        
        }


        public void MoveCursorDown(InputAction.CallbackContext context)
        {
            var pageSize = GetPageSize();
            
            if (!_ready)
                return;


            // if the button is pressed.
            if (!context.started)
                return;
            
            _soundEmitter.Play(menuChangeSound);

            if (_selectorPosition < pageSize-1)
            {
                _selectorPosition++;
            }
            else
            {
                if (_currentPageOffset + pageSize < menuItemData.Length)
                {
                    _currentPageOffset++;
                    RefreshPage();
                }
            }
            onChange?.Invoke();
            menuItemData[_currentPageOffset + _selectorPosition].onItemHighlighted?.Invoke();

            RefreshMenuItems();
        }
        
        public void MenuEnter(InputAction.CallbackContext context)
        {
            if (!_ready)
                return;
            
            // if the button is pressed.
            if (!context.started)
                return;
            
            _soundEmitter.Play(menuOkSound);

            // selector does the squeeze animation 
            var oldScaleY = selector.transform.localScale.y;
            LeanTween.scaleY(selector, 0.8f, 0.1f).setOnComplete(()=>LeanTween.scaleY(selector, oldScaleY, 0.1f));
            
            menuItemData[_currentPageOffset + _selectorPosition].OnItemSelected?.Invoke();
            onAnyMenuItemSelected?.Invoke();    
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
            
            _soundEmitter.Play(menuCancelSound);
            
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
            public UnityEvent onItemHighlighted;

            [SerializeField]
            public bool IsEnabled = true;
        }
    }

}