using Source.Timers;
using UnityEngine;

namespace Source.Director
{
    [Tooltip("Considers itself complete when the attached timer completes")]
    public class DelayInstruction : GameInstruction
    {
        [SerializeField] private float seconds = 5f;

        private bool _isComplete;
        private IntervalEventTimer _timer;

        private void Start()
        {
            _timer = gameObject.AddComponent<IntervalEventTimer>();
            _timer.SetInterval(seconds);
            _timer.AddEventListener(OnTimerComplete);
        }
        
        private void OnTimerComplete()
        {
            _isComplete = true;
            _timer.enabled = false;
        }
        
        public override bool IsInstructionComplete() => _isComplete;

    }
}