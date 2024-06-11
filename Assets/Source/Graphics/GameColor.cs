using System.Collections.Generic;
using UnityEngine;

namespace Source.Graphics
{
    public enum GameColor
    {
        BloodRed,
        SalmonRed,
        MelonRed,

        SexyPink,
        JustPink,
        PiggyPink,

        GrimacePurple,
        LavenderPurple,
        BoldPurple,

        BabyBleu,
        WaterBlue,
        CookieMonsterBlue,

        Aquafresh,
        NeonBlueGreen,
        AlienGreen,
        PaleGreen,
        ArmyGreen,
        GlowInTheDarkGreen,
        
        SlightlyOffYellow,
        SchoolBusYellow,
        HighlighterYellow,
        MustardYellow,
        
        LightOrange,
        RegularOrange,
        BloodOrange,
    }
    
    public static class GameColorUtils
    {
        /// <summary>
        /// Specific colors that are supported in the game, designed to look awesome with any color filter.  This can be used
        /// to randomize particles, enemies, etc.
        /// </summary>
        public static Dictionary<GameColor, string> ColorCodes => new Dictionary<GameColor, string>
        {
            { GameColor.BloodRed, "#CF0303" },
            { GameColor.SalmonRed, "#B7392A" },
            { GameColor.MelonRed, "#FF002D" },


            { GameColor.SexyPink, "#C82186" },
            { GameColor.JustPink, "#FF00D2" },
            { GameColor.PiggyPink, "#D969C5" },

            { GameColor.GrimacePurple, "#7C29C3" },
            { GameColor.LavenderPurple, "#D4A5FF" },
            { GameColor.BoldPurple, "#8900FF" },

            { GameColor.BabyBleu, "#314ED4" },
            { GameColor.WaterBlue, "#288DD9" },
            { GameColor.CookieMonsterBlue, "#4191FA" },

            { GameColor.Aquafresh, "#41A5C1"},
            { GameColor.NeonBlueGreen, "#02E7E7" },
            { GameColor.AlienGreen, "#00FFB3" },
            
            { GameColor.PaleGreen, "#6CF58F" },
            { GameColor.ArmyGreen, "#4D935F" },
            { GameColor.GlowInTheDarkGreen, "#44FF00" },

            { GameColor.SlightlyOffYellow, "#AFD932" },
            { GameColor.SchoolBusYellow, "#D9D532" },
            { GameColor.HighlighterYellow, "#F1FF00" },
            { GameColor.MustardYellow, "#B78500" },

            { GameColor.LightOrange, "#FF8100" },
            { GameColor.RegularOrange, "#C34908" },
            { GameColor.BloodOrange, "#B42500" },
            
        };
        

        public static Color GetRandomUnityColor()
        {
            var randomHex = ColorCodes[(GameColor)UnityEngine.Random.Range(0, ColorCodes.Count)];
            return ColorUtility.TryParseHtmlString(randomHex, out var unityColor) ? unityColor : Color.white;
        }





    }


}