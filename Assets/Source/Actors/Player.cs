using System;
using Cinemachine;
using Source.Behaviors;
using Source.Constants;
using Source.Data;
using Source.Dialogue;
using Source.Extensions;
using Source.Interfaces;
using Source.Optimizations;
using Source.Timers;
using Source.UI;
using Source.Weapons;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Source.Actors
{
    public class Player : MonoBehaviour
    {
        // how long after taking damage the player can take more damage
        private const float DamageCooldownMax = 1;
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
        private DialogueTyper dialogueTyper;

        [SerializeField]
        private LivesDisplay livesDisplay;

        [SerializeField]
        [Tooltip("Needed for going to the death scene.")]
        private SceneLoader sceneLoader;
        
        private Vector2 _inputVelocity = new();
        private static readonly int DamageAnimatorParam = Animator.StringToHash("damage");
        private int _lastDirection = 1;

        private float _currentHealthRegenDelay = 0f;
        private float _damageCooldownTime = 0f;
        
        // if true the player can't shoot
        private bool _isWeaponsLocked = false;
        // if true the player can't move 
        private bool _isMovementLocked = false;
        // if true the player will move itself to the center position for dialogue 
        private bool _isDialogueModeEnabled = false;

        private bool _healthRegenEnabled = true;
        
        private UnityEvent onUserInputEnter = new();
        private static readonly int DieAnimatorParam = Animator.StringToHash("die");
        private static readonly int HasMoreLivesAnimatorParam = Animator.StringToHash("hasMoreLives");

        /// <summary>
        /// Triggers when the player presses the Menu Enter button.
        ///
        /// Please please please follow up with "RemoveUserInputEnterListener" when you're done with this listener.
        /// </summary>
        /// <param name="action"></param>
        public void AddMenuEnterEventListener(UnityAction action)
            => onUserInputEnter.AddListener(action);
        
        /// <summary>
        /// Cleans up a registered event listener
        /// </summary>
        /// <param name="action"></param>
        public void RemoveMenuEnterEventListener(UnityAction action)
            => onUserInputEnter.RemoveListener(action);
        
        
        public bool ShieldProtectionEnabled { get; set; }
        
        private void Start()
        {
            var healthRegenInterval = gameObject.AddComponent<IntervalEventTimer>();
            healthRegenInterval.SetInterval(HealthRegenRate);
            healthRegenInterval.AddEventListener(RegenHealth);

            var energyRegenInterval = gameObject.AddComponent<IntervalEventTimer>();
            energyRegenInterval.SetInterval(EnergyRegenRate);
            energyRegenInterval.AddEventListener(RegenEnergy);
            
            dialogueTyper.OnDialogueBegin.AddListener(() => SetDialogueMode(true));
            dialogueTyper.OnDialogueComplete.AddListener(() => SetDialogueMode(false));
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
            if (!_healthRegenEnabled)
                return; 
            
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

            if (_isDialogueModeEnabled)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(0, -6), 4* dt);
            }

            DoubleCheckForOutOfBounds();
        }

        /// <summary>
        /// extra protection in case the player somehow gets forced out of bounds
        ///
        /// For example, without this method, if you push forward into enemies that are
        /// pushing you out of bounds you glitch through the wall.
        ///
        /// This should still be combined with physics based rigidbody walls though.
        /// </summary>
        private void DoubleCheckForOutOfBounds()
        {
            var pos = transform.position;

            var x = pos.x;
            var y = pos.y;
            
            if (pos.y < -10)
            {
                y = -8;
            } 
            else if (pos.y > 12)
            {
                y = 11;
            }

            if (x > 20)
            {
                x = 17;
            }
            else if (x < -19)
            {
                x = -16;
            }
            
            transform.position = new Vector2(x, y);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            var collisionResponder = other.gameObject.GetComponent<ICollideWithPlayerResponder>();
            collisionResponder?.CollideWithPlayer(this);
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

        /// <summary>
        /// Player has begun dying
        /// </summary>
        public void BeginDying()
        {
            _isMovementLocked = true;
            _isWeaponsLocked = true;
            _healthRegenEnabled = false;
            Stats.TrackDeath();
            Stats.Current.LivesRemaining--;
            
            animator.SetBool(HasMoreLivesAnimatorParam, Stats.Current.LivesRemaining >= 0);
            animator.SetTrigger(DieAnimatorParam);
        }

        /// <summary>
        /// (called from animator) player is done dying, and
        /// will either revive or we go to game over
        /// </summary>
        public void EndDying()
        {
            var lives = Stats.Current.LivesRemaining;

            if (lives >= 0)
            {
                health = 100;
                // animator will handle this transition, but  refresh the hud
                livesDisplay.Refresh();
                return;
            }

            sceneLoader.BeginFadingToScene(Scenes.GameOver);
        }

        /// <summary>
        /// (called from animator) player has finished reviving,
        /// time to play the game again
        /// </summary>
        public void EndRevive()
        {
            _isMovementLocked = false;
            _isWeaponsLocked = false;
            _healthRegenEnabled = true;
            
            // free shield
            shield.gameObject.SetActive(true);
            RefreshHud();
            healthDiplayBar.BounceUp();
        }

        public bool TakeDamage(int amount)
        {
            // not fair to damage player if they can't move lol
            if (_isMovementLocked)
                return false;
            
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
                Stats.TrackDamageBlockedByShield(amount);
                return true;
            }

            Stats.TrackDamageTaken(amount);

            healthDiplayBar.BounceUp();
            _currentHealthRegenDelay = HealthRegenDelay;
            animator.SetTrigger(DamageAnimatorParam);
            cameraImpulseSource.GenerateImpulse(amount/5f);
            health -= amount;
            _damageCooldownTime = DamageCooldownMax;
            RefreshHud();
            
            if (health <= 0)
            {
                BeginDying();
            }
            
            
            return true;
        }
        
        public void SetDialogueMode(bool enabled)
        {
            _isDialogueModeEnabled = enabled;
            _isWeaponsLocked = enabled;
            _isMovementLocked = enabled;
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
            if (_isMovementLocked)
            {
                _inputVelocity = Vector2.zero;
                return;
            }

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
            if (_isWeaponsLocked)
                return;

            Stats.TrackBulletFired();
            blaster.Shoot();
        }

        private void OnLaser(InputValue inputValue)
        {
            if (_isWeaponsLocked)
                return;
            
            var requiredEnergy = 30;

            if (!TryReduceEnergy(requiredEnergy))
                return;

            Stats.TrackLaser();
            var position = transform.position;
            Instantiate(playerLaserPrefab).At(position.x, position.y + 8);
        }

        private void OnDash(InputValue inputValue)
        {
            if (_isWeaponsLocked)
                return;
            
            var requiredEnergy = 10;
            
            if (!TryReduceEnergy(requiredEnergy))
                return;

            Stats.TrackPlayerDash();
            var position = transform.position;
            Instantiate(playerDashAnimationPrefab).At(position);
            rigidBody.AddForce(new(_lastDirection * dashThrust, 0), ForceMode2D.Impulse);
        }

        private void OnShield(InputValue inputValue)
        {
            if (_isWeaponsLocked)
                return;
            
            if (shield.gameObject.activeInHierarchy)
                return;

            var requiredEnergy = 20;

            if (!TryReduceEnergy(requiredEnergy))
                return;
            Stats.TrackShield();
            shield.gameObject.SetActive(true);
        }

        private void OnMissile(InputValue inputValue)
        {
            if (_isWeaponsLocked)
                return;
            
            var requiredEnergy = 30;

            if (!TryReduceEnergy(requiredEnergy))
                return;

            Stats.TrackMissile();
            Instantiate(playerMissilePrefab).At(transform.position);
        }

        private void OnMine(InputValue inputValue)
        {
            if (_isWeaponsLocked)
                return;
            
            var requiredEnergy = 30;

            if (!TryReduceEnergy(requiredEnergy))
                return;

            Stats.TrackMine();
            Instantiate(playerMinePrefab).At(transform.position);
        }
        
        private void OnForcefield(InputValue inputValue)
        {
            if (dialogueTyper is not null && dialogueTyper.isActiveAndEnabled)
            {
                dialogueTyper.UserCycleDialogue();
                return;
            }
            
            if (_isWeaponsLocked)
                return;
            
            var requiredEnergy = 50;

            if (!TryReduceEnergy(requiredEnergy))
                return;
            
            Stats.TrackForceField();
            var position = transform.position;
            var adjustedPosition = new Vector2(position.x, position.y + 0.75f);
            Instantiate(forcefieldPrefab).At(adjustedPosition);
        }

        
        
        private void OnMenuEnter(InputValue inputValue)
        {
            onUserInputEnter?.Invoke();
            
            // the logic below should maybe one day be consolidated into the event handling pattern
            // i will probably never do that.
            
            if (dialogueTyper is null || !dialogueTyper.isActiveAndEnabled)
                return;

            dialogueTyper.UserCycleDialogue();

        }

        #endregion
        
    }
}