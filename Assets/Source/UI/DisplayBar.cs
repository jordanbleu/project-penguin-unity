using UnityEngine;
using UnityEngine.UI;

namespace Source.UI
{
    public class DisplayBar : MonoBehaviour
    {
        private float _errorTweenOffset = 20f;
        
        [SerializeField]
        private Image barImage;

        /// <summary>
        /// Set how full the bar is from 0 to 100
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(float value)
        {
            var obj = barImage.gameObject;
            
            if (value < 0)
                value = 0;

            if (LeanTween.isTweening(obj))
            {
                LeanTween.cancel(obj);
            }

            LeanTween.scaleY(barImage.gameObject, value, 0.25f)
                .setEase(LeanTweenType.easeOutQuad);
        }

        public void BounceDown()
        {
            var parent = barImage.gameObject.transform.parent.gameObject;

            if (LeanTween.isTweening(parent))
                return;
            
            var position = parent.transform.position;

            LeanTween.moveY(parent, position.y + _errorTweenOffset, 0.5f)
                .setEase(LeanTweenType.punch)
                .setOnComplete(()=>LeanTween.moveY(parent, position.y, 0.5f));
        }
        
        public void BounceUp()
        {
            var parent = barImage.gameObject.transform.parent.gameObject;

            if (LeanTween.isTweening(parent))
                return;
            
            var position = parent.transform.position;

            LeanTween.moveY(parent, position.y - _errorTweenOffset, 0.5f)
                .setEase(LeanTweenType.punch)
                .setOnComplete(()=>LeanTween.moveY(parent, position.y, 0.5f));
        }
    }
}