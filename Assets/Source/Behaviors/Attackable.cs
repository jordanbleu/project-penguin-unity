using System;
using Source.Actors;
using Source.Constants;
using Source.Data;
using Source.GameData;
using Source.Interfaces;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

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

        [SerializeField]
        private UnityEvent onDeath = new();
        
        private IAttackResponder _responder;

        private int _maxHealth;

        private float _multiplierTimer = MultiplierTimer;

        private StatsTracker _statsTracker;
        private Player _player;
        public float ScoreMultiplier { get; private set; } = 0f;

        private void Start()
        {
            _maxHealth = health;
            _responder = GetComponent<IAttackResponder>();

            _player = GameObject.FindWithTag(Tags.Player).GetComponent<Player>()
                            ?? throw new UnityException("Missing player in scene");
            
            _statsTracker = GameObject.FindWithTag(Tags.StatsTracker).GetComponent<StatsTracker>()
                ?? throw new UnityException("Missing StatsTracker in scene");
        }

        private void Update()
        {
            if (_multiplierTimer > 0)
                _multiplierTimer -= Time.deltaTime;
        }

        public bool IsAlive() => isActiveAndEnabled && health > 0;

        public void Damage(int baseDamage)
        {
            if (!isActiveAndEnabled)
                return;
            
            health -= baseDamage;

            _statsTracker.Stats.DamageDealt+=baseDamage;
            
            if (!_triggeredDeathEvent && health <= 0)
            {
                _triggeredDeathEvent = true;
                onDeath?.Invoke();
                _responder?.OnDeath();
                // lock in the score multiplier
                ScoreMultiplier = DetermineScoreMultiplier();
                AddScore();
            }
        }

        private void AddScore()
        {
            var total = (baseScoreValue * ScoreMultiplier);
            _statsTracker.Stats.BaseScore+=(int)total;
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
            var currentCombo = _player.CurrentCombo;
            multiplier += (2 * StatsCalculator.CalculateComboMultiplier(currentCombo));
            
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