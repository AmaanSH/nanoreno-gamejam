using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum SpritePosition
{
    None,
    Left,
    Centre,
    Right,
    Custom
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
        ControlNode controlNode;

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

        public ControlNode GetControlNode()
        {
            return controlNode;
        }

#if UNITY_EDITOR
        public void SetPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            rect.position = newPosition;
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

        public void SetControlNode(ControlNode controlNode)
        {
            Undo.RecordObject(this, "Add Control Node");
            this.controlNode = controlNode;
            EditorUtility.SetDirty(this);
        }

        public void RemoveControlNode()
        {
            Undo.RecordObject(this, "Removed Control Node");
            Undo.DestroyObjectImmediate(controlNode);
            controlNode = null;
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
