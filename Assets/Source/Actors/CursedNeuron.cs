using System;
using Cinemachine;
using Source.Behaviors;
using Source.Extensions;
using Source.Interfaces;
using Source.Weapons;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Source.Actors
{
    /// <summary>
    /// Boss Battle #1.
    /// </summary>
    public class CursedNeuron : MonoBehaviour, IAttackResponder
    {
        [SerializeField]
        private GameObject damageEffectPrefab;
        
        [SerializeField]
        private CursedNeuronNode[] nodes;
        
        [SerializeField]
        private GameObject shield;

        [SerializeField]
        private CinemachineImpulseSource impulseSource;

        [SerializeField]
        private GameObject debrisExplosionPrefab;

        [SerializeField]
        private GameObject introDialogue; 
        
        private bool _isShieldActive = true;
        private Attackable _attackable;
        private State _state = State.Intro;
        
        private void Start()
        {
            // disable the nodes so they can animate back in
            foreach (var node in nodes)
            {
                node.gameObject.SetActive(false);
            }

            _attackable = GetComponent<Attackable>();
        }
        
        private void Update()
        {
            _isShieldActive = UpdateShieldStatus();
            
            switch (_state)
            {
                case State.Intro:
                    IntroUpdate();
                    break;
                case State.Dialogue:
                    MoveTowardsDefaultPosition();
                    break;
                case State.Phase1:
                    break;
                case State.Phase2:
                    break;
                case State.Phase3:
                    break;
                case State.Dying:
                    break;
                case State.ActivatingPods:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void IntroUpdate()
        {
            var pos = transform.position;

            if (pos.y <= 7f)
            {
                _state = State.Dialogue;
                // trigger dialogue 1 
            }

            MoveTowardsDefaultPosition();
        }

        private void MoveTowardsDefaultPosition()
        {
            var pos = transform.position;
            // move y position towards 7 using MoveTowards
            var step = 4f * Time.deltaTime;
            transform.position = Vector2.MoveTowards(pos, new Vector2(pos.x, 7f), step);
        }

        private bool UpdateShieldStatus()
        {
            var shieldActive = false;
            
            foreach (var node in nodes)
            {
                if (node.IsEnabled())
                {
                    shieldActive = true;
                    break;
                }
            }

            shield.SetActive(shieldActive);
            return shieldActive;
        }


        public void AttackedByBullet(GameObject bullet)
        {
            var bulletComp = bullet.GetComponent<Bullet>();
            if (_isShieldActive)
            {
                bulletComp.Ricochet();
                return;
            }

            var pos = transform.position;
            Instantiate(damageEffectPrefab).At(pos.x + Random.Range(-1, 1), pos.y + Random.Range(-1, 1));
            _attackable.Damage(1);
            bulletComp.HitSomething();
            impulseSource.GenerateImpulse();
            
            // explode every 10 hit points
            if (_attackable.Health % 10 == 0)
                Instantiate(debrisExplosionPrefab).At(pos.x, pos.y);
        }

        public void AttackedByLaser(GameObject laser)
        {
            throw new NotImplementedException();
        }

        public void HitByMissileExplosion(GameObject explosion)
        {
            throw new NotImplementedException();
        }

        public void HitByMineExplosion(GameObject explosion)
        {
            throw new NotImplementedException();
        }

        public void OnDeath()
        {
            throw new NotImplementedException();
        }

        private enum State
        {
            /// <summary>
            /// Neuron is moving down to the default position
            /// </summary>
            Intro,
            /// <summary>
            /// Neuron has started dialogue and is waiting for callback 
            /// </summary>
            Dialogue,
            /// <summary>
            /// Neruon is doing the 'activating pods' animation
            /// </summary>
            ActivatingPods,
            /// <summary>
            /// Neuron is in battle phase 1
            /// </summary>
            Phase1,
            /// <summary>
            /// Neuron is in battle phase 2
            /// </summary>
            Phase2,
            /// <summary>
            /// Neuron is on the final battle phase 
            /// </summary>
            Phase3,
            /// <summary>
            /// Neuron is doing the dying animation
            /// </summary>
            Dying
        }
    }
}