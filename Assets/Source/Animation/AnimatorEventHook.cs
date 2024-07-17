using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Source.Unity
{
    /// <summary>
    /// This just provides a single entry point for a group of unity events to be triggered.  Generally used in the animator.
    /// </summary>
    public class AnimatorEventHook : MonoBehaviour
    {
        [SerializeField]
        private List<AnimatorEvent> animatorEvents = new();

        private Dictionary<string, UnityEvent> _eventDictionary = new();
        private void Start()
        {
            _eventDictionary = animatorEvents.ToDictionary(x => x.eventId, x => x.unityEvent);
        }
        
        public void TriggerEvent(string eventId)
        {
            if (_eventDictionary.TryGetValue(eventId, out var unityEvent))
            {
                unityEvent.Invoke();
                return;
            }
            
            Debug.LogWarning($"Event with id {eventId} not found.");
            
        }


        [Serializable]
        public class AnimatorEvent
        {
            public string eventId = "";
            public UnityEvent unityEvent = new();
        }
    }
}