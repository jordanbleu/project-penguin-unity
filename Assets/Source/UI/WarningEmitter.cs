using UnityEngine;
using UnityEngine.Serialization;
using Source.Extensions;
namespace Source.UI
{
    /// <summary>
    /// when the player enters the collider, will emit a warning toast
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class WarningEmitter : MonoBehaviour
    {
        [SerializeField]
        private GameObject warningToastPrefab;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            // finds the HUD
            var hud = GameObject.FindWithTag("hud");
            
            if (hud == null)
            {
                Debug.LogError("Please add an object in the canvas with a tag named 'hud'");
                return;
            }

            var x = Random.Range(-100, 100);
            var y = 100 + Random.Range(-20, 20);
            
            // instantiates the warning toast
            Instantiate(warningToastPrefab, hud.transform).AtLocal(x, y);
        }
        

    }
}