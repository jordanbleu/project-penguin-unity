using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Source.UI
{
    /// <summary>
    /// UI Item that displays itself with an animation and then disappears.
    /// </summary>
    public class Toast : MonoBehaviour
    {
        [SerializeField]
        private float preDelaySeconds = 0f;
        
        [SerializeField]
        private float animateInSeconds = 1f;

        [SerializeField]
        private float displaySeconds = 2f;

        [SerializeField]
        private float animateOutSeconds = 1f;

        [SerializeField]
        private ToastStyle style = ToastStyle.Scale;
        
        [SerializeField]
        private UnityEvent onToastComplete = new UnityEvent();
        
        private float _duration = 0f;

        // this just means that it is not animating in or out
        private bool _isDisplayed = false;
        private float _yPosition;
        private float _yOffscreenPosition;
        private void OnEnable()
        {
            
            _yPosition = transform.localPosition.y;
            
            _yOffscreenPosition = _yPosition - 20;

            SetInitialPosition();

            BeginAnimateIn();
        }

        private void SetInitialPosition()
        {
            var localPos = transform.localPosition;
            
            if (style == ToastStyle.Scroll)
            {
                transform.localPosition = new Vector3(localPos.x, -150, localPos.z);
                return;
            }

            transform.localScale = Vector2.zero;
            transform.localPosition = new Vector3(localPos.x, _yOffscreenPosition, localPos.z);
        }

        private void BeginAnimateIn()
        {
            if (style == ToastStyle.Scroll)
            {
                LeanTween.moveLocalY(gameObject, _yPosition, animateInSeconds).setEaseOutCubic()
                    .setDelay(preDelaySeconds)
                    .setOnComplete(OnAnimateInComplete);
                return;
            }
            
            
            LeanTween.moveLocalY(gameObject, _yPosition, animateInSeconds).setEaseOutCubic()
                .setDelay(preDelaySeconds)
                .setOnComplete(OnAnimateInComplete);

            LeanTween.scaleY(gameObject, 1, animateInSeconds)
                .setDelay(preDelaySeconds)
                .setEaseOutCubic();

            LeanTween.scaleX(gameObject, 1, animateInSeconds / 2)
                .setDelay(preDelaySeconds)
                .setEaseOutCubic();
        }

        private void Update()
        {
            if (!_isDisplayed || _duration >= displaySeconds)
                return;
            
            _duration += Time.deltaTime;
            
            if (_duration >= displaySeconds)
            {
                BeginAnimateOut();
            }
        }

        private void BeginAnimateOut()
        {
            if (style == ToastStyle.Scroll)
            {
                LeanTween.moveLocalY(gameObject, 150, animateInSeconds).setEaseOutCubic()
                    .setOnComplete(OnAnimateOutComplete);
                return;
            }
            
            LeanTween.moveLocalY(gameObject, _yOffscreenPosition, animateOutSeconds).setEaseOutCubic()
                .setOnComplete(OnAnimateOutComplete);
            LeanTween.scaleY(gameObject, 0, animateInSeconds).setEaseOutCubic();
            LeanTween.scaleX(gameObject, 0, animateInSeconds / 2).setEaseOutCubic();
        }

        private void OnAnimateOutComplete()
        {
            onToastComplete.Invoke();
            Destroy(gameObject);
        }

        private void OnAnimateInComplete()
        {
            _isDisplayed = true;
        }

        public enum ToastStyle
        {
            Scale,
            Scroll
        }
    }
}