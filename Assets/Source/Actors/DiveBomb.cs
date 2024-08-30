using System;
using Cinemachine;
using Source.Audio;
using Source.Behaviors;
using Source.Constants;
using Source.Data;
using Source.Extensions;
using Source.Interfaces;
using Source.Weapons;
using UnityEngine;
using UnityEngine.Serialization;

namespace Source.Actors
{
    public class DiveBomb : MonoBehaviour, IAttackResponder, ICollideWithPlayerResponder
    {
        private const float CameraImpulseForce = 1f;

        [SerializeField] private Attackable attackable;
        [SerializeField] private float verticalMaxVelocity = 2f;
        [SerializeField] private float horizontalMaxVelocity = 2f;
        
        [SerializeField] 
        private Animator animator;
        
        [SerializeField] 
        private CinemachineImpulseSource cameraImpulseSource;

        [SerializeField] private Rigidbody2D rigidBody;

        [SerializeField]
        private AudioClip squish;
        
        private static readonly int DeathAnimatorTrigger = Animator.StringToHash("death");

        private GameObject _player;
        private SoundEffectEmitter _soundEmitter;

        private void Start()
        {
            _player = GameObject.FindWithTag(Tags.Player);
            _soundEmitter = GameObject.FindWithTag(Tags.SoundEffectEmitter).GetComponent<SoundEffectEmitter>();

        }

        private void Update()
        {
            var position = transform.position;
            var playerPosition = _player.transform.position;

            var xVelocity = 0f;

            if (position.x.IsWithin(0.5f, playerPosition.x))
            {
                rigidBody.velocity = new Vector2(0f, -verticalMaxVelocity);
                return;
            }

            if (position.x < playerPosition.x)
            {
                xVelocity = horizontalMaxVelocity;
            }
            else
            {
                xVelocity = -horizontalMaxVelocity;
            }

            rigidBody.velocity = new Vector2(xVelocity, -verticalMaxVelocity);
        }

        public void AttackedByBullet(GameObject bullet)
        {
            bullet.GetComponent<Bullet>().HitSomething();
            Stats.TrackBulletHit();
            attackable.Die();
        }

        public void AttackedByLaser(GameObject laser) =>
            attackable.Die();

        public void HitByMissileExplosion(GameObject explosion)
        {
            throw new NotImplementedException();
        }

        public void HitByMineExplosion(GameObject explosion)
        {
            throw new NotImplementedException();
        }

        public void PickRandomXPosition()
        {
            var position = transform.position;
            transform.position = new Vector2(UnityEngine.Random.Range(-17, 17), position.y);
        }

        public void OnDeath()
        {
            _soundEmitter.Play(gameObject, squish);
            cameraImpulseSource.GenerateImpulse(CameraImpulseForce);
            animator.SetTrigger(DeathAnimatorTrigger);
        }

        public void CollideWithPlayer(Player playerComponent)
        {
            playerComponent.TakeDamage(5);
            attackable.Die();
        }
    }
}