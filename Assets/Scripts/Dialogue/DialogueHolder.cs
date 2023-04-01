using System.Collections.Generic;
using UnityEngine;

namespace Nanoreno.Dialogue
{
    [CreateAssetMenu(fileName = "New Chapter", menuName = "Nanoreno/Dialogue/Create Chapter", order = 1)]
    public class DialogueHolder : ScriptableObject
    {
        public List<Chapter> dialogues;
    }
}
