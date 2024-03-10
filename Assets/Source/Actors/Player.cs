using System;
using Source.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Source.Actors
{
    public class Player : MonoBehaviour
    {
        [Tooltip("How much thrust is applied when moving ship.")]
        [SerializeField]
        private float thrust = 120;
        
        
        [SerializeField]
        private Rigidbody2D rigidBody;

        private Vector2 _inputVelocity = new();

        private void FixedUpdate()
        {
            rigidBody.AddForce(_inputVelocity.Clone(), ForceMode2D.Force);
        }


        #region Input Events

        private void OnMove(InputValue inputValue)
        {
            var inputVector = inputValue.Get<Vector2>();
            _inputVelocity = inputVector * thrust;
        }

        #endregion
        
    }
}