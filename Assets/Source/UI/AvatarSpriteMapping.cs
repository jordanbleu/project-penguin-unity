using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Source.UI
{
    /// <summary>
    /// Use this to map each avatar id with which sprite frames to show.
    /// </summary>
    public class AvatarSpriteMapping : MonoBehaviour
    {
        [SerializeField]
        private List<Mapping> mappings;

        public Dictionary<string, Sprite[]> ToDictionary()
        {
            return mappings.ToDictionary(m => m.AvatarId, m => m.Sprites);
        }

        [Serializable]
        public class Mapping
        {
            [SerializeField]
            private string avatarId;
            public string AvatarId => avatarId;


            [SerializeField]
            private Sprite[] sprites;
            public Sprite[] Sprites => sprites;

        }
    }
}