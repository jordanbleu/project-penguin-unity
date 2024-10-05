using UnityEngine;
using UnityEngine.UI;

namespace Source.UI
{
    public class DisplayBar : MonoBehaviour
    {
        private float _errorTweenOffset = 20f;
        
        [SerializeField]
        private Image barImage;

        [SerializeField]
        private bool useSmoothing = true;

        [SerializeField]
        private bool isVerticalBar = true;
        
        /// <summary>
        /// Set how full the bar is from 0 to 100
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(float value)
        {
            var obj = barImage.gameObject;
            
            if (value < 0)
                value = 0;

            if (useSmoothing)
            {
                if (LeanTween.isTweening(obj))
                {
                    LeanTween.cancel(obj);
                }

                if (isVerticalBar)
                {
                    LeanTween.scaleY(barImage.gameObject, value, 0.25f)
                        .setEase(LeanTweenType.easeOutQuad);
                }
                else
                {
                    LeanTween.scaleX(barImage.gameObject, value, 0.25f)
                        .setEase(LeanTweenType.easeOutQuad);
                }
            }
            else
            {
                var localScale = transform.localScale;
                
                if (isVerticalBar)
                {
                    barImage.gameObject.transform.localScale = new Vector3(localScale.x, value, localScale.z);
                }
                else
                {
                    barImage.gameObject.transform.localScale = new Vector3(value, localScale.y, localScale.z);
                }
            }
        }

        public void BounceDown()
        {
            var parent = barImage.gameObject.transform.parent.gameObject;

            if (LeanTween.isTweening(parent))
                return;
            
            var position = parent.transform.position;

            LeanTween.moveY(parent, position.y - _errorTweenOffset, 0.5f)
                .setEase(LeanTweenType.punch)
                .setOnComplete(()=>LeanTween.moveY(parent, position.y, 0.5f));
        }
        
        public void BounceUp()
        {
            
            var parent = barImage.gameObject.transform.parent.gameObject;

            if (LeanTween.isTweening(parent))
                return;
            
            var position = parent.transform.position;

            LeanTween.moveY(parent, position.y + _errorTweenOffset, 0.5f)
                .setEase(LeanTweenType.punch)
                .setOnComplete(()=>LeanTween.moveY(parent, position.y, 0.5f));
        }
    }
}