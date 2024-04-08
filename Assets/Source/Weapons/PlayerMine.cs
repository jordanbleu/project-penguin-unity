using System;
using Cinemachine;
using Source.Extensions;
using Source.Timers;
using UnityEngine;

namespace Source.Weapons
{
    public class PlayerMine : MonoBehaviour
    {
        [SerializeField]
        private Animator animator;

        [SerializeField]
        private IntervalEventTimer intervalTimer;

        [SerializeField]
        private GameObject explosionPrefab;

        [SerializeField]
        private CinemachineImpulseSource impulse;

        [SerializeField]
        private Rigidbody2D rigidBody;
        
        private static readonly int TimeRemainingParameter = Animator.StringToHash("time");

        private void Start()
        {
            // mine gets lobbed from the player
            rigidBody.AddForce(new(0, 12), ForceMode2D.Impulse);
        }

        public void Explode()
        {
            Instantiate(explosionPrefab).At(transform.position);
            impulse.GenerateImpulse(5);
            Destroy(gameObject);
        }

        private void Update()
        {
            animator.SetFloat(TimeRemainingParameter, intervalTimer.CurrentTime);
        }
    }
}