using System;
using Source.Extensions;
using UnityEngine;

namespace Source.Physics
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class StaticVelocity : MonoBehaviour
    {
        [SerializeField]
        private Vector2 staticVelocity = Vector2.zero;

        [SerializeField]
        [Tooltip("How quickly the object will pull back to their static velocity if an external force changes their actual velocity")]
        private float decelerationRate = 1f;

        private Rigidbody2D _rigidBody;

        private void Start()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            var staticX = staticVelocity.x;
            var staticY = staticVelocity.y;


            var velocity = _rigidBody.velocity;
            var x = velocity.x;
            var y = velocity.y;

            var newX = x;
            var newY = y;

            if (!x.IsWithin(decelerationRate, staticX))
            {
                newX = x.Stabilize(decelerationRate, staticX);
            }
            
            if (!y.IsWithin(decelerationRate, staticY))
            {
                newY = y.Stabilize(decelerationRate, staticY);
            }

            _rigidBody.velocity = new(newX, newY);
        }
    }
}