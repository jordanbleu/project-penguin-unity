using System.Collections.Generic;

namespace Source.Dialogue
{
    public static class DialogueParser
    {
        private const float DefaultTypeSpeed = 0.03f;
        public static DialogueLine ParseDialogueLine(string line)
        {
            var lineParts = line.Split("|");
            var avatarId = lineParts[0];
            var title = lineParts[1];
            var text = lineParts[2];

            var speed = DefaultTypeSpeed;

            if (lineParts.Length > 3)
            {
                speed = float.Parse(lineParts[3]);
            }

            var preEvents = new string[] { };
            
            if (lineParts.Length > 4)
            {
                preEvents = lineParts[4].Split(',');
            }

            return new()
            {
                AvatarId = avatarId,
                Title = title,
                Line = text,
                DelaySeconds = speed,
                PreEvents = preEvents
            };
        }
    }
}