using System.Text;
using Cinemachine;

namespace Source.Dialogue
{
    public class DelayAction : IDialogueAction
    {
        private readonly float _delayTime = 0f;

        private float _elapsedTime = 0f;
        
        public DelayAction(float time)
        {
            _delayTime = time;
        }

        public bool ExecuteUpdate(float deltaTime, StringBuilder displayText, CinemachineImpulseSource cameraImpulseSource)
        {
            _elapsedTime += deltaTime;

            return _elapsedTime >= _delayTime;
        }
    }
}