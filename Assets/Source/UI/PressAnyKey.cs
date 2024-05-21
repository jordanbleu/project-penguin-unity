using System;
using Source.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Source.UI
{
    public class PressAnyKey : MonoBehaviour
    {
        [SerializeField]
        private GameObject _planet;
        
        [SerializeField]
        private GameObject _logo;

        [SerializeField]
        private GameObject _pressAnyKeyToBegin;

        [SerializeField]
        private GameObject _whiteFlash;
        
        private Vector2 planetEndPosition; 
        private Vector2 logoEndPosition;
        private Vector2 pressAnyKeyToBeginEndPosition;

        private bool _isReady = false;

        private void Update()
        {
            if (!_isReady)
                return;
            
            // if any key was pressed this frame on the keyboard or gamepad
            if (InputUtils.AnyKeyHit())
            {
                LeanTween.moveLocalY(_logo, -200, 1f).setEaseInBack();
                LeanTween.moveLocalY(_pressAnyKeyToBegin, -200, 1f).setEaseInBack().setDelay(0.5f);
                LeanTween.moveLocalY(_planet, -48, 3).setEaseInOutCubic().setDelay(0.5f);
                _whiteFlash.SetActive(true);
            }
        }

        private void Start()
        {
            // store the end positions of the objects as their end state
            // this is so i can position elements in the scene view easier lol
            planetEndPosition = _planet.transform.localPosition;
            logoEndPosition = _logo.transform.localPosition;
            pressAnyKeyToBeginEndPosition = _pressAnyKeyToBegin.transform.localPosition;
            
            // immediately move the items off screen
            _planet.transform.localPosition = new Vector2(planetEndPosition.x, -180);
            _logo.transform.localPosition = new Vector2(logoEndPosition.x, 200);
            _pressAnyKeyToBegin.transform.localPosition = new Vector2(pressAnyKeyToBeginEndPosition.x, -200);
            
            // start the animation
            LeanTween.moveLocal(_planet, planetEndPosition, 5f).setEaseInOutCubic();
            
            LeanTween.moveLocal(_logo, logoEndPosition, 1f).setEaseOutQuint().setDelay(2f);
            
            LeanTween.moveLocal(_pressAnyKeyToBegin, pressAnyKeyToBeginEndPosition, 1f).setEaseOutExpo().setDelay(3f).setOnComplete(Ready);
        }
        

        private void Ready()
        {
            _isReady = true;
        }
    }
}