using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Source.Optimizations
{
    /// <summary>
    /// Add to a gameObject to expose methods to disable or enable the attached collider
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class ColliderToggle : MonoBehaviour
    {
        private Collider2D _collider;
        private void Start()
        {
            _collider = GetComponent<Collider2D>();
        }

        public void EnableCollider() =>
            _collider.enabled = true;

        public void DisableCollider() =>
            _collider.enabled = false;
    }
}