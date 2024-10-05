using System;
using Source.Actors;
using Source.Constants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Source.UI
{
    public class ReloadBar : MonoBehaviour
    {
        private const float OffScreenY = -27f;
        private const float OnScreenY = 0f;

        private const float TransitionSpeed = 0.5f;
        
        private Player _player;

        [SerializeField]
        private DisplayBar _reloadBar;
        
        [SerializeField]
        [Tooltip("any images that should be colorized go here")]
        private Image[] _reloadBarImages;
        
        [SerializeField]
        [Tooltip("Any text mesh objects that should be colorized go here")]
        private TextMeshProUGUI[] _reloadBarTexts;
        
        
        private void Start()
        {
            transform.localPosition = new Vector3(0, OffScreenY, 0);
            
            _player = GameObject.FindWithTag(Tags.Player)?.GetComponent<Player>();
            if (_player != null)
            {
                _player.OnActiveReloadFailure.AddListener(HandleActiveReloadFailure);
                _player.OnActiveReloadSuccess.AddListener(HandleActiveReloadSuccess);
                _player.OnActiveReloadEnd.AddListener(HandleReloadEnd);
                _player.OnActiveReloadBegin.AddListener(HandleReloadBegin);
            }
            
        }

        private void HandleReloadBegin()
        {
            Colorize(Color.white);
            LeanTween.moveLocalY(gameObject, OnScreenY, TransitionSpeed)
                .setEase(LeanTweenType.easeOutBack);
        }

        private void HandleReloadEnd()
        {
            LeanTween.moveLocalY(gameObject, OffScreenY, TransitionSpeed)
                .setEase(LeanTweenType.easeInBack);
        }

        private void Update()
        {
            var playerRemainingReloadTime = _player.RemainingReloadTime;
            _reloadBar.SetValue(1-(playerRemainingReloadTime/100));
        }

        private void OnDestroy()
        {
            if (_player != null)
            {
                _player.OnActiveReloadFailure.RemoveListener(HandleActiveReloadFailure);
                _player.OnActiveReloadSuccess.RemoveListener(HandleActiveReloadSuccess);
                _player.OnActiveReloadEnd.RemoveListener(HandleReloadEnd);
                _player.OnActiveReloadBegin.RemoveListener(HandleReloadBegin);
            }
        }

        private void HandleActiveReloadFailure()
        {
            Colorize(Color.grey);
        }

        private void HandleActiveReloadSuccess()
        {
            Colorize(Color.green);
        }

        private void Colorize(UnityEngine.Color color)
        {
            foreach (var image in _reloadBarImages)
            {
                image.color = color;
            }
            
            foreach (var text in _reloadBarTexts)
            {
                text.color = color;
            }
        }

    }
        
        
 
}