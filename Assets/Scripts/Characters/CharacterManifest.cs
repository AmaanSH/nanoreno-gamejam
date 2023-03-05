using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanoreno.Characters
{
    [CreateAssetMenu(fileName = "New Character Manifest", menuName = "Nanoreno/Characters/Create Character Manifest", order = 1)]
    public class CharacterManifest : ScriptableObject
    {
        [SerializeField]
        Character[] characters;

        public IEnumerable<Character> GetCharacters()
        {
            return characters;
        }

        public Character GetCharacterByIndex(int index)
        {
            return characters[index];
        }
    }
}
