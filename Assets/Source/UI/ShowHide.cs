using System;
using UnityEngine;

namespace Source.UI
{
    /// <summary>
    /// Allows the element to be moved off screen or back on screen, deactivating the object when not shown.
    ///
    /// Put the object into its onscreen position, then set the offScreenPositionOffset to where you want it to go when hidden.
    /// </summary>
    public class ShowHide : MonoBehaviour
    {
        [SerializeField]
        private Vector2 offScreenPositionOffset = Vector2.zero;

        [SerializeField]
        private float animationSeconds = 1f;
        
        [SerializeField]
        private bool startActive;
        
        private Vector2 _onScreenPosition;
        private Vector2 _offScreenPosition;
        private void Start()
        {
            
            _onScreenPosition = transform.localPosition;
            _offScreenPosition = _onScreenPosition + offScreenPositionOffset;

            if (startActive)
            {
                gameObject.SetActive(true);
            }
            else
            {
                transform.localPosition = _offScreenPosition;
                gameObject.SetActive(false);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            if (LeanTween.isTweening(gameObject))
                LeanTween.cancel(gameObject);

            LeanTween.moveLocal(gameObject, _onScreenPosition, animationSeconds)
                .setEase(LeanTweenType.easeOutBack);
        }

        public void Hide()
        {
            if (LeanTween.isTweening(gameObject))
                LeanTween.cancel(gameObject);
            
            LeanTween.moveLocal(gameObject, _offScreenPosition, animationSeconds)
                .setEase(LeanTweenType.easeInBack)
                .setOnComplete(() => gameObject.SetActive(false));
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            
            // Convert local position to world position
            Vector3 worldPosition = transform.position;

            // Convert local position + offScreenPositionOffset to world position
            Vector3 offsetWorldPosition = transform.TransformPoint(offScreenPositionOffset);

            // Draw a line from worldPosition to offsetWorldPosition
            Gizmos.DrawLine(worldPosition, offsetWorldPosition);
        }
    }
}