using Source.Interfaces;
using Source.Weapons;
using UnityEngine;

namespace Source.Behaviors
{
    public class BulletForcefield : MonoBehaviour, IAttackResponder
    {
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
    }
}