using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Source.Dummy
{
    [Tooltip("This is an absolute dummy class that just displays info for prefabs so i don't remember what needs to be done.")]
    public class PrefabChecklist : MonoBehaviour
    {
        [TextArea]
        [SerializeField]
        private string Description = string.Empty;
        
        
        [SerializeField]
        private List<string> todoItems = new List<string>();

    }
}