using System;
using Cinemachine;
using Source.Behaviors;
using Source.Dialogue;
using Source.Extensions;
using Source.Interfaces;
using Source.Timers;
using Source.UI;
using Source.Weapons;
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
        private GameObject playerMissilePrefab;

        [SerializeField]
        private GameObject playerMinePrefab;

        [SerializeField]
        private GameObject forcefieldPrefab;
        
        [SerializeField]
        private GameObject shieldAborbEffectPrefab;
        
        [SerializeField] 
        private CinemachineImpulseSource cameraImpulseSource;

        [SerializeField] 
        private Animator animator;

        [SerializeField]
        private DisplayBar healthDiplayBar;
        
        [SerializeField]
        private DisplayBar energyDiplayBar;

        [SerializeField]
        private PlayerShield shield;

        [SerializeField]
        private TextTyper textTyper;
        
        private Vector2 _inputVelocity = new();
        private static readonly int DamageAnimatorParam = Animator.StringToHash("damage");
        private int _lastDirection = 1;

        private float _currentHealthRegenDelay = 0f;
        private float _damageCooldownTime = 0f;

        public bool ShieldProtectionEnabled { get; set; }
        
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
                energy += 4;
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
            
            if (_damageCooldownTime > 0f)
                _damageCooldownTime -= dt;

            if (_currentHealthRegenDelay > 0f)
                _currentHealthRegenDelay -= dt;

        }

        private void OnCollisionStay2D(Collision2D other)
        {
            var collisionResponder = other.gameObject.GetComponent<ICollideWithPlayerResponder>();

            collisionResponder?.CollideWithPlayer(this);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            var gameObj = other.gameObject;

            if (!gameObj.TryGetComponent<IEnemyProjectile>(out var enemyProjectile)) 
                return;

            enemyProjectile.HitPlayer(this);
        }

        public void AddForceToPlayer(Vector2 force)
            => rigidBody.AddForce(force, ForceMode2D.Impulse);
        
        public bool TakeDamage(int amount)
        {
            if (_damageCooldownTime > 0)
                return false;
            
            if (ShieldProtectionEnabled)
            {
                // spawn an absorb effect somewhere around the player
                var position = transform.position;
                var ox = position.x + UnityEngine.Random.Range(-0.5f, 0.5f);
                var oy = position.y + UnityEngine.Random.Range(-0.5f, 0.5f);

                _damageCooldownTime = DamageCooldownMax;
                Instantiate(shieldAborbEffectPrefab).At(ox, oy);
                return true;
            }

            healthDiplayBar.BounceUp();
            _currentHealthRegenDelay = HealthRegenDelay;
            animator.SetTrigger(DamageAnimatorParam);
            cameraImpulseSource.GenerateImpulse(amount/5f);
            health -= amount;
            _damageCooldownTime = DamageCooldownMax;
            RefreshHud();
            return true;
        }

        /// <summary>
        /// If the player has enough energy, reduced it and refreshes the hud.
        /// else, does the energy bar error animation and return false.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        private bool TryReduceEnergy(int amount)
        {
            if (energy < amount)
            {
                energyDiplayBar.BounceUp();
                return false;
            }

            energy -= amount;
            RefreshHud();
            return true;
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

            if (!TryReduceEnergy(requiredEnergy))
                return;

            var position = transform.position;
            Instantiate(playerLaserPrefab).At(position.x, position.y + 8);
        }

        private void OnDash(InputValue inputValue)
        {
            var requiredEnergy = 10;
            
            if (!TryReduceEnergy(requiredEnergy))
                return;
            
            var position = transform.position;
            Instantiate(playerDashAnimationPrefab).At(position);
            rigidBody.AddForce(new(_lastDirection * dashThrust, 0), ForceMode2D.Impulse);
        }

        private void OnShield(InputValue inputValue)
        {
            if (shield.gameObject.activeInHierarchy)
                return;

            var requiredEnergy = 20;

            if (!TryReduceEnergy(requiredEnergy))
                return;
            
            shield.gameObject.SetActive(true);
        }

        private void OnMissile(InputValue inputValue)
        {
            var requiredEnergy = 30;

            if (!TryReduceEnergy(requiredEnergy))
                return;

            Instantiate(playerMissilePrefab).At(transform.position);
        }

        private void OnMine(InputValue inputValue)
        {
            var requiredEnergy = 30;

            if (!TryReduceEnergy(requiredEnergy))
                return;

            Instantiate(playerMinePrefab).At(transform.position);
        }

        private void OnForcefield(InputValue inputValue)
        {
            var requiredEnergy = 50;

            if (!TryReduceEnergy(requiredEnergy))
                return;
            var position = transform.position;
            var adjustedPosition = new Vector2(position.x, position.y + 0.75f);
            Instantiate(forcefieldPrefab).At(adjustedPosition);
        }

        private void OnMenuEnter(InputValue inputValue)
        {
            if (textTyper is null || !textTyper.isActiveAndEnabled)
                return;

            textTyper.CycleDialogue();

        }

        #endregion
        
    }
}