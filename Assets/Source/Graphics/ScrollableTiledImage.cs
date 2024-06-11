using System;
using Source.Extensions;
using UnityEngine;

namespace Source.Graphics
{
    /// <summary>
    /// Used for independent parallaxing background (aka, space)
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class ScrollableTiledImage : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        [Tooltip("Speed that the image is automatically scrolling per frame")]
        private Vector2 autoScrollSpeed = Vector2.zero;
        
        private Vector2 _scroll = Vector2.zero;
        private Vector2 _size = Vector2.zero;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _size = CalculateSizeInUnits();
            var position = (Vector2)transform.position;

            CloneSprite("topLeftBuffer", position.Move(-_size.x, _size.y));
            CloneSprite("topBuffer", position.Move(0, _size.y));
            CloneSprite("topRightBuffer", position.Move(_size.x, _size.y));

            CloneSprite("middleLeftBuffer", position.Move(-_size.x,0));
            // no middle buffer because the original is in the middle
            CloneSprite("middleRightBuffer", position.Move(_size.x, 0));
            
            CloneSprite("bottomLeftBuffer", position.Move(-_size.x, -_size.y));
            CloneSprite("bottomBuffer", position.Move(0, -_size.y));
            CloneSprite("bottomRightBuffer", position.Move(_size.x, -_size.y));
        }

        private void FixedUpdate()
        {
            var position = (Vector2)transform.position;

            var newX = position.x + autoScrollSpeed.x;
            var newY = position.y + autoScrollSpeed.y;

            if (newY >= _size.y || newY <= -_size.y)
            {
                newY = 0;
            }

            if (newX >= _size.x || newX <= -_size.x)
            {
                newX = 0;
            }

            transform.position = new Vector2(newX, newY);
        }

        private void CloneSprite(string name, Vector2 position)
        {
            var gameObj = new GameObject(name)
            {
                transform =
                {
                    // re-parent the clone so it naturally moves with the original object
                    parent = gameObject.transform,
                    position = position
                }
            };

            // add a sprite renderer that is more or less a copy of the original one 
            var newSpriteRenderer = gameObj.AddComponent<SpriteRenderer>();
            newSpriteRenderer.sprite = _spriteRenderer.sprite;
            newSpriteRenderer.color = _spriteRenderer.color;
            newSpriteRenderer.flipX = _spriteRenderer.flipX;
            newSpriteRenderer.flipY = _spriteRenderer.flipY;
            newSpriteRenderer.size = _spriteRenderer.size;
            newSpriteRenderer.drawMode = _spriteRenderer.drawMode;
            newSpriteRenderer.tileMode = _spriteRenderer.tileMode;
            newSpriteRenderer.spriteSortPoint = _spriteRenderer.spriteSortPoint;
            newSpriteRenderer.shadowCastingMode = newSpriteRenderer.shadowCastingMode;
            newSpriteRenderer.maskInteraction = _spriteRenderer.maskInteraction;
            newSpriteRenderer.spriteSortPoint = _spriteRenderer.spriteSortPoint;
            newSpriteRenderer.material = _spriteRenderer.material;
            newSpriteRenderer.sortingLayerID = _spriteRenderer.sortingLayerID;
            newSpriteRenderer.sortingOrder = _spriteRenderer.sortingOrder;
        }

        // Sprite's width in pixels / the pixels per unit
        private Vector2 CalculateSizeInUnits()
        {
            var sprite = _spriteRenderer.sprite;
            var ppu = sprite.pixelsPerUnit;
            var rect = sprite.rect;
            var w = rect.width / ppu;
            var h = rect.height / ppu;
            return new Vector2(w, h);
        }
        
        public void SetYSpeed(float speed)
        {
            autoScrollSpeed.y = speed;
        }
        
        public void SetXSpeed(float speed)
        {
            autoScrollSpeed.x = speed;
        }

    }
}