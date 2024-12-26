using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Source.Behaviors
{
    /// <summary>
    /// Follows a set of waypoints and then stops at the last one.
    /// </summary>
    public class Pathfinder : MonoBehaviour
    {

        [SerializeField]
        private Waypoint[] waypoints;

        [SerializeField]
        private float maxDistanceDelta = 3;
        
        [SerializeField]
        private bool loop = false;
        
        private int _currentWaypointIndex = 0;

        private void Update()
        {
            if (_currentWaypointIndex >= waypoints.Length)
            {
                if (loop)
                {
                    _currentWaypointIndex = 0;
                }
                else
                {
                    return;
                }
            }
            
            var waypoint = waypoints[_currentWaypointIndex];
            
            
            var distance = Vector2.Distance(transform.position, waypoint.position);
            if (distance < 0.1f)
            {
                waypoint.onWaypointReached.Invoke();
                
                if (!waypoint.waitForSignal)
                    _currentWaypointIndex++;
            }
            else
            {
                // move towards the waypoint using Vector2.MoveTowards
                transform.position = Vector2.MoveTowards(transform.position, waypoint.position, maxDistanceDelta * Time.deltaTime);
            }
            
        }

        [Serializable]
        public class Waypoint
        {
            public Vector2 position;
            public UnityEvent onWaypointReached;
            [Tooltip("If true, the actor will wait here until NextWaypoint() is called externally before continuing on to the next one.")]
            public bool waitForSignal = false;
        }

        public void NextWaypoint()
        {
            _currentWaypointIndex++;
        }

        private void OnDrawGizmos()
        {
            if (!waypoints.Any())
                return;
            
            for (var i = 0; i < waypoints.Length; i++)
            {
                var waypoint = waypoints[i];
                var nextWaypoint = i == waypoints.Length-1 ? null : waypoints[i + 1];

                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(waypoint.position, 0.1f);
                
                if (nextWaypoint != null)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(waypoint.position, nextWaypoint.position);
                }
                
                // Draw the label indicating the index of the waypoint
                UnityEditor.Handles.Label(waypoint.position + Vector2.up * 0.2f, i.ToString());
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            if (!waypoints.Any())
                return;
            
            for (var i = 0; i < waypoints.Length; i++)
            {
                var waypoint = waypoints[i];
                var nextWaypoint = i == waypoints.Length-1 ? null : waypoints[i + 1];

                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(waypoint.position, 0.1f);
                
                if (nextWaypoint != null)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(waypoint.position, nextWaypoint.position);
                }
                
                // Draw the label indicating the index of the waypoint
                UnityEditor.Handles.Label(waypoint.position + Vector2.up * 0.2f, i.ToString());
            }
        }
    }
}