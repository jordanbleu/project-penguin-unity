using System;
using UnityEngine;

namespace Source.UI
{
    /// <summary>
    /// Idk im tired and need to go to bed soon.
    ///
    /// This just takes the health and tells the animator if its low idk.
    /// </summary>
    public class HealthBarToAnimatorConnector : MonoBehaviour
    {
        [SerializeField]
        private DisplayBar _healthBar;
        
        [SerializeField]
        private Animator _animator;

        private static readonly int LowHealth = Animator.StringToHash("is-health-low");

        private void Update()
        {
            var health = _healthBar.GetValue();
            _animator.SetBool(LowHealth, health < 0.3);
        }
    }
}