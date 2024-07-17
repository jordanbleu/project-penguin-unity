using System;
using Cinemachine;
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
        private const float HealTime = 7f;
        private const int MaxHits = 3;
        
        private static readonly int Damage = Animator.StringToHash("damage");
        private static readonly int IsDisabled = Animator.StringToHash("is-disabled");

        private int _hits = MaxHits;
        private Animator _animator;

        private bool _isDisabled = false;
        private float _healTimer = 0f;

        [SerializeField]
        private GameObject connector;

        [SerializeField]
        private CinemachineImpulseSource impulseSource;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (!_isDisabled) 
                return;

            _healTimer -= Time.deltaTime;

            if (_healTimer <= 0f)
            {
                _isDisabled = false;
                _animator.SetBool(IsDisabled, false);
                _hits = MaxHits;
            }

            connector.SetActive(!_isDisabled);
        }

        public void AttackedByBullet(GameObject bullet)
        {
            var b = bullet.GetComponent<Bullet>();
            
            if (_isDisabled)
            {
                impulseSource.GenerateImpulse(0.25f);

                b.Ricochet();
                return;
            }

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