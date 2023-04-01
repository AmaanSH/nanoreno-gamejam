using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nanoreno.UI
{
    public class UIHolder
    {
        public VisualElement Element { get; private set; }

        private bool hideOnClose = false;
        private VisualElement holder;

        public UIHolder(string id)
        {
            Element = UIManager.GetElement(id);
        }

        public UIHolder(VisualElement element)
        {
            Element = element;
        }

        public void SetToggleButton(Button button)
        {
            hideOnClose = true;
            button.clicked += Toggle;
        }

        public void SetOpenButton(Button button)
        {
            button.clicked += ShowPanel;
        }

        public void SetCloseButton(Button button)
        {
            button.clicked += HidePanel;
        }

        public VisualElement GetElement(string id)
        {
            return Element.Q(id);
        }

        public void AddToHierarchy()
        {
            UIDocument root = UIManager.GetActiveUIDocument();

            // create new absolute positioned element
            holder = new VisualElement();
            holder.style.position = Position.Absolute;
            holder.style.width = Length.Percent(100);
            holder.style.height = Length.Percent(100);

            holder.Add(Element);

            root.rootVisualElement.Add(holder);
        }

        private void Toggle()
        {
            if (Element.style.display == DisplayStyle.Flex)
            {
                HidePanel();
            }
            else
            {
                ShowPanel();
            }
        }

        public void ShowPanel()
        {
            if (hideOnClose)
            {
                Element.style.display = DisplayStyle.Flex;
            }
            else
            {
                AddToHierarchy();
            }
        }

        public void HidePanel()
        {
            if (hideOnClose)
            {
                Element.style.display = DisplayStyle.None;
            }
            else
            {
                UIManager.GetActiveUIDocument().rootVisualElement.Remove(holder);
                holder = null;
            }
        }
    }
}
