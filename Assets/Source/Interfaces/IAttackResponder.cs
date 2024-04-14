using Source.Behaviors;
using UnityEngine;

namespace Source.Interfaces
{
    /// <summary>
    /// Any object that uses a <see cref="Attackable"/> component
    /// must have another component that implements this.
    /// </summary>
    public interface IAttackResponder
    {
        void AttackedByBullet(GameObject bullet);
        void AttackedByLaser(GameObject laser);
        void HitByMissileExplosion(GameObject explosion);
        void HitByMineExplosion(GameObject explosion);
        void OnDeath();
    }
}