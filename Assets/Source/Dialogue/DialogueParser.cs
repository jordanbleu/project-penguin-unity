using System;
using System.Collections.Generic;

namespace Source.Dialogue
{
    public static class DialogueParser
    {
        private const float DefaultTypeSpeed = 0.03f;
        public static DialogueLine ParseDialogueLine(string line)
        {
            var lineParts = line.Split("|");
            var id = lineParts[0];
            var avatarId = lineParts[1];
            var title = lineParts[2];
            var text = lineParts[3];

            var speed = DefaultTypeSpeed;

            
            if (lineParts.Length > 4 && !float.TryParse(lineParts[4], out speed))
            {
                throw new FormatException($"Unable to parse '{lineParts[4]}' as a float");
            }
            
            var preEvents = new string[] { };
            
            if (lineParts.Length > 5)
            {
                preEvents = lineParts[5].Split(',');
            }

            return new()
            {
                Identifier = id,
                AvatarId = avatarId,
                Title = title,
                Line = text,
                DelaySeconds = speed,
                PreEvents = preEvents
            };
        }
    }
}