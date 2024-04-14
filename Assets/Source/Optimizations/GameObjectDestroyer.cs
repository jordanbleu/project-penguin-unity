using UnityEngine;

namespace Source.Optimizations
{
    [Tooltip("Simply exposes the destroy functionality for game objects, to be invoked from unity events.")]
    public class GameObjectDestroyer : MonoBehaviour
    {

        /// <summary>
        /// This will destroy the gameobject that this component is attached to.
        /// </summary>
        public void DestroySelf()
        {
            GameObject.Destroy(gameObject);
        }

        public void DeactivateSelf()
        {
            gameObject.SetActive(false);
        }

    }
}