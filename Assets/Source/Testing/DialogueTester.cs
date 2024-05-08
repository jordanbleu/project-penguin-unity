using System;
using System.IO;
using Source.Dialogue;
using Source.UI;
using UnityEngine;

namespace Source.Testing
{
    public class DialogueTester : MonoBehaviour
    {
        [SerializeField]
        private InGameDialoguePresenter w;
        
        private void OnEnable()
        {
            w.PresentDialogue("example");
        }
    }
}