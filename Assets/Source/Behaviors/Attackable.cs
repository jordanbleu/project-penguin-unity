using System;
using Source.Data;
using Source.Interfaces;
using UnityEngine;

namespace Source.Behaviors
{
    /// <summary>
    /// Used for objects that can be destroyed by the player
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(IAttackResponder))]
    public class Attackable : MonoBehaviour
    {
        private const float MultiplierTimer = 10f;
        
        private bool _triggeredDeathEvent = false;
        [SerializeField]
        private int health = 10;

        [SerializeField]
        private int baseScoreValue = 0;
        
        private IAttackResponder _responder;

        private int _maxHealth;

        private float _multiplierTimer = MultiplierTimer;

        public float ScoreMultiplier { get; private set; } = 0f;

        private void Start()
        {
            _maxHealth = health;
            _responder = GetComponent<IAttackResponder>();
        }

        private void Update()
        {
            if (_multiplierTimer > 0)
                _multiplierTimer -= Time.deltaTime;
        }

        public void Damage(int baseDamage)
        {
            health -= baseDamage;

            Stats.TrackDamageDealt(baseDamage);
            
            if (!_triggeredDeathEvent && health <= 0)
            {
                _triggeredDeathEvent = true;
                _responder.OnDeath();
                // lock in the score multiplier
                ScoreMultiplier = DetermineScoreMultiplier();
                AddScore();
            }
        }

        private void AddScore()
        {
            var total = (baseScoreValue * ScoreMultiplier);
            Stats.AddToScore((int)total);
        }

        /// <summary>
        /// Determines the score multiplier for this kill based on how long the thing was alive.
        /// </summary>
        /// <returns></returns>
        private float DetermineScoreMultiplier()
        {
            var multiplier = 1f;
            
            // add speed multiplier 
            if (_multiplierTimer > 7)
            {
                multiplier = 2;
            }

            if (_multiplierTimer > 6)
            {
                multiplier = 1.66f;
            }

            if (_multiplierTimer > 5)
            {
                multiplier = 1.44f;
            }

            if (_multiplierTimer > 2)
            {
                multiplier = 1.25f;
            }
            
            // add combo multiplier, adds a max of 2 to the multiplier
            var currentCombo = Stats.Current?.BulletsFiredCombo ?? 0;
            multiplier += Math.Min(2, (float)currentCombo / 100);
            
            return multiplier;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            HandleCollidedGameObject(other.gameObject);
        }

        // private void OnTriggerEnter2D(Collider2D other)
        // {
        //     //HandleCollidedGameObject(other.gameObject);
        // }

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

        public int Health => health;

        public int MaxHealth => _maxHealth;
    }
    
}