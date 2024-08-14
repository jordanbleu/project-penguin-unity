using Source.Actors;
using Source.Data;
using Source.Interfaces;
using UnityEngine;

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
        private Bullet bullet;
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
            bullet.HitSomething();
            Stats.TrackBulletHit();
            attackingBullet.GetComponent<Bullet>().HitSomething();
        }


        public void AttackedByLaser(GameObject laser) => bullet.HitSomething();

        public void HitByMissileExplosion(GameObject explosion) => bullet.HitSomething();

        public void HitByMineExplosion(GameObject explosion) => bullet.HitSomething();

        public void OnDeath() => bullet.HitSomething();
    }
}