using System;
using Cinemachine;
using Source.Behaviors;
using Source.Extensions;
using Source.Interfaces;
using Source.Utilities;
using Source.Weapons;
using UnityEngine;

namespace Source.Actors
{
    
    /// <summary>
    /// Basic catchall behavior for many enemy types.
    /// </summary>
    public class SimpleEnemy : MonoBehaviour, IAttackResponder, ICollideWithPlayerResponder
    {
        private const float ForceAgainstPlayer = 20f;
        private const float Acceleration = 0.5f;
        
        [SerializeField] 
        private Animator animator;
        
        [SerializeField]
        [Tooltip("If true, will use the damage effect prefab when hit by a bullet. " +
                 " If false, will call the animator 'damage' trigger." +
                 "To do neither, set to false and leave the prefab empty.")]
        private bool useDamageEffectPrefab = true;
        
        [SerializeField] 
        private GameObject damageEffectPrefab;
        
        [SerializeField] 
        private CinemachineImpulseSource impulseSource;
       
        [SerializeField] 
        private Rigidbody2D rigidBody;
        
        [SerializeField] 
        private Attackable attackable;

        [SerializeField]
        [Tooltip("How fast downward the enemy moves")]
        private float speed = -2f;
        
        [SerializeField] 
        [Tooltip("When colliding with the player will apply a strong force against them")]
        private bool repelPlayerOnCollide = true;
        
        [SerializeField]
        [Tooltip("Damage to apply to the player on collide")]
        private int playerCollisionDamage = 0;
        
        
        private bool _isVulnerable = true;
        private static readonly int DeathAnimatorTrigger = Animator.StringToHash("death");
        private static readonly int DamageAnimatorTrigger = Animator.StringToHash("damage");

        
        public void SetVulnerable() => 
            _isVulnerable = true;

        public void SetInvulnerable() =>
            _isVulnerable = false;

        private void Start()
        {
            rigidBody.velocity = new Vector2(0, speed);
        }

        private void FixedUpdate()
        {
            // if speed is knocked off course, accelerate back to speed.
            var currentVelocity = rigidBody.velocity;
            
            if (currentVelocity.y.IsWithin(Acceleration, speed) && currentVelocity.x.IsWithin(Acceleration, 0)) return;

            var nextYValue = currentVelocity.y.Stabilize(Acceleration, speed);
            var nextXValue = currentVelocity.x.Stabilize(Acceleration, 0);

            rigidBody.velocity = new Vector2(nextXValue, nextYValue);
        }

        public void AttackedByBullet(GameObject bullet)
        {
            var bulletComponent = bullet.GetComponent<Bullet>();
            
            if (_isVulnerable)
            {
                impulseSource.GenerateImpulse();
                ApplyDamageEffect();
                bulletComponent.HitSomething();
                attackable.Damage(1);
                return;
            }
            
            // enemy is shielded
            bulletComponent.Ricochet();
        }

        public void AttackedByLaser(GameObject laser)
        {
            ApplyDamageEffect();
            attackable.Damage(3);
        }

        public void HitByMissileExplosion(GameObject explosion)
        {
            ApplyDamageEffect();
            attackable.Damage(10);
        }

        public void HitByMineExplosion(GameObject explosion)
        {
            ApplyDamageEffect();
            attackable.Damage(5);
            
            var xForce = UnityEngine.Random.Range(-8f, 8f);
            var yForce = UnityEngine.Random.Range(-8f, -5f);

            rigidBody.AddForce(new(xForce, yForce), ForceMode2D.Impulse);
        }

        public void OnDeath()
        {
            rigidBody.velocity = Vector2.zero;
            animator.SetTrigger(DeathAnimatorTrigger);
        }

        public void CollideWithPlayer(Player playerComponent)
        {
            if (repelPlayerOnCollide)
            {
                var xVelocity = RandomUtils.Choose(-ForceAgainstPlayer, ForceAgainstPlayer);
                playerComponent.AddForceToPlayer(new Vector2(xVelocity, -10));
                impulseSource.GenerateImpulseWithForce(3f);
            }

            if (playerCollisionDamage > 0f)
            {
                playerComponent.TakeDamage(playerCollisionDamage);
            }
        }
        
        public void PickRandomXPosition()
        {
            var position = transform.position;
            transform.position = new Vector2(UnityEngine.Random.Range(-17, 17), position.y);
        }

        private void ApplyDamageEffect()
        {
            if (useDamageEffectPrefab && damageEffectPrefab is null)
                return;

            if (!useDamageEffectPrefab)
            {
                animator.SetTrigger(DamageAnimatorTrigger);
                return;
            }
            
            Instantiate(damageEffectPrefab, gameObject.transform);

        }

    }
}