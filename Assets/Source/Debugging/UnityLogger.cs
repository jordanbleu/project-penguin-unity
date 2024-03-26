using UnityEngine;

namespace Source.Debugging
{
    public class UnityLogger : MonoBehaviour
    {
        public void Log(string message) =>
            UnityEngine.Debug.Log(message);
        
    }
}