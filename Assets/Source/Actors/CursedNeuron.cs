using System;
using Cinemachine;
using Source.Audio;
using Source.Behaviors;
using Source.Constants;
using Source.Data;
using Source.Extensions;
using Source.GameData;
using Source.Interfaces;
using Source.Mathematics;
using Source.UI;
using Source.Weapons;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;
namespace Source.Actors
{
    /// <summary>
    /// Boss Battle #1.
    /// </summary>
    public class CursedNeuron : MonoBehaviour, IAttackResponder
    {
        [SerializeField]
        [Tooltip("Tweak during gamplay for testing :) ")]
        private int damagePerHit = 2;
        
        [SerializeField]
        private GameObject damageEffectPrefab;
        
        [SerializeField]
        private CursedNeuronNode[] nodes;
        
        [SerializeField]
        private GameObject shield;

        [SerializeField]
        private CinemachineImpulseSource impulseSource;

        [SerializeField]
        private GameObject debrisExplosionPrefab;

        [SerializeField]
        private GameObject dialogue1;

        [SerializeField]
        private GameObject dialogue2;

        [SerializeField]
        private GameObject deathDialogue;

        [SerializeField]
        private GameObject wound1;
        
        [SerializeField]
        private GameObject wound2;

        [SerializeField]
        private GameObject wound3;

        [SerializeField]
        private GameObject wound4;

        [SerializeField]
        private ShowHide[] hudItems;

        [SerializeField]
        private GameObject enemyWave1;
        private bool wave1Spawned = false;
        
        [SerializeField]
        private GameObject enemyWave2;
        private bool wave2Spawned = false;
        
        [SerializeField]
        private GameObject enemyWave3;
        private bool wave3Spawned = false;

        [SerializeField]
        private UnityEvent onBeginFinalDialogue = new();
        
        [SerializeField]
        private AudioClip[] bodyHitSounds;
        
        [SerializeField]
        private AudioClip[] shieldHitSounds;
        
        [SerializeField]
        private AudioClip scream;
        
        private Animator _animator;
        
        private bool _isShieldActive = true;
        private Attackable _attackable;
        private State _state = State.Intro; 

        /// <summary> If true, will move around occasionally </summary>
        private bool _enableMovement = false;

        private Range<float> _movementTimerSeconds = new(2, 5);
        private float _movementTimeRemaining = 0f;
        private Range<float> _movementXRange = new(-9f, 9f);
        private Range<float> _movementYRange = new(-1f, 11);
        private Vector2 _movementDestination = Vector2.zero;

        private bool _enableLaser = false;
        private Range<float> _laserAttackWaitSecondsRange = new(4, 10);
        private float _laserAttackWaitTimeRemaining = 0f;
        private static readonly int FireLaserAnimatorParameter = Animator.StringToHash("fire-laser");

        private bool phase1DialogueTriggered = false;
        private bool phase2DialogueTriggered = false;
        
        // stuff for the dying sequence
        private float _explosionTimer = 0f;
        private float _explosionWaitSeconds = 0.25f;
        [SerializeField]
        private GameObject _explosionPrefab;

        [SerializeField]
        private UnityEvent onFullyDead = new();
        
        private SoundEffectEmitter _soundEffectEmitter;
        
        [SerializeField]
        private AudioClip _dyingMusic;
        
        [SerializeField]
        private AudioClip _deathDramaImpactSound;
        
        [SerializeField]
        private AudioClip _laserSound;
        
        
        private MusicBox _musicBox;
        
        private StatsTracker _statsTracker;
        
        private Player _player;
        private void Start()
        {
            _musicBox = GameObject.FindWithTag(Tags.MusicBox)?.GetComponent<MusicBox>()
                        ?? throw new UnityException("No music box");
            
            _soundEffectEmitter = GameObject.FindWithTag(Tags.SoundEffectEmitter)?.GetComponent<SoundEffectEmitter>()
                ??  throw new UnityException("No sound effect emitter");
            
            _attackable = GetComponent<Attackable>();
            _animator = GetComponent<Animator>();
            _player = GameObject.FindWithTag(Tags.Player).GetComponent<Player>();
            
            _statsTracker = GameObject.FindWithTag(Tags.StatsTracker)?.GetComponent<StatsTracker>()
                            ?? throw new UnityException("No StatsTracker found in scene");
        }

        private float _noAttackSeconds = 3f;
        
        private void Update()
        {
            _isShieldActive = UpdateShieldStatus();
            
            switch (_state)
            {
                case State.Intro:
                case State.Dialogue:
                    MoveTowardsDefaultPosition();
                    break;
                case State.Battle:
                    BattleUpdate();
                    break;
                case State.Dying:
                    MoveTowardsDefaultPosition();
                    DoSporadicExplosions();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Triggered after the death dialogue is done by the dialogue itself.
        /// This will probably just be a pass through to the director.
        /// </summary>
        public void IsFullyDead()
            => onFullyDead?.Invoke();

        private void DoSporadicExplosions()
        {
            _explosionTimer -= Time.deltaTime;
            
            if (_explosionTimer > 0)
                return;

            _explosionTimer = _explosionWaitSeconds;
            var pos = transform.position;
            Instantiate(_explosionPrefab).At(pos.x + Random.Range(-2, 2), pos.y + Random.Range(-2, 2));
            impulseSource.GenerateImpulse(2);
        }
        
        private void ResetNoAttackTimer()
        {
            _noAttackSeconds = 3f;

            foreach (var node in nodes)
            {
                node.ResetNoAttackTimer();
            }
        }

        private void BattleUpdate()
        {
            if (_noAttackSeconds > 0)
            {
                _noAttackSeconds -= Time.deltaTime;
                return;
            }

            if (_enableMovement)
                MoveTowardsDestination();

            if (_enableLaser)
                HandleLaserAttacks();
        }

        private void HandleLaserAttacks()
        {
            var deltaTime = Time.deltaTime;
            _laserAttackWaitTimeRemaining -= deltaTime;

            if (_laserAttackWaitTimeRemaining > 0)
                return;

            _laserAttackWaitTimeRemaining = _laserAttackWaitSecondsRange.ChooseRandom();
            
            // fire laser
            _animator.SetTrigger(FireLaserAnimatorParameter);
            
            _soundEffectEmitter.Play(gameObject, _laserSound);
        }

        private void MoveTowardsDestination()
        {
            var deltaTime = Time.deltaTime;
            
            // move position towards _movementDestination
            var pos = transform.position;
            var step = 4f * deltaTime;
            transform.localPosition = Vector2.MoveTowards(pos, _movementDestination, step);

            // if movement time remaining is zero pick a new location to move to 
            _movementTimeRemaining -= deltaTime;

            if (_movementTimeRemaining > 0)
                return;

            _movementTimeRemaining = _movementTimerSeconds.ChooseRandom();
            var x = _movementXRange.ChooseRandom();
            var y = _movementYRange.ChooseRandom();
            _movementDestination = new Vector2(x, y);
        }

        private void MoveTowardsDefaultPosition()
        {
            var pos = transform.position;
            // move y position towards 7 using MoveTowards
            var step = 4f * Time.deltaTime;
            transform.position = Vector2.MoveTowards(pos, new Vector2(0, 7f), step);
        }

        // only needed for testing
        public void MoveInstantlyToDefaultPosition()
        {
            transform.position = new Vector2(0, 7f);
        }

        private bool UpdateShieldStatus()
        {
            var shieldActive = false;
            
            foreach (var node in nodes)
            {
                if (node.IsEnabled())
                {
                    shieldActive = true;
                    break;
                }
            }

            shield.SetActive(shieldActive);
            return shieldActive;
        }


        public void AttackedByBullet(GameObject bullet)
        {
            var bulletComp = bullet.GetComponent<Bullet>();
            if (_isShieldActive)
            {
                _soundEffectEmitter.PlayRandom(gameObject, shieldHitSounds);
                bulletComp.Ricochet();
                _player.ResetCombo();
                return;
            }

            var pos = transform.position;
            _soundEffectEmitter.PlayRandom(gameObject, bodyHitSounds);
            Instantiate(damageEffectPrefab).At(pos.x + Random.Range(-1, 1), pos.y + Random.Range(-1, 1));
            _attackable.Damage(damagePerHit);
            bulletComp.HitSomething();
            impulseSource.GenerateImpulse();
            
            // explode every 10 hit points
            if (_attackable.Health % 10 == 0)
                Instantiate(debrisExplosionPrefab).At(pos.x, pos.y);
            
            // show wounds maybe 
            var hp = _attackable.Health;

            if (hp < 80)
                wound1.SetActive(true);
            
            if (hp < 60)
                wound2.SetActive(true);
            if (hp < 40)
                wound3.SetActive(true);
            if (hp < 30)
                wound4.SetActive(true);
            
            _statsTracker.Stats.BulletsHit++;
            UpdateState();
        }

        public void BeginBattle()
        {
            var health = _attackable.Health;
            
            // after phase 1 dialogue spawn first wave
            if (!wave1Spawned && health <= 75)
            {
                enemyWave1.SetActive(true);
            }
            
            // after phase 2 dialogue spawn first wave
            if (!wave2Spawned && health <= 50)
            {
                enemyWave2.SetActive(true);
            }

            ResetNoAttackTimer();
            
            foreach (var node in nodes)
            {
                node.EnableShooting = true;
            }

            _state = State.Battle;
            
            _player.SetDialogueMode(false);
            ShowHud();
        }

        private void KillAllEnemies(GameObject wave)
        {
            foreach (var attackable in wave.GetComponentsInChildren<Attackable>())
            {
                attackable?.Die();
            }
        }

        private void UpdateState()
        {
            var hp = _attackable.Health;
            
            if (hp <= 0)
            {
                
                // abruptly switch to the dying music 
                _musicBox.PlayImmediate(_dyingMusic, loop: true);
                _soundEffectEmitter.Play(gameObject, _deathDramaImpactSound);
                
                _state = State.Dying;
                _enableLaser = false;
                _enableMovement = false;
                
                foreach (var node in nodes)
                {
                    node.gameObject.SetActive(false);
                }

                KillAllEnemies(enemyWave1);
                KillAllEnemies(enemyWave2);
                KillAllEnemies(enemyWave3);

                _player.SetDialogueMode(true);
                onBeginFinalDialogue?.Invoke();
                deathDialogue.SetActive(true);
                
                return;
            }
            
            if (!wave3Spawned && hp <= 30)
            {
                _soundEffectEmitter.Play(gameObject, scream);
                enemyWave3.SetActive(true);
                wave3Spawned = true;
            }
            
            // enable laser 
            if (!phase2DialogueTriggered && hp <= 50)
            {
                // make sure we don't die during the dialogue lol
                KillAllEnemies(enemyWave1);
                
                // disable node shooting (will be re-enabled when dialogue completes)
                foreach (var node in nodes)
                {
                    node.EnableShooting = false;
                }
                _enableLaser = true;
                _state = State.Dialogue;
                dialogue2.SetActive(true);
                _player.SetDialogueMode(true);
                HideHud();
                phase2DialogueTriggered = true;
                ResetNoAttackTimer();
                return;
            }

            if (!phase1DialogueTriggered && hp <= 75)
            {
                // disable node shooting (will be re-enabled when dialogue completes)
                foreach (var node in nodes)
                {
                    node.EnableShooting = false;
                }
                _enableMovement = true;
                _state = State.Dialogue;
                dialogue1.SetActive(true);
                _player.SetDialogueMode(true);
                HideHud();
                phase1DialogueTriggered = true;
                ResetNoAttackTimer();
                return;
            }
        }

        public void AttackedByLaser(GameObject laser)
        {
            throw new NotImplementedException();
        }

        public void HitByMissileExplosion(GameObject explosion)
        {
            throw new NotImplementedException();
        }

        public void HitByMineExplosion(GameObject explosion)
        {
            throw new NotImplementedException();
        }

        public void OnDeath()
        {
            // not implemented here
        }

        private void HideHud()
        {
            foreach (var hudItem in hudItems)
            {
                hudItem.Hide();
            }
        }
        
        private void ShowHud()
        {
            foreach (var hudItem in hudItems)
            {
                hudItem.Show();
            }
        }

        private enum State
        {
            /// <summary>
            /// Neuron is moving down to the default position
            /// </summary>
            Intro,
            /// <summary>
            /// Neuron has started dialogue and is waiting for callback 
            /// </summary>
            Dialogue,
            /// <summary>
            /// Neuron is attacking the player
            /// </summary>
            Battle,
            /// <summary>
            /// Neuron is doing the dying animation
            /// </summary>
            Dying
        }
    }
}