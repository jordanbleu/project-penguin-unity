using Cinemachine;
using Source.Behaviors;
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
        private DoubleBlaster blaster;
        
        [SerializeField]
        private Rigidbody2D rigidBody;

        [SerializeField] 
        private GameObject playerLaserPrefab;

        [SerializeField] 
        private CinemachineImpulseSource cameraImpulseSource;
        
        private Vector2 _inputVelocity = new();

        private void FixedUpdate()
        {
            rigidBody.AddForce(_inputVelocity * thrust, ForceMode2D.Force);
        }

        #region Input Events

        private void OnMove(InputValue inputValue)
        {
            var inputVector = inputValue.Get<Vector2>();
            _inputVelocity = inputVector;
        }

        private void OnShoot(InputValue inputValue)
        {
            cameraImpulseSource.GenerateImpulse(0.25f);
            blaster.Shoot();
        }

        private void OnLaser(InputValue inputValue)
        {
            var position = transform.position;
            Instantiate(playerLaserPrefab).At(position.x, position.y + 8);
        }

        #endregion
        
    }
}