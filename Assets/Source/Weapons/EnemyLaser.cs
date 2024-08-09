using Source.Actors;
using Source.Interfaces;
using UnityEngine;

namespace Source.Weapons
{
    public class EnemyLaser : MonoBehaviour, ICollideWithPlayerResponder
    {
        public void CollideWithPlayer(Player playerComponent)
        {
            playerComponent.TakeDamage(20);
        }
    }
}