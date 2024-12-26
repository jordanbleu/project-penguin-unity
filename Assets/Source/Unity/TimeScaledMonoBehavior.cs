using System;
using Source.Data;
using UnityEngine;

namespace Source.Unity
{
    /// <summary>
    /// Wrapper class around unity's normal MonoBehaviour class.
    /// If your component extends this class instead of the MonoBehaviour class, it will
    /// be affected by the 'TimeDelaySeconds' property in the gameplay stats -> <see cref="Stats"/>.
    ///
    /// 
    /// </summary>
    public class TimeScaledMonoBehavior : MonoBehaviour
    {
        private float _timeDelaySeconds;
        
        public virtual void Update()
        {
            if (_timeDelaySeconds < Stats.Current.TimeDelaySeconds)
            {
                
            }

        }
    }
}