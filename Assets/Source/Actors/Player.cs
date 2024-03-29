using System;
using Cinemachine;
using Source.Behaviors;
using Source.Extensions;
using Source.Interfaces;
using Source.Projectiles;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Source.Actors
{
    public class Player : MonoBehaviour
    {
        // how long after taking damage the player can take more damage
        private const float DamageCooldownMax = 2;
        private float _collisionCooldownTime = 0f;
        
        [Tooltip("How much thrust is applied when moving ship.")]
        [SerializeField]
        private float thrust = 120;

        [SerializeField]
        [Tooltip("How much thrust is applied when dashing ")]
        private float dashThrust = 40;
        
        [Tooltip("Player's current HP.")]
        [SerializeField]
        private int health = 100;
        
        [SerializeField]
        private DoubleBlaster blaster;
        
        [SerializeField]
        private Rigidbody2D rigidBody;

        [SerializeField] 
        private GameObject playerLaserPrefab;

        [SerializeField]
        private GameObject playerDashAnimationPrefab;
        
        [SerializeField] 
        private CinemachineImpulseSource cameraImpulseSource;

        [SerializeField] 
        private Animator animator;
        
        private Vector2 _inputVelocity = new();
        private static readonly int DamageAnimatorParam = Animator.StringToHash("damage");
        private int _lastDirection = 1;
        
        private void FixedUpdate()
        {
            rigidBody.AddForce(_inputVelocity * thrust, ForceMode2D.Force);
        }

        private void Update()
        {
            if (_collisionCooldownTime > 0f)
                _collisionCooldownTime -= Time.deltaTime;
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (_collisionCooldownTime > 0f)
                return;

            var collisionResponder = other.gameObject.GetComponent<ICollideWithPlayerResponder>();

            collisionResponder?.CollideWithPlayer(this);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            var gameObj = other.gameObject;

            if (gameObj.TryGetComponent<IEnemyProjectile>(out var enemyProjectile))
            {
                enemyProjectile.HitPlayer(this);
            }
        }

        public void AddForceToPlayer(Vector2 force)
            => rigidBody.AddForce(force, ForceMode2D.Impulse);
        
        public void TakeDamage(int amount)
        {
            animator.SetTrigger(DamageAnimatorParam);
            cameraImpulseSource.GenerateImpulse(amount/5f);
            health -= amount;
            _collisionCooldownTime = DamageCooldownMax;
        }

        #region Input Events

        private void OnMove(InputValue inputValue)
        {
            var inputVector = inputValue.Get<Vector2>();
            var xDirection = Math.Sign(inputVector.x);

            if (xDirection != 0)
            {
                _lastDirection = xDirection;
            }

            _inputVelocity = inputVector;
        }

        private void OnShoot(InputValue inputValue)
        {
            blaster.Shoot();
        }

        private void OnLaser(InputValue inputValue)
        {
            var position = transform.position;
            Instantiate(playerLaserPrefab).At(position.x, position.y + 8);
        }

        private void OnDash(InputValue inputValue)
        {
            var position = transform.position;
            Instantiate(playerDashAnimationPrefab).At(position);
            rigidBody.AddForce(new(_lastDirection * dashThrust, 0), ForceMode2D.Impulse);
        }


        #endregion
        
    }
}