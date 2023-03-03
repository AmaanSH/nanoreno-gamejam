using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Nanoreno.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField]
        string text;
        [SerializeField]
        List<string> children = new List<string>();
        [SerializeField]
        Rect rect = new Rect(0, 0, 200, 100);

        public string GetText()
        {
            return text;
        }

        public List<string> GetChildren()
        {
            return children;
        }

        public Rect GetRect()
        {
            return rect;
        }

#if UNITY_EDITOR
        public void SetPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            rect.position = newPosition;
        }

        public void SetText(string newText)
        {
            if (newText != text)
            {
                Undo.RecordObject(this, "Update Dialogue Text");
                text = newText;
            }
        }

        public void AddChild(string child)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            children.Add(child);
        }

        public void RemoveChild(string child)
        {
            Undo.RecordObject(this, "Remove Dialogue Link");
            children.Remove(child);
        }
#endif
    }
}
