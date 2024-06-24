using UnityEngine;

namespace Source.Graphics
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteRandomizer : MonoBehaviour
    {
        [Tooltip("One of these will be chosen at random")]
        [SerializeField]
        private Sprite[] sprites;
        
        private void Start()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
        }
        
    }
}