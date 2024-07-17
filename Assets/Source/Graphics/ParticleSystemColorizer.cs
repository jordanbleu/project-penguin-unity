using System;
using UnityEngine;

namespace Source.Graphics
{
    public class ParticleSystemColorizer : MonoBehaviour
    {
        private void Start()
        {
            var particleSystem = GetComponent<ParticleSystem>();
            var main = particleSystem.main;
            main.startColor = GameColorUtils.GetRandomUnityColor();
            particleSystem.Play();
        }
    }
}