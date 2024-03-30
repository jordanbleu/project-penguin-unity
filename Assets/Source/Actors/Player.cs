using System;
using Cinemachine;
using Source.Behaviors;
using Source.Extensions;
using Source.Interfaces;
using Source.Timers;
using Source.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Source.Actors
{
    public class Player : MonoBehaviour
    {
        // how long after taking damage the player can take more damage
        private const float DamageCooldownMax = 2;
        private const float HealthRegenRate = 2f;
        private const float HealthRegenDelay = 8f;
        private const float EnergyRegenRate = 0.5f;
        
        [Tooltip("How much thrust is applied when moving ship.")]
        [SerializeField]
        private float thrust = 120;

        [SerializeField]
        [Tooltip("How much thrust is applied when dashing ")]
        private float dashThrust = 40;
        
        [Tooltip("Player's current HP.")]
        [SerializeField]
        private int health = 100;
        
        [Tooltip("Player's current energy.")]
        [SerializeField]
        private int energy = 100;
        
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

        [SerializeField]
        private DisplayBar healthDiplayBar;
        
        [SerializeField]
        private DisplayBar energyDiplayBar;
        
        private Vector2 _inputVelocity = new();
        private static readonly int DamageAnimatorParam = Animator.StringToHash("damage");
        private int _lastDirection = 1;

        private float _currentHealthRegenDelay = 0f;
        private float _collisionCooldownTime = 0f;

        private void Start()
        {
            var healthRegenInterval = gameObject.AddComponent<IntervalEventTimer>();
            healthRegenInterval.SetInterval(HealthRegenRate);
            healthRegenInterval.AddEventListener(RegenHealth);

            var energyRegenInterval = gameObject.AddComponent<IntervalEventTimer>();
            energyRegenInterval.SetInterval(EnergyRegenRate);
            energyRegenInterval.AddEventListener(RegenEnergy);
        }

        private void RegenEnergy()
        {
            if (energy == 100)
                return;

            if (energy >= 98)
            {
                energy = 100;
            }
            else
            {
                energy += 2;
            }
            RefreshHud();
        }

        // called on interval for health regen
        private void RegenHealth()
        {
            if (_currentHealthRegenDelay > 0f)
                return;
            
            if (health == 100)
                return;
            
            if (health >= 97)
            {
                health = 100;
            }
            else
            {
                health += 3;
            }

            RefreshHud();
        }

        private void RefreshHud()
        {
            energyDiplayBar.SetValue(energy / 100f);
            healthDiplayBar.SetValue(health / 100f);
        }

        private void FixedUpdate()
        {
            rigidBody.AddForce(_inputVelocity * thrust, ForceMode2D.Force);
        }

        private void Update()
        {
            var dt = Time.deltaTime;
            
            if (_collisionCooldownTime > 0f)
                _collisionCooldownTime -= dt;

            if (_currentHealthRegenDelay > 0f)
                _currentHealthRegenDelay -= dt;

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
            _currentHealthRegenDelay = HealthRegenDelay;
            animator.SetTrigger(DamageAnimatorParam);
            cameraImpulseSource.GenerateImpulse(amount/5f);
            health -= amount;
            _collisionCooldownTime = DamageCooldownMax;
            RefreshHud();
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
            var requiredEnergy = 30;

            if (energy < requiredEnergy)
            {
                energyDiplayBar.Error();
                return;
            }

            var position = transform.position;
            Instantiate(playerLaserPrefab).At(position.x, position.y + 8);

            energy -= requiredEnergy;

            RefreshHud();

        }

        private void OnDash(InputValue inputValue)
        {
            var requiredEnergy = 10;
            
            if (energy < requiredEnergy)
            {
                energyDiplayBar.Error();
                return;
            }
            
            var position = transform.position;
            Instantiate(playerDashAnimationPrefab).At(position);
            rigidBody.AddForce(new(_lastDirection * dashThrust, 0), ForceMode2D.Impulse);

            energy -= requiredEnergy;
            RefreshHud();
        }


        #endregion
        
    }
}