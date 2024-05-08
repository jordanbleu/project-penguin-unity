using System.Text;
using Cinemachine;

namespace Source.Dialogue
{
    public class TypeCharAction : IDialogueAction
    {
        private char _char;

        public TypeCharAction(char @char)
        {
            _char = @char;
        }

        public bool ExecuteUpdate(float deltaTime, StringBuilder displayText, CinemachineImpulseSource cameraImpulseSource)
        {
            displayText.Append(_char);
            return true;
        }
    }
}