using System;
using Cinemachine;
using Source.Behaviors;
using Source.Interfaces;
using Source.Weapons;
using UnityEngine;
using UnityEngine.Events;

namespace Source.Actors
{
    /// <summary>
    /// Similar to <seealso cref="SimpleEnemy"/> but even more stripped down.
    ///
    /// Only provides functionality for taking damage, dying, and optionally hurting player on collide.
    ///
    /// Requires an animator with a 'death' trigger and optionally a 'damage' trigger.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(Attackable))]
    public class SimpleAttackable : MonoBehaviour, IAttackResponder, ICollideWithPlayerResponder
    {
        [SerializeField]
        private bool triggerDamageAnimatorParameter = true;
        
        [SerializeField]
        private GameObject damageEffectPrefab;

        [SerializeField]
        private CinemachineImpulseSource impulseSource;

        [SerializeField]
        private int damageToPlayerOnCollide = 0;

        [SerializeField]
        private int damageToSelfOnCollideWithPlayer = 0;

        [SerializeField]
        private UnityEvent onBeginDeath = new();
        
        private Rigidbody2D _rigidBody;
        private Animator _animator;
        private Attackable _attackable;
        
        private static readonly int DamageAnimParameter = Animator.StringToHash("damage");
        private static readonly int DeathAnimParameter = Animator.StringToHash("death");

        private void Start()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _attackable = GetComponent<Attackable>();
        }

        private void AddDamageEffect()
        {
            if (triggerDamageAnimatorParameter)
                _animator.SetTrigger(DamageAnimParameter);

            if (damageEffectPrefab != null)
                Instantiate(damageEffectPrefab, transform);

            // for now this is required until i find a reason for it not to be
            impulseSource.GenerateImpulse();
        }

        public void AttackedByBullet(GameObject bullet)
        {
            AddDamageEffect();
            _attackable.Damage(1);

            bullet.GetComponent<Bullet>().HitSomething();
        }

        public void AttackedByLaser(GameObject laser)
        {
            AddDamageEffect();
            _attackable.Damage(3);
            
        }

        public void HitByMissileExplosion(GameObject explosion)
        {
            AddDamageEffect();
            _attackable.Damage(10);
            
        }

        public void HitByMineExplosion(GameObject explosion)
        {
            AddDamageEffect();
            _attackable.Damage(10);
            
        }

        public void OnDeath()
        {
            onBeginDeath?.Invoke();
            _animator.SetTrigger(DeathAnimParameter);
        }

        public void CollideWithPlayer(Player playerComponent)
        {
            if (damageToPlayerOnCollide <= 0)
                return;
            
            var playerTookDamage = playerComponent.TakeDamage(damageToPlayerOnCollide);

            if (playerTookDamage && damageToSelfOnCollideWithPlayer > 0)
                _attackable.Damage(damageToSelfOnCollideWithPlayer);
        }
    }
}