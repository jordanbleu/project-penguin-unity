using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Source.Graphics
{
    /// <summary>
    /// Adds that signature sexy color shadow to the sprite and brings it to life 
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteShadow : MonoBehaviour
    {
        [SerializeField]
        private Color shadowColor = Color.green;

        [SerializeField]
        private int numberOfLayers = 3;

        [SerializeField]
        private float shadowDistance = 0.1f;
        
        private void Start()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();

            var darkenssPerLayer = (float)1/numberOfLayers;
            
            for (var i=0;i<numberOfLayers;i++)
            {
                CreateShadowLayer(i, spriteRenderer, shadowColor, darkenssPerLayer * i);
            }

        }

        private void CreateShadowLayer(int layerIndex, SpriteRenderer spriteRenderer, Color color, float darkenPercent)
        {
            var layer = new GameObject("shadow-layer-" + layerIndex);
            
            // add the sprite renderer and set it to the same sprite
            var layerSpriteRenderer = layer.AddComponent<SpriteRenderer>();
            layerSpriteRenderer.sprite = spriteRenderer.sprite;
            layerSpriteRenderer.color = DarkenColor(color, darkenPercent);
            
            //add the sprite shadow layer component 
            var shadowLayer = layer.AddComponent<SpriteShadowLayer>();
            shadowLayer.SetMasterSpriteRenderer(spriteRenderer);
            shadowLayer.MaxDistance = shadowDistance;
            
            // set the layer to be a child of the parent
            layer.transform.SetParent(transform);

            layer.transform.localPosition = new Vector3(0, 0, layerIndex + 1);
            layer.transform.rotation = spriteRenderer.transform.rotation;
        }

        private Color DarkenColor(Color color, float percent)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            v -= v*percent;
            return Color.HSVToRGB(h, s, v);
        }
    }
}