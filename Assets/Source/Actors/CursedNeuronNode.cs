using System;
using Cinemachine;
using Source.Audio;
using Source.Constants;
using Source.Data;
using Source.Extensions;
using Source.Interfaces;
using Source.Timers;
using Source.Weapons;
using UnityEngine;

namespace Source.Actors
{
    /// <summary>
    /// One of the three nodes that controls the cursed neuron's shield 
    /// </summary>
    public class CursedNeuronNode : MonoBehaviour, IAttackResponder
    {
        private const float HealTime = 10f;
        private const int MaxHits = 3;
        private const float ShootWaitSeconds = 3f;
        
        private static readonly int Damage = Animator.StringToHash("damage");
        private static readonly int IsDisabled = Animator.StringToHash("is-disabled");

        private int _hits = MaxHits;
        private Animator _animator;

        private bool _isDisabled = false;
        private float _healTimer = 0f;

        public bool EnableShooting { get; set; } = false;


        [SerializeField]
        private GameObject connector;

        [SerializeField]
        private GameObject bulletPrefab;

        [SerializeField]
        private AudioClip shootSound;
        
        [SerializeField]
        private AudioClip[] hitSounds;
        
        [SerializeField]
        private CinemachineImpulseSource impulseSource;

        private GameObject _player;

        private float _shootTimer = 0f;
        
        private float _noAttackSeconds = 3f;

        private SoundEffectEmitter _soundEffectEmitter;
        
        private void Start()
        {
            _soundEffectEmitter = GameObject.FindWithTag(Tags.SoundEffectEmitter)?.GetComponent<SoundEffectEmitter>()
                ?? throw new UnityException("No SoundEffectEmitter found in scene");
            
            _animator = GetComponent<Animator>();
            _player = GameObject.FindWithTag(Tags.Player);
        }

        private void Update()
        {
            // gives the player a second to get ready
            if (_noAttackSeconds > 0)
            {
                _noAttackSeconds -= Time.deltaTime;
                return;
            }
            
            if (!_isDisabled)
            {
                ShootAtPlayer();
                return;
            }

            _healTimer -= Time.deltaTime;

            if (_healTimer <= 0f)
            {
                _isDisabled = false;
                _animator.SetBool(IsDisabled, false);
                _hits = MaxHits;
            }

            connector.SetActive(!_isDisabled);
        }

        public void ResetNoAttackTimer()
        {
            _noAttackSeconds = 3f;
        }

        private void ShootAtPlayer()
        {
            if (!EnableShooting)
                return;
            
            var playerPosition = _player.transform.position;
            var playerY = playerPosition.y;
            var playerX = playerPosition.x;

            var position = transform.position;
            var x = position.x;
            var y = position.y;

            // cannot shoot what is above me
            if (playerY > y)
                return;

            var min = x - 0.25;
            var max = x + 0.25;

            if (_shootTimer > 0f)
            {
                _shootTimer -= Time.deltaTime;
                return;
            }

            
            if (playerX > min && playerX < max)
            {
                _soundEffectEmitter.Play(gameObject, shootSound);
                Instantiate(bulletPrefab).At(x, y - 0.5f);
                _shootTimer = ShootWaitSeconds;
            }
        }

        public void AttackedByBullet(GameObject bullet)
        {
            _soundEffectEmitter.PlayRandom(gameObject, hitSounds);
            var b = bullet.GetComponent<Bullet>();
            
            if (_isDisabled)
            {
                impulseSource.GenerateImpulse(0.25f);

                b.Ricochet();
                return;
            }

            Stats.TrackBulletHit();
            b.HitSomething();

            _hits--;

            if (_hits <= 0)
            {
                _isDisabled = true;
                _healTimer = HealTime;
                _animator.SetBool(IsDisabled, true);
                impulseSource.GenerateImpulse(5);
            }
            else
            {
                impulseSource.GenerateImpulse(1);
            }

            _animator.SetTrigger(Damage);
        }

        public void AttackedByLaser(GameObject laser)
        {
            throw new System.NotImplementedException();
        }

        public void HitByMissileExplosion(GameObject explosion)
        {
            throw new System.NotImplementedException();
        }

        public void HitByMineExplosion(GameObject explosion)
        {
            throw new System.NotImplementedException();
        }

        public void OnDeath()
        {
            throw new System.NotImplementedException();
        }
        
        public bool IsEnabled() => gameObject.activeSelf && !_isDisabled;
    }
}