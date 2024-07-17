using System;
using UnityEngine;

namespace Source.Physics
{
    public class StaticRotation : MonoBehaviour
    {
        [SerializeField]
        private float minimumRotationSpeed = 1f;
        
        [SerializeField]
        private float maximumRotationSpeed = 4;

        private float rotationSpeed = 0f;

        private void Start()
        {
            rotationSpeed = UnityEngine.Random.Range(minimumRotationSpeed, maximumRotationSpeed);
        }

        private void Update()
        {
            transform.Rotate(Vector3.forward, rotationSpeed);
        }
    }
}