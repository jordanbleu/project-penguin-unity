using UnityEngine.InputSystem;

namespace Source.Utilities
{
    public class InputUtils
    {
        
        /// <summary>
        /// Input agnostic way to check if anything was hit on any device
        /// </summary>
        /// <returns></returns>
        public static bool AnyKeyHit()
        {
            if (Keyboard.current?.anyKey?.wasPressedThisFrame == true)
                return true;
            
            if (Gamepad.current == null) 
                return false;
            
            // Check if any button was pressed this frame on the gamepad
            foreach (var button in Gamepad.current.allControls)
            {
                if (button.IsPressed())
                {
                    return true;
                }
            }

            return false;
        }
    }
}