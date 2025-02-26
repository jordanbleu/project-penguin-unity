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
                ColorFilterOption.Midnight => new Color32(26, 27, 58, 73),//
                ColorFilterOption.Aliens => new Color32(31, 53, 31, 154),//
                ColorFilterOption.Bleu => new Color32(17, 0, 255, 170), //
                ColorFilterOption.Scorched => new Color32(80, 29, 34, 121),//
                ColorFilterOption.FadedGrey => new Color32(17, 17, 17, 170),//
                ColorFilterOption.Sepia => new Color32(85, 37, 22, 112),
                _ => new Color32(35, 37, 46, 47)
            };
        }
    }
}