using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanoreno.Characters
{
    [CreateAssetMenu(fileName = "New Character", menuName = "Nanoreno/Characters/Create Character", order = 0)]
    public class Character : ScriptableObject
    {
        [SerializeField]
        string characterName;

        [SerializeField]
        Sprite sprite;

        // TODO: emotion data
        public string GetName()
        {
            return characterName;
        }

        public Sprite GetSprite()
        {
            return sprite;
        }
    }
}
