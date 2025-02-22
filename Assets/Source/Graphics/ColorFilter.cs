using System;
using Source.Data;
using Source.Extensions;
using Source.GameData;
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
            GlobalSaveDataManager.GlobalData.ColorFilter = (ColorFilterOption)optAsInt;
            Refresh();
            // NOTE - Saving the setting isn't done here, you need to do that somewhere else lol 
        }

        public void Refresh()
        {
            var filter = GlobalSaveDataManager.GlobalData.ColorFilter;
            var img = GetComponent<Image>();
            img.color = filter.ToUnityColor();
        }
    }
}