using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nanoreno.UI
{
    public class UI
    {
        public VisualElement Element { get; private set; }

        public UI(string id)
        {
            Element = UIManager.GetElement(id);
        }

        public VisualElement GetElement(string id)
        {
            return Element.Q(id);
        }
    }
}
