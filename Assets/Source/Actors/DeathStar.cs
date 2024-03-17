using System;
using Cinemachine;
using Source.Behaviors;
using Source.Extensions;
using Source.Interfaces;
using Source.Projectiles;
using Source.Utilities;
using UnityEngine;

namespace Source.Actors
{
    
    /// <summary>
    /// Animation opens and closes, enemy is only vulnerable
    /// for certain frames.
    ///
    /// Bullets will ricochet
    /// </summary>
    public class DeathStar : MonoBehaviour, IAttackResponder, ICollideWithPlayerResponder
    {
        private const float ForceAgainstPlayer = 20f;
        private const float Speed = 2f;
        private const float Acceleration = 0.1f;
        
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject damageEffectPrefab;
        [SerializeField] private CinemachineImpulseSource impulseSource;
        [SerializeField] private Rigidbody2D rigidBody;
        [SerializeField] private Attackable attackable;
        
        private bool _isVulnerable = false;
        private static readonly int DeathAnimatorTrigger = Animator.StringToHash("death");

        public void SetVulnerable() => 
            _isVulnerable = true;

        public void SetInvulnerable() =>
            _isVulnerable = false;

        private void Start()
        {
            rigidBody.velocity = new Vector2(0, -2);
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
            var bulletComponent = bullet.GetComponent<PlayerBullet>();
            
            if (_isVulnerable)
            {
                impulseSource.GenerateImpulse();
                var damageAnim = Instantiate(damageEffectPrefab, gameObject.transform);
                bulletComponent.HitSomething();
                attackable.Damage(1);
                return;
            }

            var xVel = UnityEngine.Random.Range(-3, 3);
            // else ricochet
            bulletComponent.Ricochet();


        }

        public void AttackedByLaser(GameObject laser)
        {
        }

        public void OnDeath()
        {
            rigidBody.velocity = Vector2.zero;
            animator.SetTrigger(DeathAnimatorTrigger);
        }

        public void CollideWithPlayer(Player playerComponent)
        {
            var xVelocity = RandomUtils.Choose(-ForceAgainstPlayer, ForceAgainstPlayer);
            playerComponent.AddForceToPlayer(new Vector2(xVelocity, -10));
            impulseSource.GenerateImpulseWithForce(3f);
        }
    }
}