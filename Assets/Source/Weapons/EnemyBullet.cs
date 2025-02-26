using System;
using Source.Actors;
using Source.Constants;
using Source.Data;
using Source.GameData;
using Source.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Source.Weapons
{
    /// <summary>
    /// Used for a standard enemy bullet that triggers a destroy trigger on impact.
    /// </summary>
    [RequireComponent(typeof(Bullet))]
    public class EnemyBullet : MonoBehaviour, IEnemyProjectile, IAttackResponder
    {
        [SerializeField]
        private int damage = 10;
        
        [SerializeField]
        private UnityEvent onCollideWithPlayerBullet = new();
        
        [SerializeField]
        private Bullet bullet;
        
        private StatsTracker _statsTracker;

        private void Start()
        {
            _statsTracker = GameObject.FindWithTag(Tags.StatsTracker).GetComponent<StatsTracker>()
                            ?? throw new UnityException("Missing StatsTracker in scene");
        }

        public void HitPlayer(Player playerComponent)
        {
            playerComponent.TakeDamage(damage);
            
            if (playerComponent.ShieldProtectionEnabled)
            {
                // if we hit the shield, don't show the bullet hit animation
                Destroy(gameObject);
                return;
            }

            bullet.HitSomething();
        }

        public void AttackedByBullet(GameObject attackingBullet)
        {
            onCollideWithPlayerBullet?.Invoke();
            bullet.HitSomething();
            _statsTracker.Stats.BulletsHit++;
            
            attackingBullet.GetComponent<Bullet>().HitSomething();
        }


        public void AttackedByLaser(GameObject laser) => bullet.HitSomething();

        public void HitByMissileExplosion(GameObject explosion) => bullet.HitSomething();

        public void HitByMineExplosion(GameObject explosion) => bullet.HitSomething();

        public void OnDeath() => bullet.HitSomething();
    }
}