using UnityEngine;

namespace Source.UI
{
    /// <summary>
    /// when the player enters the collider, will emit a warning toast
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class WarningEmitter : MonoBehaviour
    {

        [SerializeField]
        private Toast warningToast;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            
            if (warningToast.isActiveAndEnabled)
            {
                return;
            }
                
            warningToast.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
        

    }
}