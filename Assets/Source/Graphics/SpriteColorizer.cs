using System;
using UnityEngine;

namespace Source.Graphics
{
    public class SpriteColorizer : MonoBehaviour
    {
        private void Start()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();

            spriteRenderer.color = GameColorUtils.GetRandomUnityColor();
        }
    }
}