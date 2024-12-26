using System;
using TMPro;
using UnityEngine;

namespace Source.UI
{
    /// <summary>
    /// Basically a simpler version of Toast but works better for repeated alerts while it's still active
    /// (feel free to spam these alerts ;) )
    ///
    /// Animates in from top, displays, then goes away.
    /// </summary>
    public class Alert : MonoBehaviour
    {
        private const float TopOffscreenPosition = 200;
        
        [SerializeField]
        private float defaultDisplaySeconds = 6f;

        [SerializeField]
        private TextMeshProUGUI textMesh;
        
        private Vector2 displayPosition;

        private float elapsedDisplayTime = 0f;

        private bool isAnimating = false;   

        private void Start()
        {
            displayPosition = new Vector2(transform.localPosition.x, 50);
            transform.localPosition = new(transform.localPosition.x, TopOffscreenPosition);
        }

        public void Show(string text, float? secondsToDisplay = null)
        {
            isAnimating = true;
            
            if (LeanTween.isTweening(gameObject))
            {
                LeanTween.cancel(gameObject);
            }
            
            transform.localPosition = new Vector2(transform.localPosition.x, TopOffscreenPosition);
            textMesh.SetText(text);

            isAnimating = true;
            LeanTween.moveLocalY(gameObject, displayPosition.y, 0.5f)
                .setEase(LeanTweenType.easeOutBounce)
                .setOnComplete(() => isAnimating = false);
            
            elapsedDisplayTime = (secondsToDisplay ?? defaultDisplaySeconds);
        }

        private void Update()
        {
            if (isAnimating)
            {
                return;
            }
            
            if (elapsedDisplayTime > 0f)
            {
                elapsedDisplayTime -= Time.deltaTime; 
                return;
            }

            if (elapsedDisplayTime <= 0f)
            {
                isAnimating = true;
                
                LeanTween.moveLocalY(gameObject, TopOffscreenPosition, 0.5f)
                    .setEase(LeanTweenType.easeInQuart)
                    .setOnComplete(() => isAnimating = false);
            }
        }
    }
}