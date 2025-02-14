using System;
using UnityEngine;
using UnityEngine.Events;
using Vector2 = UnityEngine.Vector2;

namespace Source.Behaviors
{
    /// <summary>
    /// Emits objects on an interval, used for bullets etc
    /// </summary>
    public class IntervalEmitter : MonoBehaviour
    {
        [SerializeField] private float bulletDropIntervalSeconds = 2;
        [SerializeField] private bool randomizeInterval = true;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Vector2 positionOffset = Vector2.zero;

        [SerializeField] private float startTime = 0f;
        
        [SerializeField]
        private UnityEvent onEmit = new();
        
        private float _currentTime = 0f;

        private void Start()
        {
            _currentTime = startTime;
        }

        private void Update()
        {
            _currentTime += Time.deltaTime;

            if (_currentTime < bulletDropIntervalSeconds) 
                return;
            
            onEmit?.Invoke();
            var obj = Instantiate(bulletPrefab);
            
            obj.transform.position = ((Vector2)transform.position) + positionOffset;

            if (randomizeInterval)
            {
                _currentTime = UnityEngine.Random.Range(0, bulletDropIntervalSeconds);
            }
            else
            {
                _currentTime = 0;
            }
        }

        private void OnDrawGizmosSelected()
        {
            var pos = transform.position;
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(new Vector3(pos.x + positionOffset.x, pos.y+positionOffset.y), new Vector2(0.25f,0.25f));
        }
    }
}