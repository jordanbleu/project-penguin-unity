using System;
using Cinemachine;
using Source.Constants;
using Source.Extensions;
using Source.Triggers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Source.Weapons
{
    public class PlayerMissile : MonoBehaviour
    {
        private const float FollowPlayerSpeed = 4f; 
        
        [SerializeField]
        private ConstantForce2D force;

        [SerializeField]
        private GameObject explosionPrefab;

        [SerializeField]
        private CinemachineImpulseSource impulse;

        [SerializeField]
        private Rigidbody2D rigidBody;
        
        private GameObject _playerObject;

        private void Start()
        {
            _playerObject = GameObject.FindWithTag(Tags.Player);
            rigidBody.AddForce(new Vector2(0, 8), ForceMode2D.Impulse);
        }

        private void Update()
        {
            var posX = transform.position.x;
            var playerX = _playerObject.transform.position.x;

            var currentForce = force.relativeForce;

            var xForceDirection = -FollowPlayerSpeed;
            
            if (playerX >= posX)
            {
                xForceDirection = FollowPlayerSpeed;
            }
            
            force.relativeForce = new Vector2(xForceDirection, currentForce.y);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var otherObj = other.gameObject;

            if (!otherObj.TryGetComponent<PlayerBombTrigger>(out _)) 
                return;
            
            impulse.GenerateImpulse(5);
            Instantiate(explosionPrefab).At(transform.position);
            Destroy(gameObject);
        }
    }
}