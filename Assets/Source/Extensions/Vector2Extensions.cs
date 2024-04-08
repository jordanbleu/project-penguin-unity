using UnityEngine;

namespace Source.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2 Clone(this Vector2 vector2) =>
            new Vector2(vector2.x, vector2.y);

        /// <summary>
        /// Returns a new vector2 that adds or subtracts the specified offsets.
        /// </summary>
        /// <param name="vector2"></param>
        /// <param name="x">How far to move the x position</param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Vector2 Move(this Vector2 vector2, float x, float y) =>
            new Vector2(vector2.x + x, vector2.y + y);
        
    }
}