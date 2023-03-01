using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanoreno.Dialogue
{
    [System.Serializable]
    public class DialogueNode
    {
        public string uniqueID;
        public string text;
        public string[] children;
        public Rect position;
    }
}
