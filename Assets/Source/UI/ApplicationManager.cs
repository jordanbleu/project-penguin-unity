using System.Collections;
using UnityEngine;

namespace Source.UI
{
    [Tooltip("Handles various things related to the application itself.")]
    public class ApplicationManager : MonoBehaviour
    {
        public void BeginExitingGame()
        {
            StartCoroutine(QuitAfterDelay());
        }

        private IEnumerator QuitAfterDelay()
        {
            yield return new WaitForSeconds(1f);
            
            Application.Quit();
            
            // if editor is playing quite that
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
            
        }
    }
}