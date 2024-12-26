using Source.Data;
using UnityEngine;

namespace Source.Extensions
{
    public static class ColorFilterOptionExtensions
    {
        public static Color ToUnityColor(this ColorFilterOption opt)
        {
            return opt switch
            {
                ColorFilterOption.Midnight => new Color32(20, 23, 36, 70),
                ColorFilterOption.Aliens => new Color32(20, 36, 21, 70),
                ColorFilterOption.Bleu => new Color32(31, 33, 43, 70),
                ColorFilterOption.Scorched => new Color32(41, 21, 23, 70),
                ColorFilterOption.FadedGrey => new Color32(38, 38, 38, 70),
                ColorFilterOption.Sepia => new Color32(55, 42, 32, 70),
                _ => new Color32(36, 31, 20, 70)
            };
        }
    }
}