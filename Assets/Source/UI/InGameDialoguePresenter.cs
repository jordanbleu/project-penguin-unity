using System;
using Source.Dialogue;
using UnityEngine;

namespace Source.UI
{
    /// <summary>
    /// Handles the animation and tells the text writer to do its thing
    /// </summary>
    public class InGameDialoguePresenter : MonoBehaviour
    {
        [SerializeField]
        private TextTyper typer;

        [SerializeField]
        private Animator dialogueAnimator;

        private static readonly int AvatarIdAnimatorParameter = Animator.StringToHash("avatarId");
        private static readonly int IsSpeakingAnimatorParameter = Animator.StringToHash("isSpeaking");
        
        private const float OffScreenY = -152;
        private const float OnScreenY = 0;
        private const float AnimationSeconds = 0.75f;

        private void Start()
        {
            typer.OnDialogueCompleted.AddListener(CloseDialogueBox);
            typer.OnDialogueLineBegin.AddListener(RefreshAvatar);
            typer.OnDialogueLineEnd.AddListener(DoneSpeaking);
            dialogueAnimator.gameObject.SetActive(false);
        }

        private void Update()
        {
            dialogueAnimator.SetBool(IsSpeakingAnimatorParameter, typer.IsTypingLetters());
        }

        /// <summary>
        /// Starts the animation to begin animating dialogue
        /// </summary>
        /// <param name="filePath"></param>
        public void PresentDialogue(string filePath)
        {
            dialogueAnimator.gameObject.SetActive(true);
            LeanTween.moveLocalY(gameObject, OnScreenY, AnimationSeconds)
                .setEase(LeanTweenType.easeInOutBack)
                .setOnComplete(()=>OnReady(filePath));
        }

        /// <summary>
        /// called when dialogue box is ready
        /// </summary>
        private void OnReady(string filePath)
        {
            typer.BeginPresentingDialogue(filePath);
        }

        public void CloseDialogueBox()
        {
            dialogueAnimator.gameObject.SetActive(true);
            LeanTween.moveLocalY(gameObject, OffScreenY, AnimationSeconds)
                .setEase(LeanTweenType.easeInOutBack);
        }
        
        private void RefreshAvatar()
        {
            dialogueAnimator.SetInteger(AvatarIdAnimatorParameter, (int)typer.Avatar);
            dialogueAnimator.SetBool(IsSpeakingAnimatorParameter, true);
        }

        private void DoneSpeaking()
        {
            dialogueAnimator.SetBool(IsSpeakingAnimatorParameter, false);

        }
    }
}