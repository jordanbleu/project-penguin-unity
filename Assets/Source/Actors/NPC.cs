using UnityEngine;

namespace Source.Actors
{
    [Tooltip("Dummy actors that get controlled externally (usually by the director).")]
    public class NPC : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("How fast the NPC moves towards the seek position. ")]
        private float speed = 4f;
        
        private Vector2 _seekPosition;

        private float _leadTimer = 0f;
        private float _leadTimeSeconds = 2f;

        private float _defaultSpeed;
        private void Start()
        {
            _defaultSpeed = speed;
            _seekPosition = transform.position;
        }

        private void Update()
        {
            // the lead time is just so that the NPC has a sec to move before checking if its at its seek pos
            if (_leadTimer < _leadTimeSeconds)
            {
                _leadTimer += Time.deltaTime;
                return;
            }
            
            
            // if position is close to the seek position, stop moving and return
            if (HasArrived())
            {
                return;
            }
            
            var step = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, _seekPosition, step);
        }

        public bool HasArrived() =>
            _leadTimer >= _leadTimeSeconds &&
            Vector2.Distance(transform.position, _seekPosition) < 0.01f;
        
        public void SetSeekY (float y)
        {
            _leadTimer = 0f;
            _seekPosition = new Vector2(transform.position.x, y);
        }
        
        public void SetSeekX (float x)
        {
            _leadTimer = 0f;
            _seekPosition = new Vector2(x, transform.position.y);
        }

        public void SetSpeed(float newSpeed) => speed = newSpeed;

        public void DefaultSpeed() => speed = _defaultSpeed;

    }
}