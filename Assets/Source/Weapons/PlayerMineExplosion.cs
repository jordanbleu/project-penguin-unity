using Source.Interfaces;
using UnityEngine;

namespace Source.Weapons
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerMineExplosion : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.TryGetComponent<IAttackResponder>(out var attackResponder))
                return;

            attackResponder.HitByMineExplosion(gameObject);
        }
    }
}