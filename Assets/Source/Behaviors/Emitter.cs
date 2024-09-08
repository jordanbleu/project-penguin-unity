using System;
using Source.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace Source.Behaviors
{
    public class Emitter : MonoBehaviour
    {
        [SerializeField]
        private GameObject prefab;

        [SerializeField]
        private Vector2 relativePosition;

        public void Emit()
        {
            Instantiate(prefab).At((Vector2)transform.position + relativePosition);
        }
    }
}