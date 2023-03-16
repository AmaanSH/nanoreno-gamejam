using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nanoreno.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;

        public UIDocument document;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        public static UIDocument GetActiveUIDocument()
        {
            return instance.document;
        }

        public static VisualElement GetElement(string id)
        {
            return instance.document.rootVisualElement.Q(id);
        }
    }
}
