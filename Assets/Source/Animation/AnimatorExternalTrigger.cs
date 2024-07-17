
using UnityEngine;

namespace Source.Unity
{
    [Tooltip("Used for triggering animator events from external sources.")]
    [RequireComponent(typeof(Animator))]
    public class AnimatorExternalTrigger : MonoBehaviour
    {
        private Animator _animator;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void TriggerAnimatorEvent(string eventId)
        {
            _animator.SetTrigger(eventId);
        }
        
    }
}