using Source.Interfaces;
using Source.Projectiles;
using UnityEngine;

namespace Source.Behaviors
{
    /// <summary>
    /// Used for objects that can be destroyed by the player
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(IAttackResponder))]
    public class Attackable : MonoBehaviour
    {
        private bool _triggeredDeathEvent = false;
        [SerializeField] private int health = 10;

        private IAttackResponder _responder;

        private void Start()
        {
            _responder = GetComponent<IAttackResponder>();
        }

        public void Damage(int baseDamage)
        {
            health -= baseDamage;

            if (!_triggeredDeathEvent && health <= 0)
            {
                _triggeredDeathEvent = true;
                _responder.OnDeath();
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

        private void HandleCollidedGameObject(GameObject other)
        {
            switch (other.tag)
            {
                case "player-bullet":
                    _responder.AttackedByBullet(other);
                    break;
                case "player-laser":
                    _responder.AttackedByLaser(other);
                    break;
            }
        }

        public void Die() => Damage(health);
        
        public bool WasDestroyed => _triggeredDeathEvent;
    }
    
}