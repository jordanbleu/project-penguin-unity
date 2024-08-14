using System;
using Source.Constants;
using Source.Data;
using Source.Extensions;
using UnityEngine;

namespace Source.UI
{
    public class LivesDisplay : MonoBehaviour
    {
        // how much to spread out the life items
        private const float PositionOffset = -11f;
        
        [SerializeField]
        private GameObject _lifeDisplayItemPrefab;

        private GameObject[] _items;
        private static readonly int HideAnimatorParam = Animator.StringToHash("hide");

        private void Start()
        {
            _items = new GameObject[3];
            
            for (var i = 0; i < Stats.Current.LivesRemaining; i++)
            {
                var offset = i * PositionOffset;
                
                var inst = Instantiate(_lifeDisplayItemPrefab, transform)
                    .AtLocal(0, offset);
                _items[i] = inst;
            }
        }

        public void Refresh()
        {
            var remainingLives = Stats.Current.LivesRemaining;

            // index is zero based so this actually works out
            var lifeToRemove = remainingLives;
            
            // if some weirdo calls refresh but we still have all our lives, just return
            if (lifeToRemove > _items.Length)
                return;
            
            // get the animator of the life we want to remove and tell it to go away
            var animator = _items[lifeToRemove].GetComponent<Animator>();
            animator.SetTrigger(HideAnimatorParam);
        }
    }
}