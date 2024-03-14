using System;
using Source.Timers;
using UnityEngine;

namespace Source.Actors
{
    public class Grunt : MonoBehaviour
    {
        [SerializeField] private float burstVelocity = 10f;
        
        [SerializeField] Animator animator;
        private GameObject player;

        [SerializeField] private Rigidbody2D rigidBody2d;
        
        private void Start()
        {
            var burstInterval = gameObject.AddComponent<IntervalEventTimer>();
            burstInterval.AddEventListener(BurstMove);
            player = GameObject.FindWithTag("Player");
        }

        private void BurstMove()
        {
            var position = transform.position;
            var playerPosition = player.transform.position;

            var velocity = UnityEngine.Random.Range(burstVelocity / 2, burstVelocity);
            
            var xVelocity = velocity;
            
            if (position.x > playerPosition.x)
            {
                xVelocity = -xVelocity;
            }

            rigidBody2d.AddForce(new Vector2(xVelocity, -burstVelocity), ForceMode2D.Impulse);
        }
        
        public void TriggerDeathAnimation() =>
            animator.SetTrigger("death");

    }
}