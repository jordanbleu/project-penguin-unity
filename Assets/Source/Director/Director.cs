using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

namespace Source.Director
{
    /// <summary>
    /// The director is in charge of managing the overall game flow.
    ///
    /// Each 'segment' is a gameObject with an ISegment component on it.  The director will
    /// cycle through each wave, deactivating previous game objects.
    ///
    /// Segments can include spawning enemies, time delays, dialogue, etc.
    /// </summary>
    public class Director : MonoBehaviour
    {
        [SerializeField] [Tooltip("List of game objects, each containing one ISegment component.")]
        private List<GameSegment> segmentList;

        [SerializeField] [Tooltip("What to do when all segments are completed")]
        private UnityEvent onSegmentsCompleted = new();

        private Queue<GameSegment> _segments;
        private GameSegment _currentSegment;
        
        private void Start()
        {
            _segments = new Queue<GameSegment>(segmentList);
            BeginNextSegment();
        }

        private void Update()
        {
            if (_currentSegment is null)
                return;

            if (_currentSegment.IsSegmentComplete())
                BeginNextSegment();
            
        }

        private void BeginNextSegment()
        {
            if (_currentSegment is not null)
            {
                _currentSegment.SegmentEnd();
                _currentSegment.gameObject.SetActive(false);
            }

            
            if (!_segments.TryDequeue(out _currentSegment))
            {
                _currentSegment = null;
                onSegmentsCompleted.Invoke();
                return;
            }

            _currentSegment.gameObject.SetActive(true);
            _currentSegment.SegmentBegin();
            
        }




    }
}