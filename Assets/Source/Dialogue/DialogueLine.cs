namespace Source.Dialogue
{
    public class DialogueLine
    {
        public string Identifier { get; set; } = string.Empty;
        public string Line { get; set; }
        public string AvatarId { get; set; }
        public string Title { get; set; } = string.Empty;
        public float DelaySeconds { get; set; } = 0.02f;
        public string[] PreEvents { get; set; } = null;
    }
}