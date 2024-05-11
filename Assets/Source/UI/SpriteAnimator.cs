using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Source.UI
{
    /// <summary>
    /// Simply swaps sprites in an animated sort of way
    ///
    /// Meant for UI elements, avoids the overhead of the animator.
    /// </summary>
    public class SpriteAnimator : MonoBehaviour
    {

        [SerializeField]
        private Sprite[] frames;

        [SerializeField]
        private Image uiImage;

        [SerializeField]
        private float secondsPerFrame = 0.01f;
        
        private int _imageIndex = 0;
        private float _elapsedTime = 0f;

        private bool _isPlaying = true;
        
        private void Update()
        {
            if (!_isPlaying)
                return;

            if (!frames.Any())
                return;
            
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime < secondsPerFrame)
                return;

            _elapsedTime = 0f;

            _imageIndex += 1;

            if (_imageIndex >= frames.Length)
            {
                _imageIndex = 0;
            }

            uiImage.sprite = frames[_imageIndex];
        }

        public void Play() => _isPlaying = true;
        public void Pause() => _isPlaying = false;

        public void Stop()
        {
            Pause();
            Reset();
        }

        public void Reset()
        {
            if (!frames.Any())
                return;
            
            _imageIndex = 0;
            uiImage.sprite = frames[0];
        }

        public void SwapFrames(Sprite[] newFrames)
        {
            Stop();
            frames = newFrames;
        }

        public void SetFrameDelay(float seconds) => secondsPerFrame = seconds;

    }
}