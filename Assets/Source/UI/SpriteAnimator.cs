using System;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
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
        [Tooltip("The image to animate if in ui mode")]
        private Image uiImage;

        [SerializeField]
        [Tooltip("In sprite mode, will animate the attached sprite renderer instead of the UI image")]
        private SpriteRenderer sprite;
        
        [SerializeField]
        private float secondsPerFrame = 0.01f;

        [SerializeField]
        private UnityEvent onIterationComplete = new();
        public UnityEvent OnIterationComplete => onIterationComplete;
        
        [SerializeField]
        private float preDelaySeconds = 0f;

        [SerializeField]
        private LoopMode loopMode = LoopMode.Loop;
        
        [SerializeField]
        private bool playOnAwake = true;
        
        private float _preDelayCounter = 0f;
        
        private int _imageIndex = 0;
        private float _elapsedTime = 0f;

        private bool _isPlaying = true;

        private void Start()
        {
            if (sprite != null && uiImage != null)
            {
                Debug.LogWarning("Cannot have both a sprite renderer and a ui image.  Logic will fallback to the ui image.");
            }
            
            if (frames.Any())
            {
                SetFrame(0);
            }
        
        }

        private void OnEnable()
        {
            if (playOnAwake)
            {
                PlayFromBeginning();
            }
        }

        private void SetFrame(int index)
        {
            if (uiImage != null)
            {
                uiImage.sprite = frames[index];
                return;
            }

            sprite.sprite = frames[index];
        
            
        }

        private void Update()
        {
            if (!_isPlaying)
                return;
            
            if (_preDelayCounter < preDelaySeconds)
            {
                _preDelayCounter += Time.deltaTime;
                return;
            }

            if (!frames.Any())
                return;
            
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime < secondsPerFrame)
                return;

            _elapsedTime = 0f;

            _imageIndex += 1;

            if (_imageIndex >= frames.Length)
            {
                
                onIterationComplete?.Invoke();
                
                if (loopMode == LoopMode.Once)
                {
                    _isPlaying = false;
                    return;
                } 

                _imageIndex = 0;
            }

            SetFrame(_imageIndex);
        }

        public void Play() => _isPlaying = true;

        public void PlayFromBeginning()
        {
            Reset();
            Play();
        }

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
            SetFrame(0);
        }

        public void SwapFrames(Sprite[] newFrames)
        {
            Stop();
            frames = newFrames;
        }

        public void SetFrameDelay(float seconds) => secondsPerFrame = seconds;

        public enum LoopMode
        {
            Loop = 0,
            Once = 1,
            
        }
    }
}