using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Source.Dialogue
{
    public class DialogueTyper : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI textMesh;

        [SerializeField]
        private TextMeshProUGUI titleTextMesh;

        [SerializeField]
        private CinemachineImpulseSource impulseSource;
        
        public string AvatarId { get; private set; }
        
        // events 
        public UnityEvent OnLineComplete { get; private set; } = new();
        public UnityEvent OnLineBegin { get; private set; } = new();
        public UnityEvent OnDialogueComplete { get; private set; } = new();
        
        private Queue<DialogueLine> _queue = new();
        private DialogueLine _currentLine;
        private int _charIndex = 0;
        private float _elapsedDelay = 0f;
        private bool _wasDoneTyping = false;
        
        private void Update()
        {
            if (_currentLine is null)
                return;
            
            // if already typed, do nothing until user presses enter
            if (IsDoneTypingCurrentLine())
            {
                if (_wasDoneTyping)
                    return;
                
                _wasDoneTyping = true;
                OnLineComplete?.Invoke();
                return;
            }

            // wait for delay
            _elapsedDelay += Time.deltaTime;

            if (_elapsedDelay < _currentLine.DelaySeconds)
                return;

            _elapsedDelay = 0f;

            var fullText = _currentLine.Line;
            
            // Type next character
            _charIndex++;

            var currentString = fullText[.._charIndex];
            textMesh.SetText(currentString);
        
        }

        /// <summary>
        /// User acknowledges the dialogue 
        /// </summary>
        public void UserCycleDialogue()
        {
            if (IsDoneTypingCurrentLine())
            {
                if (!_queue.Any())
                {
                    
                    _currentLine = null;
                    ResetParameters();
                    OnDialogueComplete?.Invoke();
                    //prevent update() from running
                    gameObject.SetActive(false);
                    
                    return;
                }

                textMesh.SetText(string.Empty);
                
                if (titleTextMesh is not null)
                    titleTextMesh.SetText(string.Empty);
                
                DequeueNextLine();
                return;
            }

            _charIndex = _currentLine.Line.Length;
            textMesh.SetText(_currentLine.Line);
        }

        private bool IsDoneTypingCurrentLine()
        {
            var fullText = _currentLine.Line;
            return _charIndex >= fullText.Length;
        }

        public void Present(List<DialogueLine> lines)
        {
            gameObject.SetActive(true);
            _queue = new Queue<DialogueLine>(lines);
            DequeueNextLine();
        }

        private void DequeueNextLine()
        {
            ResetParameters();
            _currentLine = _queue.Dequeue();

            foreach (var preEvent in _currentLine.PreEvents)
            {
                DoEvent(preEvent);
            }

            AvatarId = _currentLine.AvatarId;
            
            if (titleTextMesh is not null)  
                titleTextMesh.SetText(_currentLine.Title);

            OnLineBegin?.Invoke();
        }

        private void DoEvent(string ev)
        {
            switch (ev.Trim().ToLower())
            {
                case "shake":
                    impulseSource.GenerateImpulse();
                    break;
                default:
                    throw new FormatException($"Unrecognized eventId {ev}");
            }
        }

        public float CurrentTypingSpeed => _currentLine?.DelaySeconds ?? 0;

        private void ResetParameters()
        {
            _currentLine = null;
            _charIndex = 0;
            _elapsedDelay = 0f;
            _wasDoneTyping = false;
        }
    }
}