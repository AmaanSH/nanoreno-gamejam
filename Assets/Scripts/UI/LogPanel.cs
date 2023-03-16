using Nanoreno.UI.Builder;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nanoreno.UI
{
    public class LogPanel : MonoBehaviour
    {
        private UI uiPanel;
        private Button logButton;
        private ListView entryListView;

        private List<LogEntryBuilder> elements = new List<LogEntryBuilder>();

        private void Start()
        {
            uiPanel = new UI("logPanel");

            SetupEntryListView();

            logButton = UIManager.GetElement("logButton") as Button;
            logButton.clicked += Toggle;
        }

        private void SetupEntryListView()
        {
            entryListView = uiPanel.GetElement("textHolder") as ListView;

            Func<VisualElement> makeItem = () => new LogEntryBuilder().Build();
            Action<VisualElement, int> bindItem = (e, i) =>
            {
                e.Q<TextElement>("logEntryCharacterText").text = elements[i].Text;
                e.Q<Image>("logEntryCharacterSprite").sprite = elements[i].Sprite;

                if (!string.IsNullOrEmpty(elements[i].CharacterName))
                {
                    e.Q<TextElement>("logEntryCharacterName").style.display = DisplayStyle.Flex;
                    e.Q<TextElement>("logEntryCharacterName").text = elements[i].CharacterName;
                }
                else
                {
                    e.Q<TextElement>("logEntryCharacterText").style.unityTextAlign = TextAnchor.MiddleLeft;
                    e.Q<TextElement>("logEntryCharacterName").style.display = DisplayStyle.None;
                }
            };

            entryListView.makeItem = makeItem;
            entryListView.bindItem = bindItem;
            entryListView.itemsSource = elements;

            entryListView.style.flexGrow = 1.0f;
        }

        private void OnDisable()
        {
            logButton.clicked -= Toggle;
        }

        private void Toggle()
        {
            if (uiPanel.Element.style.display == DisplayStyle.Flex)
            {
                HidePanel();
            }
            else
            {
                ShowPanel();
            }
        }

        private void ShowPanel()
        {
            uiPanel.Element.style.display = DisplayStyle.Flex;
        }

        private void HidePanel()
        {
            uiPanel.Element.style.display = DisplayStyle.None;
        }

        public void AddEntry(string characterName, string text, Sprite sprite)
        {
            LogEntryBuilder entry = CreateEntry(characterName, text, sprite);
            elements.Add(entry);
            entryListView.RefreshItems();
        }

        private LogEntryBuilder CreateEntry(string characterName, string text, Sprite sprite)
        {
            return new LogEntryBuilder()
                .SetName(characterName)
                .SetText(text)
                .SetSprite(sprite);
        }
    }
}
