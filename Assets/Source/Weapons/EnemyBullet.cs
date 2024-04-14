using Source.Actors;
using Source.Interfaces;
using UnityEngine;

namespace Source.Weapons
{
    /// <summary>
    /// Used for a standard enemy bullet that triggers a destroy trigger on impact.
    /// </summary>
    [RequireComponent(typeof(Bullet))]
    public class EnemyBullet : MonoBehaviour, IEnemyProjectile
    {
        [SerializeField]
        private Bullet bullet;
        public void HitPlayer(Player playerComponent)
        {
            playerComponent.TakeDamage(5);
            
            if (playerComponent.ShieldProtectionEnabled)
            {
                // if we hit the shield, don't show the bullet hit animation
                Destroy(gameObject);
                return;
            }

            bullet.HitSomething();
        }

    }
}