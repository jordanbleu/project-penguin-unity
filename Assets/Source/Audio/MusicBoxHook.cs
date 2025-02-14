using UnityEngine;

namespace Source.Audio
{
    [Tooltip("Workaround for unity's shortcomings in regards to unity events and parameters")]
    public class MusicBoxHook : MonoBehaviour
    {
        [SerializeField]
        private MusicBox musicBox;
        
        [SerializeField]
        private AudioClip musicClip;
        
        public void PlayImmediate()
        {
            musicBox.PlayImmediate(musicClip);
        }
        
        
    }
}