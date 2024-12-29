using System;
using System.Collections;
using Source.Actors;
using Source.Constants;
using UnityEngine;

namespace Source.UI
{
    public class AmmoDisplay : MonoBehaviour
    {
        [SerializeField]
        private GameObject _displayItemPrefab;

        [SerializeField]
        private float spread = 0.1f;

        private Player player;

        private BulletDisplayItem[] _displayItems;
        
        
        private void Start()
        {
            _displayItems = new BulletDisplayItem[GameplayConstants.MagSize];
            for (var i = 0; i < GameplayConstants.MagSize; i++)
            {
                var obj = Instantiate(_displayItemPrefab, transform);
                obj.transform.localPosition = new Vector3(i * spread, 0, 0);
                _displayItems[i] = obj.GetComponent<BulletDisplayItem>();
            }
            
            // center the whole display on the bottom
            var fullWidth = spread * GameplayConstants.MagSize;
            transform.localPosition = new Vector2(-fullWidth / 2, transform.localPosition.y);

            player = GameObject.FindWithTag(Tags.Player).GetComponent<Player>();
            player.OnActiveReloadEnd.AddListener(BeginReloadAnimation);
            player.OnPlayerShoot.AddListener(RefreshAmmoDisplay);
            player.OnManualReload.AddListener(TriggerManualReload);
        }

        private void TriggerManualReload()
        {
            foreach (var item in _displayItems)
            {
                item.Hide(-27f);
            }
        }

        private void RefreshAmmoDisplay()
        {
            var playersRemainingAmmo = player.RemainingBullets;
            _displayItems[playersRemainingAmmo].Hide(-27f);
        }

        private void BeginReloadAnimation()
        {
            StartCoroutine(ReloadAnimationCoroutine());
        }

        private IEnumerator ReloadAnimationCoroutine()
        {
            var i = 0;
            
            while (i < GameplayConstants.MagSize)
            {
                _displayItems[i].Show(0);
                i++;
                yield return new WaitForSeconds(0.05f);
            }
            
            // once the animation is done we hide all the bullets that were shot during
            // the animation in one big chonk
            for (var j=player.RemainingBullets; j < GameplayConstants.MagSize; j++)
            {
                _displayItems[j].Hide(-27f);
            }
        }

    }
}