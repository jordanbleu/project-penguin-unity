using System;
using Source.Constants;
using Source.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Source.UI
{
    /// <summary>
    /// UI Item that displays itself with an animation and then disappears.
    /// </summary>
    public class Toast : MonoBehaviour
    {
        private const float TopOffscreenPosition = 300;
        private const float BottomOffscreenPosition = -200;

        [SerializeField]
        private GameObject upArrow;
        
        [SerializeField]
        private GameObject downArrow;
        
        
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
        private LeanTweenType easeInStyle = LeanTweenType.linear;
        
        [SerializeField]
        private LeanTweenType easeOutStyle = LeanTweenType.linear;

        [SerializeField]
        private UnityEvent onToastComplete = new UnityEvent();
        
        [SerializeField]
        [Tooltip("Setting to false will keep the toast alive once it completes.  Make sure to add a listener to onToastComplete if you do this, or weird things will happen!")]
        private bool destroyOnComplete = true;
        
        private float _duration = 0f;

        // this just means that it is not animating in or out
        private bool _isDisplayed = false;
        private float _yOffscreenPosition;

        private Camera _mainCamera;
        private Canvas _canvas;

        private Vector2 _initialPosition;
        

        private void OnEnable()
        {
            _initialPosition = transform.localPosition;
            _yOffscreenPosition = _initialPosition.y - 20;

            SetInitialPosition();
            BeginAnimateIn();
        }

        private void SetInitialPosition()
        {
            var localPos = transform.localPosition;

            if (style == ToastStyle.TranslateTopToTop)
            {
                transform.localPosition = new Vector3(localPos.x, TopOffscreenPosition, localPos.z);
                return;
            }

            if (style == ToastStyle.TranslateBottomToTop)
            {
                transform.localPosition = new Vector3(localPos.x, BottomOffscreenPosition, localPos.z);
                return;
            }

            transform.localScale = Vector2.zero;
            transform.localPosition = new Vector3(localPos.x, _yOffscreenPosition, localPos.z);
        }

        private void BeginAnimateIn()
        {
            var yPosition = _initialPosition.y;
            if (style == ToastStyle.TranslateBottomToTop || style == ToastStyle.TranslateTopToTop)
            {
                LeanTween.moveLocalY(gameObject, yPosition, animateInSeconds)
                    .setEase(easeInStyle)
                    .setDelay(preDelaySeconds)
                    .setOnComplete(OnAnimateInComplete);
                return;
            }
            
            LeanTween.moveLocalY(gameObject, yPosition, animateInSeconds)
                .setEase(easeInStyle)
                .setDelay(preDelaySeconds)
                .setOnComplete(OnAnimateInComplete);

            LeanTween.scaleY(gameObject, 1, animateInSeconds)
                .setDelay(preDelaySeconds)
                .setEase(easeInStyle);

            LeanTween.scaleX(gameObject, 1, animateInSeconds / 2)
                .setDelay(preDelaySeconds)
                .setEase(easeInStyle);
        }

        private void Update()
        {
            // if the duration is set to zero, trigger animate out immediately once animateIn is complete
            if (_isDisplayed && displaySeconds == 0)
            {
                BeginAnimateOut();
                return;
            }

            // if we're still animating in, wait for it to finish
            if (!_isDisplayed)
                return;
            
            // begin counting the 'display time' (not animating in or out)
            _duration += Time.deltaTime;
            
            if (_duration >= displaySeconds)
            {
                BeginAnimateOut();
            }
            
        }
        

        private void BeginAnimateOut()
        {
            onToastComplete.Invoke();

            _isDisplayed = false;
            
            if (style == ToastStyle.TranslateBottomToTop || style == ToastStyle.TranslateTopToTop)
            {
                LeanTween.moveLocalY(gameObject, 150, animateInSeconds).setEase(easeOutStyle)
                    .setOnComplete(OnAnimateOutComplete);
                return;
            }
            
            LeanTween.moveLocalY(gameObject, _yOffscreenPosition, animateOutSeconds).setEase(easeOutStyle)
                .setOnComplete(OnAnimateOutComplete);
            LeanTween.scaleY(gameObject, 0, animateInSeconds).setEase(easeOutStyle);
            LeanTween.scaleX(gameObject, 0, animateInSeconds / 2).setEase(easeOutStyle);
        }

        private void OnAnimateOutComplete()
        {
            if (destroyOnComplete)
                Destroy(gameObject);
        }

        private void OnAnimateInComplete()
        {
            _isDisplayed = true;
            
        }

        public enum ToastStyle
        {
            Scale,
            TranslateBottomToTop,
            TranslateTopToTop
        }
    }
}