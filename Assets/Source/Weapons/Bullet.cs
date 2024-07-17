using System;
using Source.Interfaces;
using Source.Physics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Source.Weapons
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [FormerlySerializedAs("constantForce2D")]
        [SerializeField] private StaticVelocity staticVelocity;
        [SerializeField] private Collider2D attachedCollider;
        [SerializeField] private Rigidbody2D rigidBody;
        
        private static readonly int DestroyTrigger = Animator.StringToHash("destroy");

        public void Ricochet()
        {
            // if you add a static rotation component to the bullet it will enable when the bullet ricochets   
            if (gameObject.TryGetComponent<StaticRotation>(out var staticRotation))
            {
                staticRotation.enabled = true;
            }
            
            var randomXVel = UnityEngine.Random.Range(-12, 12);

            var absYVel = Math.Abs(staticVelocity.Velocity.y);
            
            staticVelocity.Velocity =  new Vector2(randomXVel, -(absYVel*1));
            rigidBody.velocity =  new Vector2(randomXVel, -(absYVel*1));
            attachedCollider.enabled = false;
        }
        

        public void HitSomething()
        {
            rigidBody.velocity = Vector2.zero;
            attachedCollider.enabled = false;
            animator.SetTrigger(DestroyTrigger);
            staticVelocity.Velocity = Vector2.zero;
        }

    }
}