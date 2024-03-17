using Source.Actors;
using Source.Interfaces;
using UnityEngine;

namespace Source.Projectiles
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
            bullet.HitSomething();
        }
    }
}