
using Source.Behaviors;
using Source.Extensions;
using Source.Interfaces;
using Source.Mathematics;
using Source.Weapons;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Source.Actors
{
    /// <summary>
    /// The bee acts like...a bee.  It will hover around a position, then choose a random position, then repeat. 
    /// </summary>
    [RequireComponent(typeof(Attackable), typeof(Animator))]
    public class Bee : MonoBehaviour, IAttackResponder, ICollideWithPlayerResponder
    {
        [SerializeField]
        [Tooltip("How long the bee hovers around a target position before moving to another.")]
        private float waitSeconds = 5;
        
        [SerializeField]
        [Tooltip("center position of the zone")]
        private Vector2 zonePosition = new Vector2(0, 5);

        [SerializeField] 
        [Tooltip("Width / height of the movement zone ")]
        private Vector2 zoneSize = new Vector2(33, 10);

        [SerializeField]
        private GameObject bulletPrefab;

        [SerializeField]
        private float shootIntervalSeconds = 6;

        private float _shootIntervalTimer;
        
        [SerializeField]
        private float speed = 15f;
        
        private float _moveWaitTimer;
        
        // The actual world position the bee is trying to reach
        private Vector2 _targetPosition;
        
        private Range<float> _xZoneRange;
        private Range<float> _yZoneRange;

        private float _hoverTimer;

        private Attackable _attackable;
        private Animator _animator;
        private Rigidbody2D _rigidBody;
        
        private static readonly int DeathAnimatorParam = Animator.StringToHash("death");
        private bool _isAlive = true;
        private static readonly int IdleAnimAnimatorParam = Animator.StringToHash("idleAnim");

        private float _startAnimationTimer;
        private bool _animatorStarted;
        private static readonly int StartAnimAnimatorParam = Animator.StringToHash("startAnim");

        private void Start()
        {
            var halfSize = zoneSize / 2;
            
            _xZoneRange = new Range<float>(zonePosition.x - halfSize.x, zonePosition.x + halfSize.x);
            _yZoneRange = new Range<float>(zonePosition.y - halfSize.y, zonePosition.y + halfSize.y);
            
            _shootIntervalTimer = shootIntervalSeconds;
            _attackable = GetComponent<Attackable>();
            _animator = GetComponent<Animator>();
            _rigidBody = GetComponent<Rigidbody2D>();

            var idleAnim = Random.Range(1, 4);
            _animator.SetInteger(IdleAnimAnimatorParam, idleAnim);

            // wait up to one second to start the animation
            // this ensures the bees don't all animate in sync
            _startAnimationTimer = Random.Range(0f, 1f);
        }

        private void Update()
        {
            if (!_animatorStarted)
            {
                _startAnimationTimer -= Time.deltaTime;
                
                if (_startAnimationTimer <= 0)
                {
                    _animatorStarted = true;
                    _animator.SetTrigger(StartAnimAnimatorParam);
                }
            }


            // if dying just stay put
            if (!_isAlive)
                return;
            
            var pos = transform.position;
            var step = speed * Time.deltaTime;

            var adjustedTargetPosition = _targetPosition;
            
            // only count down if the bee is within the target position
            if (pos.x.IsWithin(0.5f, _targetPosition.x) &&
                pos.y.IsWithin(0.5f, _targetPosition.y))
            {
                _moveWaitTimer -= Time.deltaTime;
                ShootUpdate();
            }
            
            if (_moveWaitTimer <= 0)
            {
                _targetPosition = new Vector2(Random.Range(_xZoneRange.Min, _xZoneRange.Max),
                    Random.Range(_yZoneRange.Min, _yZoneRange.Max));
                
                _moveWaitTimer = Random.Range(waitSeconds/4, waitSeconds);
            }
            
            transform.position = Vector2.MoveTowards(pos, adjustedTargetPosition, step);
        }

        private void ShootUpdate()
        {
            _shootIntervalTimer -= Time.deltaTime;
            if (!(_shootIntervalTimer <= 0)) 
                return;
            _shootIntervalTimer = shootIntervalSeconds;
            Instantiate(bulletPrefab).At(transform.position);
        }

        private void Die()
        {
            // prevent the bee from shooting during death animation
            _isAlive = false;
            _attackable.Die();
        }

        public void AttackedByBullet(GameObject attackingBullet)
        {
            attackingBullet.GetComponent<Bullet>().HitSomething();
            _rigidBody.AddForce(new(0,5), ForceMode2D.Impulse);
            Die();
        }


        public void AttackedByLaser(GameObject laser)
        {
            Die();
        }

        public void HitByMissileExplosion(GameObject explosion)
        {
            Die();
        }

        public void HitByMineExplosion(GameObject explosion)
        {
            Die();
        }

        public void OnDeath()
        {
            _animator.SetTrigger(DeathAnimatorParam);
        }

        public void CollideWithPlayer(Player playerComponent)
        {
            playerComponent.TakeDamage(10);
            
            Die();
        }

        private void OnDrawGizmosSelected()
        {
            // orange
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(zonePosition, zoneSize);
        }
    }
}