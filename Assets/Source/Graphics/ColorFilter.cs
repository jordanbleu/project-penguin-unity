using System;
using Source.Data;
using Source.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Source.Graphics
{
    public class ColorFilter : MonoBehaviour
    {
        private void Start()
        {
            Refresh();
        }


        public void UpdateColorFilterSetting(int optAsInt)
        {
            // need to use int or cannot call this from Unityevents fml
            GameSettings.ColorFilterOption = (ColorFilterOption)optAsInt;
            Refresh();
        }

        private void Refresh()
        {
            var img = GetComponent<Image>();
            img.color = GameSettings.ColorFilterOption.ToUnityColor();
        }
    }
}