using System;
using System.Linq;
using Source.Audio;
using Source.Constants;
using Source.Interfaces;
using Source.Weapons;
using UnityEngine;

namespace Source.Behaviors
{
    public class BulletForcefield : MonoBehaviour, IAttackResponder
    {
        private SoundEffectEmitter _soundEffectEmitter;
        
        [SerializeField]
        [Tooltip("Optional - Sound(s) to play when hit by a bullet")]
        private AudioClip[] _bulletHitSounds;

        private void Start()
        {
            if (!_bulletHitSounds.Any())
                return;
            
            _soundEffectEmitter = GameObject.FindWithTag(Tags.SoundEffectEmitter)?.GetComponent<SoundEffectEmitter>()
                ?? throw new UnityException("No SoundEffectEmitter found in scene");
        }

        public void AttackedByBullet(GameObject bullet)
        {
            if (_bulletHitSounds.Any())
                _soundEffectEmitter.PlayRandom(gameObject, _bulletHitSounds);
            bullet.GetComponent<Bullet>().Ricochet();
        }

        public void AttackedByLaser(GameObject laser)
        {
        }

        public void HitByMissileExplosion(GameObject explosion)
        {
        }

        public void HitByMineExplosion(GameObject explosion)
        {
        }

        public void OnDeath()
        {
        }
    }
}