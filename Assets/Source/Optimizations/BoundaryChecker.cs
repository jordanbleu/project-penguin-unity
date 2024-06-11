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

        [SerializeField] [Tooltip("If true, will flip to the opposite side of the boundaries if out of bounds (enemy behavior)")]
        private bool flipToOtherSide = false;
        
        [SerializeField]
        private UnityEvent onOutOfBounds = new();

        private void Update()
        {
            var position = transform.position;
            var x = position.x;
            var y = position.y;
            
            var minY = boundaryPosition.y - boundarySize.y;
            var maxY = boundaryPosition.y;
            
            if (y < minY)
            {
                if (flipToOtherSide)
                {
                    transform.position = new Vector2(x, maxY);
                }

                onOutOfBounds?.Invoke();
                return;
            }

            // todo: unsure if we need this, this checks if the object is out of bounds on the top
            // if (y > maxY)
            // {
            //     if (flipToOtherSide)
            //     {
            //         transform.position = new Vector2(x, minY);
            //     }
            //
            //     onOutOfBounds?.Invoke();
            //     return;
            // }

            var minX = boundaryPosition.x;
            var maxX = boundarySize.x + boundaryPosition.x;
            if (x < minX)
            {
                
                if (flipToOtherSide)
                {
                    transform.position = new Vector2(maxX, y);
                }

                
                onOutOfBounds?.Invoke();
                return;
            }

            if (x > maxX)
            {
                if (flipToOtherSide)
                {
                    transform.position = new Vector2(minX, y);
                }
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