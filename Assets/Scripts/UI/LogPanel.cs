using Nanoreno.UI.Builder;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nanoreno.UI
{
    public class LogPanel : MonoBehaviour
    {
        public VisualTreeAsset logEntryAsset;

        private UIHolder uiPanel;
        private Button logButton;
        private ListView entryListView;

        private List<LogEntryBuilder> elements = new List<LogEntryBuilder>();

        public void Setup()
        {
            uiPanel = new UIHolder("logPanel");

            SetupEntryListView();

            logButton = UIManager.GetElement("logButton") as Button;
            uiPanel.SetToggleButton(logButton);
        }

        private void SetupEntryListView()
        {
            entryListView = uiPanel.GetElement("textHolder") as ListView;

            Func<VisualElement> makeItem = () => logEntryAsset.CloneTree();
            Action<VisualElement, int> bindItem = (e, i) =>
            {
                if (elements.Count == 0) { return; }

                e.Q<Label>("logEntryCharacterText").text = elements[i].Text;

                if (!string.IsNullOrEmpty(elements[i].CharacterName))
                {
                    e.Q<Label>("logEntryCharacterName").style.display = DisplayStyle.Flex;
                    e.Q<Label>("logEntryCharacterName").text = elements[i].CharacterName;
                }
                else
                {
                    e.Q<Label>("logEntryCharacterText").style.unityTextAlign = TextAnchor.MiddleLeft;
                    e.Q<Label>("logEntryCharacterName").style.display = DisplayStyle.None;
                }
            };

            entryListView.makeItem = makeItem;
            entryListView.bindItem = bindItem;
            entryListView.itemsSource = elements;

            entryListView.style.flexGrow = 1.0f;
        }

        public void AddEntry(string characterName, string text, Sprite sprite)
        {
            LogEntryBuilder entry = CreateEntry(characterName, text, sprite);
            elements.Add(entry);
            entryListView.RefreshItems();
        }

        private LogEntryBuilder CreateEntry(string characterName, string text, Sprite sprite)
        {
            return new LogEntryBuilder(logEntryAsset)
                .SetName(characterName)
                .SetText(text);
        }
    }
}
