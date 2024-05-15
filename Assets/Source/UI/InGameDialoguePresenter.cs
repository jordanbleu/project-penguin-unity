using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Source.Dialogue;
using UnityEngine;

namespace Source.UI
{
    /// <summary>
    /// Handles the animation and tells the text writer to do its thing
    /// </summary>
    public class InGameDialoguePresenter : MonoBehaviour
    {
        private const float OffScreenY = -175;
        private const float OnScreenY = 0;
        private const float AnimationSeconds = 0.75f;
        
        [SerializeField]
        private DialogueTyper dialogueTyper;

        [SerializeField]
        private SpriteAnimator avatarAnimator;

        [SerializeField]
        private GameObject doneTypingIndicator;

        private Dictionary<string, Sprite[]> _avatarSpriteMappings;
        
        private string _avatarId;
        
        private void Start()
        {
            dialogueTyper.OnDialogueComplete.AddListener(OnCompleted);
            dialogueTyper.OnLineBegin.AddListener(OnLineBegin);
            dialogueTyper.OnLineComplete.AddListener(OnLineEnd);
            doneTypingIndicator.SetActive(false);
            _avatarSpriteMappings = GetComponent<AvatarSpriteMapping>().ToDictionary();
        }

        public void PresentDialogueFromFile(string filePath)
        {
            doneTypingIndicator.SetActive(false);
            LeanTween.moveLocalY(gameObject, OnScreenY, AnimationSeconds)
                .setEase(LeanTweenType.easeInOutBack);
            
            // todo: show some sort of cool loading thing 
            StartCoroutine(PresentDialogueInternal(filePath));
        }

        private IEnumerator PresentDialogueInternal(string filePath)
        {
            // Load from file, then yield execution
            var textAsset = Resources.Load<TextAsset>("Dialogue/en/" + filePath);
            var splitLines = textAsset.text.Split('\n')
                .Select(l=>l.Trim())
                .Where(l=>!string.IsNullOrEmpty(l) && !l.StartsWith("#"));
            yield return null;

            // parse each line 
            var dialogueLines = splitLines.Select(DialogueParser.ParseDialogueLine).ToList();
            OnDataReady(dialogueLines);
            yield return null;
        }

        /// <summary>
        /// Dialogue is loaded and parsed
        /// </summary>
        /// <param name="dialogueLines"></param>
        private void OnDataReady(List<DialogueLine> dialogueLines)
        {
            dialogueTyper.Present(dialogueLines);
        }

        /// <summary>
        /// Dialogue presentation is over
        /// </summary>
        private void OnCompleted()
        {
            LeanTween.moveLocalY(gameObject, OffScreenY, AnimationSeconds)
                .setEase(LeanTweenType.easeInOutBack);

            _avatarId = null;
        }

        private void OnLineBegin()
        {
            doneTypingIndicator.SetActive(false);
            var avatarId = dialogueTyper.AvatarId ?? "";

            // update the avatar if needed
            if (avatarId != _avatarId)
            {
                _avatarId = avatarId;
                var frames = _avatarSpriteMappings[_avatarId];
                avatarAnimator.SwapFrames(frames);

                // avatar animation speed mirrors the text writer's speed (kinda)
                avatarAnimator.SetFrameDelay(dialogueTyper.CurrentTypingSpeed * 4f);
            }

            avatarAnimator.Reset();
            avatarAnimator.Play();
        }

        private void OnLineEnd()
        {
            doneTypingIndicator.SetActive(true);
            // the first frame is always the 'idle' non-speaking frame
            avatarAnimator.Stop();
        }

    }
}