using System;
using Cinemachine;
using Source.Behaviors;
using Source.Constants;
using Source.Interfaces;
using Source.Timers;
using Source.Weapons;
using UnityEngine;
using UnityEngine.Serialization;

namespace Source.Actors
{
    /// <summary>
    /// Enemy that attacks from a distance and remains within the top half of the screen or so.
    ///
    /// </summary>
    public class RangedAttacker : MonoBehaviour, IAttackResponder
    {
        // how far left / right the player can be before the enemy will move to them.
        private const float ThresholdBeforeMoving = 1f;
        
        private bool _isVulnerable = true;
        private static readonly int DeathAnimatorTrigger = Animator.StringToHash("death");
        private static readonly int DamageAnimatorTrigger = Animator.StringToHash("damage");
        
        private Vector2 _seekPosition;
        private GameObject _player;
        private IntervalEventTimer _thinkTimer;

        [SerializeField]
        private float thinkTime = 2f;
        
        [SerializeField]
        private GameObject bulletPrefab;

        [SerializeField]
        private CinemachineImpulseSource impulseSource;

        [SerializeField]
        private Attackable attackable;

        [SerializeField]
        private Animator animator;
        
        private void Start()
        {
            _player = GameObject.FindWithTag("Player");
            _thinkTimer = gameObject.AddComponent<IntervalEventTimer>();
            _thinkTimer.SetInterval(thinkTime);
            _thinkTimer.AddEventListener(AIUpdate);
        }
        
        /// <summary>
        /// Called on an interval to update the AI's state.
        /// </summary>
        private void AIUpdate()
        {
            var playerPosition = _player.transform.position;
            var myPosition = transform.position;
            
            if (Math.Abs(playerPosition.x - myPosition.x) > ThresholdBeforeMoving)
            {
                _seekPosition = playerPosition;
            }
            else
            {
                Instantiate(bulletPrefab, myPosition, Quaternion.identity);    
            }
        }
        
        private void FixedUpdate()
        {
            var myPosition= transform.position;
            transform.position = Vector2.MoveTowards(myPosition, new(_seekPosition.x, myPosition.y), 0.1f);
        }
        
        public void SetVulnerable() => 
            _isVulnerable = true;

        public void SetInvulnerable() =>
            _isVulnerable = false;
        
        
        public void AttackedByBullet(GameObject bullet)
        {
            var bulletComponent = bullet.GetComponent<Bullet>();
            
            if (!_isVulnerable)
            {
                bulletComponent.Ricochet();
                return;
            }

            impulseSource.GenerateImpulse();
            bullet.GetComponent<Bullet>().HitSomething();
            attackable.Damage(DamageValues.PlayerBulletDamage);
            animator.SetTrigger(DamageAnimatorTrigger);

        }

        public void AttackedByLaser(GameObject laser)
        {
            if (!_isVulnerable)
                return;
            
            impulseSource.GenerateImpulse();
            attackable.Damage(DamageValues.PlayerLaserDamage);
            animator.SetTrigger(DamageAnimatorTrigger);
        }

        public void HitByMissileExplosion(GameObject explosion)
        {
            if (!_isVulnerable)
                return;
            
            impulseSource.GenerateImpulse();
            attackable.Damage(DamageValues.MissileExplosionDamage);
            animator.SetTrigger(DamageAnimatorTrigger);
        }

        public void HitByMineExplosion(GameObject explosion)
        {
            if (!_isVulnerable)
                return;
            
            impulseSource.GenerateImpulse();
            attackable.Damage(DamageValues.MineExplosionDamage);
            animator.SetTrigger(DamageAnimatorTrigger);
        }

        public void OnDeath()
        {
            animator.SetTrigger(DeathAnimatorTrigger);
        }
    }
}