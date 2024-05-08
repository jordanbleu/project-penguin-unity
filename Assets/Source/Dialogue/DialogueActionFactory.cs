using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Source.Dialogue
{
    public static class DialogueActionFactory
    {
        /// <summary>
        /// parses a line of dialogue from the .d file into separate dialogue typing actions
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static DialogueActions ParseAndGenerateActionQueue(string line)
        {
            var charIndex = 0;
            var q = new Queue<IDialogueAction>();
            var displayText = new StringBuilder();
            
            while (charIndex < line.Length)
            {
                var currentChar = line[charIndex];

                // begin interpreting the action
                if (currentChar == '[')
                {
                    var cmdBuilder = new StringBuilder();
                    // skip the [
                    charIndex++;
                    
                    while (currentChar != ']')
                    {
                        if (charIndex > line.Length)
                            throw new FormatException($"[ without a matching ]. '{line}'");
                        
                        currentChar = line[charIndex];

                        cmdBuilder.Append(currentChar);

                        charIndex++;
                    }

                    if (cmdBuilder.Length == 0)
                        throw new FormatException($"Square brackets without inner command text. {line}");

                    // command text is a string containing everything in the square brackets (minus the right bracket)
                    var commandText = cmdBuilder.ToString().Substring(0, cmdBuilder.Length - 1);

                    var cmdParts = commandText.Split(",");
                    
                    var cmd = cmdParts.First();
                    var args = cmdParts.Skip(1);

                    q.Enqueue(CreateSpecialAction(cmd, args));
 
                }
                else
                {
                    // assume we are just typing a single char
                    q.Enqueue(new TypeCharAction(currentChar));
                    displayText.Append(currentChar);
                    charIndex++;
                }
            }

            return new()
            {
                ActionQueue = q,
                FullDisplayText = displayText.ToString()
            };
        }

        private static IDialogueAction CreateSpecialAction(string cmd, IEnumerable<string> args)
        {
            switch (cmd.Trim().ToLower())
            {
                case "delay":
                    if (!args.Any())
                        throw new FormatException("delay command requires a time parameter");
                    
                    var firstArg = args.First();

                    if (!float.TryParse(firstArg, out var time))
                        throw new FormatException($"Unable to parse arg as float: {firstArg}");
                    
                    return new DelayAction(time);
                case "shake":
                    return new ScreenShakeAction();
                default:
                    throw new FormatException($"Unknown dialogue action: {cmd}");
            }

        }
    }
}