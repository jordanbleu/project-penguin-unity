using System;
using Cinemachine;
using Source.Behaviors;
using Source.Interfaces;
using Source.Timers;
using Source.Weapons;
using UnityEngine;

namespace Source.Actors
{
    public class Grunt : MonoBehaviour, ICollideWithPlayerResponder, IAttackResponder
    {
        private const float CameraImpulseForce = 1f;
        private static readonly int DeathAnimatorTrigger = Animator.StringToHash("death");
        private GameObject _player;
        
        [SerializeField]
        private float burstVelocity = 5f;
        
        [SerializeField]
        private Animator animator;
        
        [SerializeField]
        private Rigidbody2D rigidBody2d;
        
        [SerializeField]
        private CinemachineImpulseSource cameraImpulseSource;
        
        [SerializeField]
        private Attackable attackable;

        private void Start()
        {
            var burstInterval = gameObject.AddComponent<IntervalEventTimer>();
            burstInterval.AddEventListener(BurstMove);
            burstInterval.PreWarm();
            _player = GameObject.FindWithTag("Player");
            
        }

        // Called by event interval timer
        private void BurstMove()
        {
            var position = transform.position;
            var playerPosition = _player.transform.position;

            var velocity = UnityEngine.Random.Range(burstVelocity / 2, burstVelocity);
            
            var xVelocity = velocity;
            
            if (position.x > playerPosition.x)
            {
                xVelocity = -xVelocity;
            }

            rigidBody2d.AddForce(new Vector2(xVelocity, -burstVelocity), ForceMode2D.Impulse);
        }

        public void AttackedByBullet(GameObject bullet)
        {
            attackable.Damage(1);
            rigidBody2d.AddForce(new Vector2(0, 5), ForceMode2D.Impulse);
            cameraImpulseSource.GenerateImpulse(CameraImpulseForce);
            bullet.GetComponent<Bullet>().HitSomething();
        }

        public void AttackedByLaser(GameObject laser)
        {
            attackable.Die();
            
            var xForce = UnityEngine.Random.Range(-7, 7);
            rigidBody2d.AddForce(new Vector2(xForce, 5), ForceMode2D.Impulse);
            
            // camera impulse not needed since the laser already does one.
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
            animator.SetTrigger(DeathAnimatorTrigger);
        }

        public void CollideWithPlayer(Player playerComponent)
        {
            playerComponent.TakeDamage(10);
        }
    }
}