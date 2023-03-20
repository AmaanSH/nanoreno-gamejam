using Nanoreno.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nanoreno.Dialogue
{
    [System.Serializable]
    public class CharacterPosition
    {
        public Character character;
        public SpritePosition screenPosition;
        public int customPercentage;
    }

    public class ControlNode : ScriptableObject
    {
        [SerializeField]
        Rect rect = new Rect(0, 0, 200, 300);

        public List<CharacterPosition> characterPositions = new List<CharacterPosition>();

        public Sprite backgroundImage;

        public AudioClip BGM;
        public AudioClip SFX;

        public IEnumerable<CharacterPosition> GetCharacterPositions()
        {
            return characterPositions;
        }

        public Rect GetRect()
        {
            return rect;
        }

#if UNITY_EDITOR
        public void SetPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Move Control Node");
            rect.position = newPosition;
            EditorUtility.SetDirty(this);
        }

        public void SetHeight(float ySize)
        {
            rect.height = ySize;
        }

        public void AddCharacterPosition(CharacterPosition characterPos)
        {
            Undo.RecordObject(this, "Add Character Position");
            characterPositions.Add(characterPos);
            EditorUtility.SetDirty(this);
        }

        public void RemoveCharacterPosition(CharacterPosition characterPos)
        {
            Undo.RecordObject(this, "Removed Character Position");
            characterPositions.Remove(characterPos);
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
