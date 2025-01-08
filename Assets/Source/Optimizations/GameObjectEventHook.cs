using System;
using UnityEngine;
using UnityEngine.Events;

namespace Source.Optimizations
{
    /// <summary>
    /// This can be used for very simple logic that doesn't require code.
    /// </summary>
    public class GameObjectEventHook : MonoBehaviour
    {

        [SerializeField]
        private UnityEvent OnStart = new();

        [SerializeField]
        private UnityEvent OnDestroyed = new();

        [SerializeField]
        private UnityEvent OnEnabled = new();
        
        private void Start()
        {
            OnStart?.Invoke();
        }

        private void OnDestroy()
        {
            OnDestroyed?.Invoke();
        }

        private void OnEnable()
        {
            OnEnabled?.Invoke();
        }
    }
}