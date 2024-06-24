using UnityEngine;


namespace Source.Utilities
{
    public static class UnityTransformUtils
    {
        public static Vector2 ConvertWorldToCanvasPosition(GameObject worldObject, Canvas canvas, Camera camera)
        {
            // Convert the world position to screen position
            Vector3 screenPosition = camera.WorldToScreenPoint(worldObject.transform.position);

            // Create a new 2D vector for the canvas position
            Vector2 canvasPosition;

            // Convert the screen position to a position on the canvas
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosition,
                canvas.worldCamera, out canvasPosition);

            return canvasPosition;
        }
    }
}