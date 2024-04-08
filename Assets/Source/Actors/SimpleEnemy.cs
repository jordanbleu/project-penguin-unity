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
        private const float Speed = 2f;
        private const float Acceleration = 0.1f;
        
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject damageEffectPrefab;
        [SerializeField] private CinemachineImpulseSource impulseSource;
        [SerializeField] private Rigidbody2D rigidBody;
        [SerializeField] private Attackable attackable;

        [SerializeField]
        [Tooltip("How fast downward the enemy moves")]
        private float speed = 2f;
        
        [SerializeField] 
        [Tooltip("When colliding with the player will apply a strong force against them")]
        private bool repelPlayerOnCollide = true;
        
        [SerializeField]
        [Tooltip("Damage to apply to the player on collide")]
        private int playerCollisionDamage = 0;
        
        private bool _isVulnerable = true;
        private static readonly int DeathAnimatorTrigger = Animator.StringToHash("death");

        public void SetVulnerable() => 
            _isVulnerable = true;

        public void SetInvulnerable() =>
            _isVulnerable = false;

        private void Start()
        {
            rigidBody.velocity = new Vector2(0, -Speed);
        }

        private void FixedUpdate()
        {
            // if speed is knocked off course, accelerate back to speed.
            var currentVelocity = rigidBody.velocity;
            
            if (currentVelocity.y.IsWithin(Acceleration, Speed)) return;

            var nextYValue = currentVelocity.y.Stabilize(Acceleration, -Speed);
            var nextXValue = currentVelocity.x.Stabilize(Acceleration, 0);

            rigidBody.velocity = new Vector2(nextXValue, nextYValue);

        }

        public void AttackedByBullet(GameObject bullet)
        {
            var bulletComponent = bullet.GetComponent<Bullet>();
            
            if (_isVulnerable)
            {
                impulseSource.GenerateImpulse();
                var damageAnim = Instantiate(damageEffectPrefab, gameObject.transform);
                bulletComponent.HitSomething();
                attackable.Damage(1);
                return;
            }
            
            // enemy is shielded
            bulletComponent.Ricochet();
        }

        public void AttackedByLaser(GameObject laser)
        {
        }

        public void HitByMissileExplosion(GameObject explosion)
        {
            Instantiate(damageEffectPrefab, gameObject.transform);
            attackable.Damage(10);
        }

        public void HitByMineExplosion(GameObject explosion)
        {
            Instantiate(damageEffectPrefab, gameObject.transform);
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

    }
}