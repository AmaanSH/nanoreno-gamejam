using Nanoreno.UI.Builder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nanoreno.UI
{
    public class SavePanel : MonoBehaviour
    {
        public string openCloseButtonId;

        public VisualTreeAsset savePanel;
        public VisualTreeAsset saveSlot;
        public int saveSlotCount = 4;

        private UI uiPanel;

        private void Start()
        {
            uiPanel = new UI(savePanel.CloneTree());
            uiPanel.Element.style.width = Length.Percent(100);
            uiPanel.Element.style.height = Length.Percent(100);

            UIDocument document = UIManager.GetActiveUIDocument();

            Button openButton = document.rootVisualElement.Q(openCloseButtonId) as Button;

            uiPanel.SetOpenButton(openButton);
            uiPanel.SetCloseButton(uiPanel.GetElement("backButton") as Button);

            Setup();
        }

        public void Setup()
        {
            VisualElement saveSlotHolder = uiPanel.GetElement("saveGrid");
            Label title = uiPanel.GetElement("titleLabel") as Label;
            title.text = "Save";

            saveSlotHolder.Clear();

            for (int i = 0; i < saveSlotCount; i++)
            {
                VisualElement slot = new SlotBuilder(saveSlot)
                    .SetSlotNumber(i + 1)
                    .SetChapterNumber(0)
                    .SetChapterName("")
                    .Build();

                saveSlotHolder.Add(slot);
            }
        }
    }
}
