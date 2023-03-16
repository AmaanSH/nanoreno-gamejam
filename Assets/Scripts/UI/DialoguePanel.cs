using Nanoreno.Dialogue;
using Nanoreno.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nanoreno.UI
{
    public class DialoguePanel : MonoBehaviour
    {
        public event Action TextEndReached;
        public event Action<DialogueNode> ChoiceMade;

        private string text;
        private bool skipped = false;
        private float typingSpeed = 0.02f;

        private UI uiPanel;

        private TextElement textElement;
        private TextElement characterNameElement;
        private VisualElement characterSprite;
        private VisualElement skipElement;
        private Button skipButton;

        private VisualElement narrativePanel;
        private VisualElement choicePanel;
        private VisualElement choiceButtonHolder;

        private const string CHOICE_BUTTON_CLASS = "choiceButton";

        private void Start()
        {
            uiPanel = new UI("narrativePanel");

            narrativePanel = uiPanel.GetElement("narrativePanel");

            choicePanel = uiPanel.GetElement("choicePanel");
            choiceButtonHolder = uiPanel.GetElement("choiceButtonHolder");

            textElement = uiPanel.GetElement("characterText") as TextElement;
            characterNameElement = uiPanel.GetElement("characterName") as TextElement;

            skipElement = uiPanel.GetElement("skip");
            skipButton = uiPanel.GetElement("skipButton") as Button;

            skipButton.clicked += Skip;
        }

        private void OnDisable()
        {
            if (skipButton != null)
            {
                skipButton.clicked -= Skip; 
            }
        }

        public void SetupChoices(List<DialogueNode> choices)
        {
            skipElement.style.visibility = Visibility.Hidden;
            skipped = false;

            choiceButtonHolder.Clear();

            foreach(DialogueNode node in choices)
            {
                Button button = new Button();
                button.text = node.GetText();
                button.AddToClassList(CHOICE_BUTTON_CLASS);

                button.clicked += () => OnChoiceButtonClicked(node);

                choiceButtonHolder.Add(button);
            }

            choicePanel.style.display = DisplayStyle.Flex;
        }

        private void OnChoiceButtonClicked(DialogueNode node)
        {
            choicePanel.style.display = DisplayStyle.None;

            ChoiceMade?.Invoke(node);
        }

        public void SetText(string text)
        {
            Cleanup();

            this.text = text;
        }

        private void Cleanup()
        {
            textElement.text = "";
            skipElement.style.visibility = Visibility.Hidden;
            skipped = false;
        }

        public void SetCharacterName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                characterNameElement.style.display = DisplayStyle.None;
            }
            else
            {
                characterNameElement.style.display = DisplayStyle.Flex;
            }

            characterNameElement.text = name;
        }

        public IEnumerator Type()
        {
            foreach (char letter in text.ToCharArray())
            {
                if (skipped)
                {
                    break;
                }

                textElement.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            skipped = true;
            textElement.text = text;
            skipElement.style.visibility = Visibility.Visible;
        }

        public void Skip()
        {
            if (!skipped)
            {
                skipped = true;
            }
            else
            {
                OnTextEndReached();
            }
        }

        public void OnTextEndReached()
        {
            TextEndReached?.Invoke();
        }
    }
}
