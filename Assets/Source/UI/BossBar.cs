using System;
using Source.Behaviors;
using UnityEngine;

namespace Source.UI
{
    public class BossBar : MonoBehaviour
    {

        [SerializeField]
        private DisplayBar bar;
        
        [SerializeField]
        private Attackable attackable;

        private void Update()
        {
            // Happens when the gameobject is inactive.
            if (attackable.MaxHealth == 0)
                return;
            
            var percent = (float)attackable.Health / attackable.MaxHealth;
            
            
            bar.SetValue(percent);
        }
    }
}