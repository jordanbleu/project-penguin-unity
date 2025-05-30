using System;
using Cinemachine;
using Source.Audio;
using Source.Behaviors;
using Source.Constants;
using Source.Data;
using Source.Dialogue;
using Source.Extensions;
using Source.GameData;
using Source.Interfaces;
using Source.Optimizations;
using Source.Timers;
using Source.UI;
using Source.Utilities;
using Source.Weapons;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Source.Actors
{
    /// <summary>
    /// Unorganized God class for the player.
    /// </summary>
    public class Player : MonoBehaviour
    {
        // how long after taking damage the player can take more damage
        private const float DamageCooldownMax = 1;
        private const float HealthRegenRate = 2f;
        private const float HealthRegenDelay = 8f;
        private const float EnergyRegenRate = 0.5f;
        private const float ReloadSpeed = 50;

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
        private PlayerShield shield;

        [SerializeField]
        private AudioClip[] dashSounds;

        [SerializeField]
        private AudioClip[] damages;
        
        // required to populate these on each scene
        [SerializeField]
        private DisplayBar healthDiplayBar;
        
        [SerializeField]
        private DisplayBar energyDiplayBar;

        [SerializeField]
        private DialogueTyper dialogueTyper;

        [SerializeField]
        private LivesDisplay livesDisplay;

        [SerializeField]
        [Tooltip("Needed for going to the death scene.")]
        private SceneLoader sceneLoader;

        [SerializeField]
        private AudioClip reloadCompleteSound;
        
        [SerializeField]
        private AudioClip reloadFailSound;

        [SerializeField]
        private AudioClip reloadSuccessSound;

        [SerializeField]
        private AudioClip playerDeathSound;

        [SerializeField]
        private AudioClip[] playerHitSounds;
        
        private StatsTracker _statsTracker;
        
        public int LivesRemaining {get;set;} = GameplayConstants.TotalLives;
        
        // Reload Mechanics
        //
        // Bullets remaining in current mag
        private short _remainingBullets = GameplayConstants.MagSize;
        public short RemainingBullets => _remainingBullets;
        
        // how much time is remaining before the player can shoot again
        private float _reloadTimeRemaining = 0f;
        public float RemainingReloadTime => _reloadTimeRemaining;   
        // if the user failed the current active reload, this will be true
        private bool _failedReload = false;
        public bool FailedReload => _failedReload;
        public UnityEvent OnActiveReloadSuccess { get;} = new();
        public UnityEvent OnActiveReloadFailure { get; } = new();
        public UnityEvent OnActiveReloadBegin { get; } = new();
        public UnityEvent OnActiveReloadEnd { get; } = new();
        public UnityEvent OnPlayerShoot { get; } = new();
        /// <summary>
        /// Called when the player manually pressed R to reload
        /// </summary>
        public UnityEvent OnManualReload { get; } = new();


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
        
        public int CurrentCombo {get; private set;} = 0;
        
        private static readonly int DieAnimatorParam = Animator.StringToHash("die");
        private static readonly int HasMoreLivesAnimatorParam = Animator.StringToHash("hasMoreLives");

        // This is needed because unity doesn't expose a way to count event listeners added during runtime
        private int _onUserInputListenerCount = 0;
        
        /// <summary>
        /// Triggers when the player presses the Menu Enter button.
        ///
        /// Please please please follow up with "RemoveUserInputEnterListener" when you're done with this listener.
        /// </summary>
        /// <param name="action"></param>
        public void AddMenuEnterEventListener(UnityAction action)
        {
            _onUserInputListenerCount++;
            onUserInputEnter.AddListener(action);
        }
        
        /// <summary>
        /// Cleans up a registered event listener
        /// </summary>
        /// <param name="action"></param>
        public void RemoveMenuEnterEventListener(UnityAction action)
        {
            _onUserInputListenerCount--;
            onUserInputEnter.RemoveListener(action);
        }    

        private SoundEffectEmitter _soundEmitter;

        private Collider2D _collider;
        
        private MusicBox _musicBox; 
        
        public bool ShieldProtectionEnabled { get; set; }
        
        private void Start()
        {
            _musicBox =  GameObject.FindWithTag(Tags.MusicBox)?.GetComponent<MusicBox>();
            
            var healthRegenInterval = gameObject.AddComponent<IntervalEventTimer>();
            healthRegenInterval.SetInterval(HealthRegenRate);
            healthRegenInterval.AddEventListener(RegenHealth);

            var energyRegenInterval = gameObject.AddComponent<IntervalEventTimer>();
            energyRegenInterval.SetInterval(EnergyRegenRate);
            energyRegenInterval.AddEventListener(RegenEnergy);
            
            dialogueTyper.OnDialogueBegin.AddListener(() => SetDialogueMode(true));
            dialogueTyper.OnDialogueComplete.AddListener(() => SetDialogueMode(false));
            
            _soundEmitter = GameObject.FindWithTag(Tags.SoundEffectEmitter).GetComponent<SoundEffectEmitter>();
            
            _collider = GetComponent<Collider2D>();
            
            _statsTracker = GameObject.FindWithTag(Tags.StatsTracker).GetComponent<StatsTracker>()
                            ?? throw new UnityException("Missing StatsTracker in scene");
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
            
            // also update the music state
            if (_musicBox)
                _musicBox.EnableLowHealthEffect = (health < 15);
            
            if (_soundEmitter)
                _soundEmitter.EnableLowHealthEffect = (health < 15);
            
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

            if (_reloadTimeRemaining > 0)
            {
                var reloadSpeed = _failedReload ? ReloadSpeed*0.5f : ReloadSpeed;
                _reloadTimeRemaining -= dt * reloadSpeed;

                // the reload bar naturally filled up
                if (_reloadTimeRemaining <= 0f)
                {
                    _soundEmitter.Play(gameObject, reloadCompleteSound);
                    _failedReload = false;
                    _remainingBullets = GameplayConstants.MagSize;
                    _reloadTimeRemaining = 0f;
                    OnActiveReloadEnd?.Invoke();
                }
            }

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
            _soundEmitter.EnableLowHealthEffect = false;
            _musicBox.EnableLowHealthEffect = false;
            
            _soundEmitter.Play(gameObject, playerDeathSound);
            _isMovementLocked = true;
            _isWeaponsLocked = true;
            _healthRegenEnabled = false;
            _statsTracker.Stats.Deaths++;
            LivesRemaining--;
            
            animator.SetBool(HasMoreLivesAnimatorParam, LivesRemaining >= 0);
            animator.SetTrigger(DieAnimatorParam);
        }

        /// <summary>
        /// (called from animator) player is done dying, and
        /// will either revive or we go to game over
        /// </summary>
        public void EndDying()
        {
            var lives = LivesRemaining;

            if (lives >= 0)
            {
                health = 100;
                // animator will handle this transition, but  refresh the hud
                livesDisplay.UpdateLives(lives);
                return;
            }
            
            // Game over. Save the current stats then fade to the game over scene
            _statsTracker.BeginSaving(false, () => sceneLoader.BeginFadingToScene(Scenes.GameOver));
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
                
                _statsTracker.Stats.DamageBlockedByShield+=amount;
                
                return true;
                
                // todo: play a shield hit sound
            }

            _statsTracker.Stats.DamageTaken += amount;

            healthDiplayBar.BounceUp();
            _currentHealthRegenDelay = HealthRegenDelay;
            animator.SetTrigger(DamageAnimatorParam);
            cameraImpulseSource.GenerateImpulse(amount/5f);
            health -= amount;
            _damageCooldownTime = DamageCooldownMax;
            RefreshHud();
            _soundEmitter.PlayRandom(gameObject, playerHitSounds);
            
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
        
        public void ResetCombo()
        {
            CurrentCombo = 0;
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
            
            // cannot shoot while something is waiting for you to press enter
            if (_onUserInputListenerCount > 0)
                return;

            // Player tried to shoot during a reload
            if (_reloadTimeRemaining > 0)
            {
                // user gets one chance at active reload
                if (_failedReload)
                    return;

                var startThresh = 33;
                var endThresh = 66;
                
                // Successful active reload
                if (_reloadTimeRemaining > startThresh && _reloadTimeRemaining < endThresh)
                {
                    _remainingBullets = GameplayConstants.MagSize;
                    _reloadTimeRemaining = 0;
                    _failedReload = false;
                    OnActiveReloadSuccess?.Invoke();
                    OnActiveReloadEnd?.Invoke();
                    _soundEmitter.Play(gameObject, reloadSuccessSound);
                    return;
                }
                
                // not so succsessful active reload
                _failedReload = true;
                OnActiveReloadFailure?.Invoke();
                _soundEmitter.Play(gameObject, reloadFailSound);
                return;
            }

            _statsTracker.Stats.BulletsFired++;
            
            // combo logic
            CurrentCombo++;
            
            if (CurrentCombo > _statsTracker.Stats.BestCombo)
                _statsTracker.Stats.BestCombo = CurrentCombo;
            
            blaster.Shoot();
            _remainingBullets--;
            OnPlayerShoot?.Invoke();

            if (_remainingBullets <= 0)
            {
                Reload();
            }
        }

        private void Reload()
        {
            OnActiveReloadBegin?.Invoke();
            _reloadTimeRemaining = 100;
            _remainingBullets = 0;
        }

        private void OnLaser(InputValue inputValue)
        {
            return; // disabled for demo
            if (_isWeaponsLocked)
                return;
            
            var requiredEnergy = 30;

            if (!TryReduceEnergy(requiredEnergy))
                return;

            _statsTracker.Stats.Lasers++;
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

            _soundEmitter.Play(gameObject, RandomUtils.Choose(dashSounds), enableRepeatLimiter:false);
            
            _statsTracker.Stats.Dashes++;
            
            var position = transform.position;
            Instantiate(playerDashAnimationPrefab).At(position);
            _damageCooldownTime = DamageCooldownMax;
            rigidBody.AddForce(new(_lastDirection * dashThrust, 0), ForceMode2D.Impulse);
        }

        private void OnShield(InputValue inputValue)
        {
            return; // disabled for demo
            if (_isWeaponsLocked)
                return;
            
            if (shield.gameObject.activeInHierarchy)
                return;

            var requiredEnergy = 20;

            if (!TryReduceEnergy(requiredEnergy))
                return;
            
            _statsTracker.Stats.Shields++;
            
            shield.gameObject.SetActive(true);
        }

        private void OnMissile(InputValue inputValue)
        {
            return; // disabled for demo
            
            if (_isWeaponsLocked)
                return;
            
            var requiredEnergy = 30;

            if (!TryReduceEnergy(requiredEnergy))
                return;

            _statsTracker.Stats.Missiles++;
            Instantiate(playerMissilePrefab).At(transform.position);
        }

        private void OnMine(InputValue inputValue)
        {
            return; // disabled for demo
            
            if (_isWeaponsLocked)
                return;
            
            var requiredEnergy = 30;

            if (!TryReduceEnergy(requiredEnergy))
                return;

            _statsTracker.Stats.Mines++;
            
            Instantiate(playerMinePrefab).At(transform.position);
        }

        private void OnReload(InputValue inputValue)
        {
            if (_isWeaponsLocked)
                return;
            
            if (_remainingBullets == GameplayConstants.MagSize)
                return;
            
            if (_reloadTimeRemaining > 0)
                return;
            
            // this just triggers an animation that hides the bullets
            OnManualReload?.Invoke();
            
            // this begins the active reload
            Reload();
        }

        private void OnForcefield(InputValue inputValue)
        {
            return; // disabled for demo
            if (_isWeaponsLocked)
                return;
            
            var requiredEnergy = 50;
            
            if (!TryReduceEnergy(requiredEnergy))
                return;
            
            _statsTracker.Stats.ForceFields++;
            
            var position = transform.position;
            var adjustedPosition = new Vector2(position.x, position.y + 0.75f);
            Instantiate(forcefieldPrefab).At(adjustedPosition);
        }

        
        
        private void OnMenuEnter(InputValue inputValue)
        {
            if (dialogueTyper is not null && dialogueTyper.isActiveAndEnabled)
            {
                dialogueTyper.UserCycleDialogue();
                return;
            }
            
            onUserInputEnter?.Invoke();
            return; // temporary hack
            
            // update: do i need this? lol
            
            // the logic below should maybe one day be consolidated into the event handling pattern
            // i will probably never do that.
            
            // if (dialogueTyper is null || !dialogueTyper.isActiveAndEnabled)
            //     return;
            //
            // dialogueTyper.UserCycleDialogue();

        }

        #endregion
        
    }
}