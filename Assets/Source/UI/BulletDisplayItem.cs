using System;
using UnityEngine;
using UnityEngine.UI;

namespace Source.UI
{
    [RequireComponent(typeof(Image))]
    public class BulletDisplayItem : MonoBehaviour
    {
        private Image image;
        
        private void Start()
        {
            image = GetComponent<Image>();
        }

        public void Show(float yPosition)
        {
            if (LeanTween.isTweening(gameObject))
                LeanTween.cancel(gameObject);

            image.color = Color.white;
            
            LeanTween.moveLocalY(gameObject, yPosition, 0.25f)
                .setEase(LeanTweenType.easeOutBack);
        }
        
        public void Hide(float yPosition)
        {
            if (LeanTween.isTweening(gameObject))
                LeanTween.cancel(gameObject);

            image.color = Color.grey;
            
            LeanTween.moveLocalY(gameObject, yPosition, 0.25f)
                .setEase(LeanTweenType.easeInBack);
        }
    }
}