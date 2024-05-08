using System.Collections.Generic;

namespace Source.Dialogue
{
    public class DialogueActions
    {
        public Queue<IDialogueAction> ActionQueue { get; set; }

        public string FullDisplayText { get; set; }
    }
}