using Source.Actors;
using Source.Constants;
using Source.UI;
using UnityEngine;

namespace Source.Director
{
    public class UnlockPlayerGameInstruction : GameInstruction
    {
        [SerializeField]
        [Tooltip("Optional - Health bar to show")]
        private ShowHide healthBar;

        [SerializeField]
        [Tooltip("Optional - Health bar to show")]
        private ShowHide energyBar;
        
        public override void InstructionBegin()
        {
            var player = GameObject.FindWithTag(Tags.Player);
            
#if UNITY_EDITOR
            if (player == null)
                Debug.LogError("Cannot find the player, make sure the player has the tag 'Player'");
#endif
            
            // show the health and energy bars
            if (healthBar != null && healthBar.TryGetComponent<ShowHide>(out var healthBarComponent))
            {
                healthBarComponent.Show();
            }
            
            if (energyBar != null && energyBar.TryGetComponent<ShowHide>(out var energyBarComponent))
            {
                energyBarComponent.Show();
            }
            
            player.GetComponent<Player>().SetDialogueMode(false);
            base.InstructionBegin();
        }

        public override bool IsInstructionComplete() => true;
    }
}