using Source.Actors;
using UnityEngine;

namespace Source.Weapons
{
    public class PlayerShield : MonoBehaviour
    {
        [SerializeField]
        private Player player;

        public void EnableProtection() => player.ShieldProtectionEnabled = true;
        public void DisableProtection() => player.ShieldProtectionEnabled = false;

        


    }
}