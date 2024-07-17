using System;
using Source.Utilities;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Source.UI
{
    public class PressAnyKey : MonoBehaviour
    {
        [SerializeField]
        private GameObject planet;
        
        [SerializeField]
        private GameObject logo;

        [SerializeField]
        private GameObject pressAnyKeyToBegin;

        [SerializeField]
        private GameObject whiteFlashPrefab;

        [SerializeField]
        private GameObject menu;

        [SerializeField]
        private GameObject whiteFlashParent;
        
        private Vector2 planetEndPosition; 
        private Vector2 logoEndPosition;
        private Vector2 pressAnyKeyToBeginEndPosition;

        private bool _isReady = false;
        private bool _wasTriggered = false;

        private void Update()
        {
            if (!_isReady)
                return;
            
            // if any key was pressed this frame on the keyboard or gamepad
            if (!_wasTriggered && InputUtils.AnyKeyHit())
            {
                LeanTween.moveLocalY(logo, -200, 1f).setEaseInBack();
                LeanTween.moveLocalY(pressAnyKeyToBegin, -200, 1f).setEaseInBack().setDelay(0.5f).setOnComplete(()=>menu.SetActive(true));
                LeanTween.moveLocalY(planet, -48, 3).setEaseInOutCubic().setDelay(0.5f);
                Instantiate(whiteFlashPrefab, whiteFlashParent.transform);
                _wasTriggered = true;
                
            }
        }

        private void Start()
        {
            // store the end positions of the objects as their end state
            // this is so i can position elements in the scene view easier lol
            planetEndPosition = planet.transform.localPosition;
            logoEndPosition = logo.transform.localPosition;
            pressAnyKeyToBeginEndPosition = pressAnyKeyToBegin.transform.localPosition;
            
            // immediately move the items off screen
            planet.transform.localPosition = new Vector2(planetEndPosition.x, -180);
            logo.transform.localPosition = new Vector2(logoEndPosition.x, 200);
            pressAnyKeyToBegin.transform.localPosition = new Vector2(pressAnyKeyToBeginEndPosition.x, -200);

            
            AnimateIn();
        }

        public void AnimateIn()
        {
            _isReady = false;

            _wasTriggered = false;

            // start the animation
            LeanTween.moveLocal(planet, planetEndPosition, 5f).setEaseInOutCubic();
            
            LeanTween.moveLocal(logo, logoEndPosition, 1f).setEaseOutQuint().setDelay(2f);
            
            LeanTween.moveLocal(pressAnyKeyToBegin, pressAnyKeyToBeginEndPosition, 1f).setEaseOutExpo().setDelay(3f).setOnComplete(Ready);
        }

        private void Ready()
        {
            _isReady = true;
        }
    }
}