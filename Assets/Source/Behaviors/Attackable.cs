using System;
using Source.Projectiles;
using UnityEngine;
using UnityEngine.Events;

namespace Source.Behaviors
{
    /// <summary>
    /// Used for objects that can be destroyed by the player
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Attackable : MonoBehaviour
    {
        private bool triggeredDeathEvent = false;
        [SerializeField] private int health = 10;
        [SerializeField] private UnityEvent onHitByPlayerBullet = new();
        [SerializeField] private UnityEvent onHitByPlayerLaser = new();
        [SerializeField] private UnityEvent onDead = new();

        public void Damage(int baseDamage)
        {
            health -= baseDamage;

            if (!triggeredDeathEvent && health <= 0)
            {
                triggeredDeathEvent = true;
                onDead?.Invoke();
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            HandleCollidedGameObject(other.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            HandleCollidedGameObject(other.gameObject);
        }

        private void HandleCollidedGameObject(GameObject collidingObject)
        {
            var tag = collidingObject.tag;
            
            switch (tag)
            {
                case "player-bullet":
                    onHitByPlayerBullet?.Invoke();
                    collidingObject.GetComponent<PlayerBullet>().HitSomething();
                    break;
                case "player-laser":
                    onHitByPlayerLaser?.Invoke();
                    break;
            }
            
        }
    }
}