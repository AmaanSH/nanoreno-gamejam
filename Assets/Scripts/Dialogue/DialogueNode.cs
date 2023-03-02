using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanoreno.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        public string text;
        public List<string> children = new List<string>();
        public Rect rect = new Rect(0, 0, 200, 100);
    }
}
