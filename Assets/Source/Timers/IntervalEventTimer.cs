using UnityEngine;
using UnityEngine.Events;

namespace Source.Timers
{
    /// <summary>
    /// Triggers a unity event over and over on a fixed interval.
    /// </summary>
    public class IntervalEventTimer : MonoBehaviour
    {
        [SerializeField] private float intervalSeconds = 1;
        [SerializeField] private UnityEvent onIntervalReached = new();
        
        private float _currentTime = 0f;
        
        private void Update()
        {
            
            _currentTime +=  Time.deltaTime;

            if (_currentTime >= intervalSeconds)
            {
                onIntervalReached?.Invoke();
                _currentTime = 0;

            }
        }

        /// <summary>
        /// Sets the timer's interval in seconds
        /// </summary>
        /// <param name="seconds"></param>
        public void SetInterval(float seconds)
        {
            intervalSeconds = seconds;
        }

        /// <summary>
        /// Sets the interval to a random value up to the max interval
        /// </summary>
        public void PreWarm()
        {
            _currentTime = UnityEngine.Random.Range(0, intervalSeconds);
        }

        public void AddEventListener(UnityAction action) =>
            onIntervalReached.AddListener(action);


    }
}