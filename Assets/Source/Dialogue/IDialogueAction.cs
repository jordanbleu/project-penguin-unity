using System.Text;
using Cinemachine;

namespace Source.Dialogue
{
    public interface IDialogueAction
    {
        bool ExecuteUpdate(float deltaTime, StringBuilder displayText, CinemachineImpulseSource cameraImpulseSource);
    }
}
