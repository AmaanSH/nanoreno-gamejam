using Nanoreno.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum SpritePosition
{
    None,
    FarLeft,
    Left,
    Center,
    Right,
    FarRight
}

namespace Nanoreno.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField]
        int characterIndex;

        [SerializeField]
        string text;

        [SerializeField]
        SpritePosition spritePositionOnScreen;

        [SerializeField]
        List<string> children = new List<string>();

        [SerializeField]
        Rect rect = new Rect(0, 0, 200, 200);

        public string GetText()
        {
            return text;
        }

        public int GetCharacterIndex()
        {
            return characterIndex;
        }

        public List<string> GetChildren()
        {
            return children;
        }

        public Rect GetRect()
        {
            return rect;
        }

        public SpritePosition GetSpritePosition()
        {
            return spritePositionOnScreen;
        }

#if UNITY_EDITOR
        public void SetPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            rect.position = newPosition;
            EditorUtility.SetDirty(this);
        }

        public void SetSpritePosition(SpritePosition newSpritePosition)
        {
            Undo.RecordObject(this, "Update Sprite Position");
            spritePositionOnScreen = newSpritePosition;
            EditorUtility.SetDirty(this);
        }

        public void SetText(string newText)
        {
            if (newText != text)
            {
                Undo.RecordObject(this, "Update Dialogue Text");
                text = newText;
                EditorUtility.SetDirty(this);
            }
        }

        public void AddChild(string child)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            children.Add(child);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(string child)
        {
            Undo.RecordObject(this, "Remove Dialogue Link");
            children.Remove(child);
            EditorUtility.SetDirty(this);
        }

        public void SetCharacter(int newCharacterIndex)
        {
            Undo.RecordObject(this, "Change Dialogue Speaker");
            characterIndex = newCharacterIndex;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
