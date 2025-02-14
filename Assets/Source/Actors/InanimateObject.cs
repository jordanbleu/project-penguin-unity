using Cinemachine;
using Source.Extensions;
using Source.Graphics;
using Source.Interfaces;
using Source.UI;
using Source.Weapons;
using UnityEngine;

namespace Source.Actors
{
    /// <summary>
    /// An object that cannot be destroyed, and causes heavy damage on collision with the player
    /// </summary>
    public class InanimateObject : MonoBehaviour, IAttackResponder, ICollideWithPlayerResponder
    {
        [SerializeField]
        private CinemachineImpulseSource impulseSource;

        [SerializeField]
        private GameObject particles;

        [SerializeField]
        private int damageToPlayerOnCollide = 50;
        
        public void AttackedByBullet(GameObject bullet)
        {
            bullet.GetComponent<Bullet>().Ricochet();
        }

        public void AttackedByLaser(GameObject laser)
        {
        }

        public void HitByMissileExplosion(GameObject explosion)
        {
        }

        public void HitByMineExplosion(GameObject explosion)
        {
        }

        public void OnDeath()
        {
        }

        public void CollideWithPlayer(Player playerComponent)
        {
            playerComponent.TakeDamage(damageToPlayerOnCollide);
            impulseSource.GenerateImpulse();
            Instantiate(particles).At(transform.position);
            Destroy(gameObject);
        }
    }
}