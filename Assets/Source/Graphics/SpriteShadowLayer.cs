using System;
using UnityEngine;

namespace Source.Graphics
{
    public class SpriteShadowLayer : MonoBehaviour
    {
        private const float Speed = 0.001f;
        
        private SpriteRenderer _spriteRenderer;
        private SpriteRenderer _masterSpriteRenderer;
        
        private float xSpeed = 0f;
        private float ySpeed = 0f;
        
        public float MaxDistance { get; set; } = 0.1f;
        
        private void Start()
        {
            
            xSpeed = UnityEngine.Random.Range(-Speed, Speed);
            ySpeed = UnityEngine.Random.Range(-Speed, Speed);
            
            if (xSpeed == 0 && ySpeed == 0)
            {
                xSpeed = Speed;
            }
            
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetMasterSpriteRenderer(SpriteRenderer spriteRenderer)
        {
            _masterSpriteRenderer = spriteRenderer;
        }

        private void Update()
        {
            // match master's rotation
            //transform.localRotation = _masterSpriteRenderer.transform.localRotation;
            
            var position = new Vector3(transform.localPosition.x + xSpeed, transform.localPosition.y + ySpeed, transform.localPosition.z);

            transform.localPosition = position;
            
            if (position.x < -MaxDistance || position.x > MaxDistance)
            {
                xSpeed = -xSpeed;
            }   
            
            if (position.y < -MaxDistance || position.y > MaxDistance)
            {
                ySpeed = -ySpeed;
            }
            
            _spriteRenderer.sprite = _masterSpriteRenderer.sprite;
            
        }
    }
}