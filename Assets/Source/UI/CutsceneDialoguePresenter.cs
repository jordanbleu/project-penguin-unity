
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Source.Audio;
using Source.Constants;
using Source.Dialogue;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Source.UI
{
    public class CutsceneDialoguePresenter : MonoBehaviour
    {
        [SerializeField]
        private DialogueTyper dialogueTyper;

        [SerializeField]
        private GameObject doneTypingIndicator;

        [SerializeField]
        private GameObject titleBoxObject;
        
        [SerializeField]
        [Tooltip("Filename within 'Dialogue/en' folder. This class loads the full dialogue on demand and caches it.")]
        private string dialogueFilePath;

        [SerializeField]
        [Tooltip("Called when the dialogue is fully loaded, since it happens asyncronously.  It's recommended to " +
                 "not try to display dialogue before this event is fired..")]
        private UnityEvent onDialogueLoaded;

        [SerializeField]
        private AudioClip typingSound;
        
        [SerializeField]
        private AudioClip cycleDialogueSound;
        
        private Dictionary<string, List<DialogueLine>> _linesById;

        public bool IsDonePresenting { get; private set; }

        private SoundEffectEmitter _soundEmitter;
        
        private void Start()
        {
            _soundEmitter = GameObject.FindWithTag(Tags.SoundEffectEmitter)?.GetComponent<SoundEffectEmitter>();
            
            if (!_soundEmitter)
                throw new UnityException("Missing game object tagged as SoundEffectEmitter in the scene");
            
            if (onDialogueLoaded == null)
                Debug.LogWarning("onDialogueLoaded event is ignored and it shouldn't be.");
            
            dialogueTyper.OnLineBegin.AddListener(OnLineBegin);
            dialogueTyper.OnLineComplete.AddListener(OnLineEnd);
            dialogueTyper.OnTypeLetter.AddListener(OnTypeLetter);
            dialogueTyper.OnUserCycleDialogue.AddListener(OnCycleDialogue);
            
            dialogueTyper.OnDialogueComplete.AddListener(OnDialogueCompleted);
            doneTypingIndicator.SetActive(false);
            
            // hide the title box until we have a title to show.
            titleBoxObject.SetActive(false);

            StartCoroutine(BeginLoadingDialogue());
        }

        private void OnCycleDialogue()
        {
            _soundEmitter.Play(cycleDialogueSound);
        }

        private void OnDialogueCompleted()
        {
            IsDonePresenting = true;
            dialogueTyper.ClearTextBoxes(); // fixes a brief flash of old text
            doneTypingIndicator.SetActive(false);
            titleBoxObject.SetActive(false);
        }
        
        private void OnDestroy()
        {
            dialogueTyper.OnLineBegin.RemoveListener(OnLineBegin);
            dialogueTyper.OnLineComplete.RemoveListener(OnLineEnd);
            dialogueTyper.OnTypeLetter.RemoveListener(OnTypeLetter);
            dialogueTyper.OnUserCycleDialogue.RemoveListener(OnCycleDialogue);
        }

        private void OnTypeLetter()
        {
            _soundEmitter.Play(typingSound);
        }

        /// <summary>
        /// Wire this up in the CutsceneInputHandler
        /// </summary>
        /// <param name="context"></param>
        public void CycleDialogue(InputAction.CallbackContext context)
        {
            // if the button is pressed.
            if (!context.started)
                return;
            
            dialogueTyper.UserCycleDialogue();
        }

        private IEnumerator BeginLoadingDialogue()
        {
            // load the file
            var textAsset = Resources.Load<TextAsset>("Dialogue/en/" + dialogueFilePath);

            if (textAsset == null)
                throw new UnityException($"Dialogue file not found: {dialogueFilePath}");
            
            yield return null;

            // split the lines and parse
            var splitLines = textAsset.text.Split('\n')
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrEmpty(l) && !l.StartsWith("#"));

            var dialogueLines = splitLines.Select(DialogueParser.ParseDialogueLine).ToList();

            // group the dialogue lines into each Identifier and a list of all lines with that identifier
            _linesById = dialogueLines.GroupBy(l => l.Identifier)
                .ToDictionary(g => g.Key, g => g.ToList());

            OnDataReady();
        }

        private void OnDataReady()
        {
            onDialogueLoaded?.Invoke();
        }

        public void PresentDialogueById(string identifier)
        {
            IsDonePresenting = false;

            if (_linesById == null)
                throw new UnityException(
                    "Dialogue not loaded yet or no lines exist.  Please wait for the onDialogueLoaded event.");
            
            var lines = _linesById[identifier];

            if (!lines.Any())
                throw new UnityException($"No lines with id {identifier} found");
                
            dialogueTyper.Present(lines);
        }
        
        public void OnLineBegin()
        {
            if (string.IsNullOrEmpty(dialogueTyper.CurrentTitle))
            {
                titleBoxObject.SetActive(false);
            }
            else
            {
                titleBoxObject.SetActive(true);
            }
            
            doneTypingIndicator.SetActive(false);
        }

        public void OnLineEnd()
        {
            doneTypingIndicator.SetActive(true);
        }
    }
}