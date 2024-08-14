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
        private OffScreenPosition offScreenPosition = OffScreenPosition.Bottom;

        [SerializeField]
        private bool show = true;

        [SerializeField]
        private float animationSeconds = 1f;
        
        private Vector2 _startPosition;

        private void Start()
        {
            _startPosition = (Vector2)transform.localPosition;

            if (show)
                Show();
            else
                Hide();
        }

        public void Show()
        {
            if (LeanTween.isTweening(gameObject))
                LeanTween.cancel(gameObject);

            LeanTween.moveLocal(gameObject, _startPosition, animationSeconds)
                .setEase(LeanTweenType.easeOutBack);
        }

        public void Hide()
        {
            if (LeanTween.isTweening(gameObject))
                LeanTween.cancel(gameObject);

            LeanTween.moveLocal(gameObject, GetOffScreen(), animationSeconds)
                .setEase(LeanTweenType.easeInBack);
        }

        private Vector2 GetOffScreen()
        {
            switch (offScreenPosition)
            {
                case OffScreenPosition.Bottom:
                    return new(_startPosition.x, _startPosition.y -50);
                case OffScreenPosition.Top:
                    return new(_startPosition.x, _startPosition.y + 50);
                case OffScreenPosition.Left:
                    return new(_startPosition.x -50, _startPosition.y);
                case OffScreenPosition.Right:
                    return new Vector2(_startPosition.x + 50, _startPosition.y);
            }

            return Vector2.zero;
        }
        
        
        public enum OffScreenPosition
        {
            Top,
            Bottom,
            Left,
            Right
        }
    }
}