using System.Text;
using Cinemachine;

namespace Source.Dialogue
{
    public class ScreenShakeAction : IDialogueAction
    {
        public bool ExecuteUpdate(float deltaTime, StringBuilder displayText, CinemachineImpulseSource cameraImpulseSource)
        {
            cameraImpulseSource.GenerateImpulse();
            return true;
        }
    }
}