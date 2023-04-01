using Nanoreno.Game;
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

        public VisualTreeAsset savePanel;
        public VisualTreeAsset saveSlot;
        public int saveSlotCount = 4;

        private UIHolder uiPanel;
        private PanelMode panelMode;

        private void Start()
        {
            uiPanel = new UIHolder(savePanel.CloneTree());
            uiPanel.Element.style.width = Length.Percent(100);
            uiPanel.Element.style.height = Length.Percent(100);

            UIDocument document = UIManager.GetActiveUIDocument();

            if (!string.IsNullOrEmpty(saveOpenId))
            {
                Button openButtonSave = document.rootVisualElement.Q(saveOpenId) as Button;
                openButtonSave.clicked += () => SetPanelMode(PanelMode.Save);
                uiPanel.SetOpenButton(openButtonSave);
            }
            if (!string.IsNullOrEmpty(loadOpenId))
            {
                Button openButtonLoad = document.rootVisualElement.Q(loadOpenId) as Button;
                openButtonLoad.clicked += () => SetPanelMode(PanelMode.Load);
                uiPanel.SetOpenButton(openButtonLoad);
            }

            Button closeButton = uiPanel.GetElement("backButton") as Button;
            uiPanel.SetCloseButton(closeButton);
        }

        private void SetPanelMode(PanelMode mode)
        {
            panelMode = mode;

            Label title = uiPanel.GetElement("titleLabel") as Label;
            title.text = GetTitle();

            Setup();
        }

        public void Setup()
        {
            VisualElement saveSlotHolder = uiPanel.GetElement("saveGrid");
            saveSlotHolder.Clear();

            for (int i = 0; i < saveSlotCount; i++)
            {
                int slotNumber = i + 1;
                SlotBuilder slot = new SlotBuilder(saveSlot);

                // find data for this
                int chapter = PlayerPrefs.GetInt($"save-{slotNumber}-chapter", -1);
                if (chapter > -1)
                {
                    slot
                        .SetSlotNumber(slotNumber)
                        .SetChapterNumber(chapter + 1)
                        .SetChapterName(PlayerPrefs.GetString($"save-{slotNumber}-chapterDialogueName"));
                }
                else
                {
                    slot
                        .SetSlotNumber(slotNumber)
                        .SetChapterNumber(0)
                        .SetChapterName("");
                }

                VisualElement slotVisualElement = slot.Build();

                if (panelMode == PanelMode.Save || (panelMode == PanelMode.Load && chapter > -1))
                {
                    Button button = slotVisualElement.Q("slotButton") as Button;
                    button.clicked += () => SlotClicked(slotNumber);
                }

                saveSlotHolder.Add(slotVisualElement);
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
            switch(panelMode)
            {
                case PanelMode.Save:
                    SaveManager.instance.SaveProgress(index);
                    Setup();
                    break;
                case PanelMode.Load:
                    SaveManager.instance.LoadProgress(index);
                    uiPanel.HidePanel();
                    break;
            }
        }
    }
}
