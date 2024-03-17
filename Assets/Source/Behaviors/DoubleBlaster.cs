using Source.Extensions;
using UnityEngine;

namespace Source.Behaviors
{
    [Tooltip("Used for actors that fire bullets in an alternating fashion.")]
    public class DoubleBlaster : MonoBehaviour
    {
        private bool _isLeftBlaster = false;
        
        [SerializeField]
        private GameObject bulletPrefab;

        [SerializeField]
        [Tooltip("Line this up with where the sprite's gun barrels are")]
        private float spread = 0.5f;
        
        public void Shoot()
        {
            var position = transform.position;
            _isLeftBlaster = !_isLeftBlaster;

            if (_isLeftBlaster)
            {
                Instantiate(bulletPrefab).At(position.x - spread, position.y);
            }
            else
            {
                Instantiate(bulletPrefab).At(position.x + spread, position.y);
            }

        }

        public void OnDrawGizmosSelected()
        {
            var position = transform.position;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(position.x-spread, position.y+1), 
                new Vector3(position.x-spread, position.y-1));
            Gizmos.DrawLine(new Vector3(position.x+spread, position.y+1), 
                new Vector3(position.x+spread, position.y-1));
        }
    }
}