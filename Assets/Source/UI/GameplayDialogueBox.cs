using UnityEngine;

namespace Source.UI
{
    /// <summary>
    /// Controls the gameplay dialogue box only.  Does not include things like the text writer, etc.
    /// </summary>
    public class GameplayDialogueBox : MonoBehaviour
    {
        private readonly float _offscreenYPosition = -200f;
        private readonly float _onscreenYPosition = 0;

        [SerializeField]
        private GameObject dialogueFrameObject;
        
        public void Open()
        {
            LeanTween.scaleY(dialogueFrameObject, 1, 1f)
                .setEase(LeanTweenType.easeOutCubic)
                .setOnComplete(OnDialogueBoxOpen);
        }

        private void OnDialogueBoxOpen()
        {
        }

        public void Close()
        {
        }

    }
}