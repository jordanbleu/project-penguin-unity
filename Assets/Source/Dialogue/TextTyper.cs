using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Source.Dialogue
{
    public class TextTyper : MonoBehaviour
    {
        [SerializeField]
        private CinemachineImpulseSource impulseSource;

        [SerializeField]
        private TextMeshProUGUI textMesh;

        [SerializeField]
        private UnityEvent onDialogueCompleted = new();
        public UnityEvent OnDialogueCompleted => onDialogueCompleted;

        [SerializeField]
        private UnityEvent onDialogueLineBegin = new();
        public UnityEvent OnDialogueLineBegin => onDialogueLineBegin;

        [SerializeField]
        private UnityEvent onDialogueLineEnd = new();

        public UnityEvent OnDialogueLineEnd => onDialogueLineEnd;
        
        private Queue<string> _dataLines = new();

        // current action queue is the current line of dialogue / all the letters we need to type
        private DialogueActions _currentActionQueue;

        // current action is the single letter we are typing / command etc
        private IDialogueAction _currentAction;

        private StringBuilder _displayText = new();

        private float _typeElapsedTime = 0f;
        private float _typeDelay = 0.025f;

        public Avatar Avatar { get; private set; }
        
        private void Update()
        {
            if (IsTypingLetters())
            {
                var deltaTime = Time.deltaTime;
                _typeElapsedTime += deltaTime;

                if (_typeElapsedTime < _typeDelay)
                    return;

                _typeElapsedTime = 0;
            }
            else
            {
                _typeElapsedTime = 0f;
            }

            // we're done typing
            if (_currentAction is null)
                return;

            var completed = _currentAction.ExecuteUpdate(Time.deltaTime, _displayText, impulseSource);

            if (completed)
            {
                DequeueNextAction();
            }
            
            textMesh.SetText(_displayText.ToString());
        }
        
        public bool IsTypingLetters() => _currentAction is TypeCharAction;

        public bool IsTyping()
        {
            return _currentAction is not null;
        }

        public void CycleDialogue() //todo fix bug with thing ending early.
        {
            if (IsTyping())
            {
                // we are instantly done, bypass remaining actions
                textMesh.SetText(_currentActionQueue.FullDisplayText);
                _currentAction = null;
                _currentActionQueue.ActionQueue.Clear();
                onDialogueCompleted?.Invoke();
                return;
            }

            // we were not typing, so begin next line
            ParseNextLine();
            
            if (_currentActionQueue?.ActionQueue?.Any() == true)
                DequeueNextAction();

        }

        public void BeginPresentingDialogue(string filePath)
        {
            var fullPath = "Dialogue/en/" + filePath;
            
            var textAsset = Resources.Load<TextAsset>(fullPath);
            _dataLines = new Queue<string>(textAsset.text.Split("\n"));
            ParseNextLine();
            DequeueNextAction();
            gameObject.SetActive(true);
        }

        private void DequeueNextAction()
        {
            // pull next action from current action queue (or null)
            _currentActionQueue.ActionQueue.TryDequeue(out var nextAction);
            _currentAction = nextAction;

            // if we are done typing 
            if (!IsTyping())
                onDialogueLineEnd?.Invoke();
        }

        private void ParseNextLine()
        {
            _displayText.Clear();
            
            if (!_dataLines.TryDequeue(out var nextLine))
            {
                // completely done, do cleanup and go to sleep
                _dataLines = null;
                _currentAction = null;
                _currentActionQueue = null;
                
                onDialogueCompleted?.Invoke();
                gameObject.SetActive(false);
                return;
            }

            // skip comments
            // maybe update this to not do recursion
            if (nextLine.StartsWith("#"))
            {
                ParseNextLine();
                return;
            }

            // skip empty lines
            if (nextLine.Trim() == string.Empty)
            {
                ParseNextLine();
                return;
            }

            var nextLineParts = nextLine.Split('|');
            var avatarPart = nextLineParts[0];
            // todo: title as well
            if (!Enum.TryParse<Avatar>(avatarPart, out var avatar))
            {
                throw new FormatException($"Avatarid '{avatarPart}' does not exist.");
            }

            Avatar = avatar;
            var dialogueLine = nextLineParts[2];
            
            _currentActionQueue = DialogueActionFactory.ParseAndGenerateActionQueue(dialogueLine);
            
            onDialogueLineBegin?.Invoke();

        }
    }

}
