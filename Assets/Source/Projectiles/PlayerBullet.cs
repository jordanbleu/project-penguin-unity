using UnityEditor.SceneManagement;
using UnityEngine;

namespace Source.Projectiles
{
    public class PlayerBullet : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private ConstantForce2D constantForce;
        
        private static readonly int DestroyTrigger = Animator.StringToHash("destroy");

        public void HitSomething()
        {
            animator.SetTrigger(DestroyTrigger);
            constantForce.relativeForce = Vector2.zero;
        }

    }
}