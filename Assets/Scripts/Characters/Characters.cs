using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

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
            return !string.IsNullOrEmpty(characterName) ? characterName : "No Name";
        }

        public Sprite GetSprite()
        {
            return sprite;
        }
    }
}
