using UnityEngine;

namespace Source.Weapons
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private ConstantForce2D constantForce2D;
        [SerializeField] private Collider2D attachedCollider;
        [SerializeField] private Rigidbody2D rigidBody;
        
        private static readonly int DestroyTrigger = Animator.StringToHash("destroy");

        public void Ricochet()
        {
            var randomXVel = UnityEngine.Random.Range(-6, 6);
            rigidBody.velocity = new Vector2(randomXVel, -(rigidBody.velocity.y*5));
            constantForce2D.relativeForce = Vector2.zero;
            constantForce2D.force = Vector2.zero;
            attachedCollider.enabled = false;
        }
        

        public void HitSomething()
        {
            rigidBody.velocity = Vector2.zero;
            attachedCollider.enabled = false;
            animator.SetTrigger(DestroyTrigger);
            constantForce2D.relativeForce = Vector2.zero;
        }

    }
}