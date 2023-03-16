using Nanoreno.UI.Builder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nanoreno.UI
{
    public enum PanelMode
    {
        Save,
        Load
    }

    public class SaveLoadPanel : MonoBehaviour
    {
        public string saveOpenId;
        public string loadOpenId;
        public PanelMode panelMode;

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

            Button openButtonSave = document.rootVisualElement.Q(saveOpenId) as Button;
            Button openButtonLoad = document.rootVisualElement.Q(loadOpenId) as Button;

            Button closeButton = uiPanel.GetElement("backButton") as Button;

            openButtonSave.clicked += () => SetPanelMode(PanelMode.Save);
            openButtonLoad.clicked += () => SetPanelMode(PanelMode.Load);

            uiPanel.SetOpenButton(openButtonSave);
            uiPanel.SetOpenButton(openButtonLoad);

            uiPanel.SetCloseButton(closeButton);

            Setup();
        }

        private void SetPanelMode(PanelMode mode)
        {
            panelMode = mode;

            Label title = uiPanel.GetElement("titleLabel") as Label;
            title.text = GetTitle();
        }

        public void Setup()
        {
            VisualElement saveSlotHolder = uiPanel.GetElement("saveGrid");
            saveSlotHolder.Clear();

            for (int i = 0; i < saveSlotCount; i++)
            {
                int slotNumber = i + 1;
                VisualElement slot = new SlotBuilder(saveSlot)
                    .SetSlotNumber(slotNumber)
                    .SetChapterNumber(0)
                    .SetChapterName("")
                    .Build();

                Button button = slot.Q("slotButton") as Button;
                button.clicked += () => SlotClicked(slotNumber);

                saveSlotHolder.Add(slot);
            }
        }

        private string GetTitle()
        {
            switch(panelMode)
            {
                case PanelMode.Load:
                    return "Load";
                case PanelMode.Save:
                    return "Save";
            }

            return "";
        }

        private void SlotClicked(int index)
        {
            Debug.Log(index);
        }
    }
}
