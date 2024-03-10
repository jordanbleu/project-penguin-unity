using System;
using UnityEngine;
using UnityEngine.Events;

namespace Source.Optimizations
{
    
    [Tooltip("Perform actions when the game object leaves it's boundaries.")]
    public class BoundaryChecker : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Top left position of the boundary box")]
        private Vector2 boundaryPosition = new Vector2(0, 0);

        [SerializeField] 
        [Tooltip("Width / height of the boundary box")]
        private Vector2 boundarySize = new Vector2(10, 10);
        
        [SerializeField]
        private UnityEvent onOutOfBounds = new();

        private void Update()
        {
            var position = transform.position;
            var x = position.x;
            var y = position.y;
            
            var minY = boundaryPosition.y - boundarySize.y;
            if (y < minY)
            {
                onOutOfBounds?.Invoke();
                return;
            }

            var maxY = boundaryPosition.y;
            if (y > maxY)
            {
                onOutOfBounds?.Invoke();
                return;
            }

            var minX = boundaryPosition.x;
            if (x < minX)
            {
                onOutOfBounds?.Invoke();
                return;
            }

            var maxX = boundarySize.x + boundaryPosition.x;
            if (x > maxX)
            {
                onOutOfBounds?.Invoke();
                return;
            }

        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            // top line
            Gizmos.DrawLine(new Vector2(boundaryPosition.x, boundaryPosition.y),
                new Vector2(boundaryPosition.x+boundarySize.x, boundaryPosition.y));
            
            // left line 
            Gizmos.color = Color.green;

            Gizmos.DrawLine(new Vector2(boundaryPosition.x, boundaryPosition.y),
                new Vector2(boundaryPosition.x, boundaryPosition.y-boundarySize.y));
            
            // right line
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(new Vector2(boundaryPosition.x + boundarySize.x, boundaryPosition.y),
                new Vector2(boundaryPosition.x+boundarySize.x, boundaryPosition.y-boundarySize.y));
            
            // bottom line
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(new Vector2(boundaryPosition.x, boundaryPosition.y-boundarySize.y),
                new Vector2(boundaryPosition.x+boundarySize.x, boundaryPosition.y-boundarySize.y));
        }
    }
    
    
}