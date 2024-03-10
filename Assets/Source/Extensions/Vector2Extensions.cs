using UnityEngine;

namespace Source.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2 Clone(this Vector2 vector2) =>
            new Vector2(vector2.x, vector2.y);
    }
}