using UnityEngine;

namespace Source.Extensions
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Use with Instantiate() to set a gameobject's position
        /// </summary>
        public static GameObject At(this GameObject gameObject, float x, float y)
        {
            gameObject.transform.position = new Vector2(x, y);
            return gameObject;
        }
        
        /// <summary>
        /// Use with Instantiate() to set a gameobject's position
        /// </summary>
        public static GameObject At(this GameObject gameObject, Vector2 position)
        {
            gameObject.transform.position = position;
            return gameObject;
        }
        
        /// <summary>
        /// Use with Instantiate() to set a gameobject's position
        /// </summary>
        public static GameObject AtLocal(this GameObject gameObject, float x, float y)
        {
            gameObject.transform.localPosition = new Vector2(x, y);
            return gameObject;
        }
        
        public static GameObject WithName(this GameObject gameObject, string name)
        {
            gameObject.name = name;
            return gameObject;
        }

    }
}