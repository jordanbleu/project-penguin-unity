using Source.Actors;
using Source.Constants;
using Source.UI;
using UnityEngine;

namespace Source.Director
{
    public class LockPlayerForDialogueInstruction : GameInstruction
    {
        [SerializeField]
        [Tooltip("Optional - Health bar to hide")]
        private ShowHide healthBar;

        [SerializeField]
        [Tooltip("Optional - Health bar to hide")]
        private ShowHide energyBar;

        [Tooltip("Optional - Score display to hide")]
        [SerializeField]
        private ShowHide score;
        
        public override void InstructionBegin()
        {
            var player = GameObject.FindWithTag(Tags.Player);
            
            #if UNITY_EDITOR
            if (player == null)
                Debug.LogError("Cannot find the player, make sure the player has the tag 'Player'");
            #endif
            
            player.GetComponent<Player>().SetDialogueMode(true);
            
            if (healthBar != null && healthBar.TryGetComponent<ShowHide>(out var healthBarComponent))
            {
                healthBarComponent.Hide();
            }
            
            
            if (energyBar != null && energyBar.TryGetComponent<ShowHide>(out var energyBarComponent))
            {
                energyBarComponent.Hide();
            }
            
            if (score != null && score.TryGetComponent<ShowHide>(out var scoreComponent))
            {
                scoreComponent.Hide();
            }
   
            
            base.InstructionBegin();
        }

        public override bool IsInstructionComplete() => true;
    }
}