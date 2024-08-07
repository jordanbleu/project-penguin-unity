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
            var percent = (float)attackable.Health / attackable.MaxHealth;
            bar.SetValue(percent);
        }
    }
}