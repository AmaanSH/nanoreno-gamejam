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

        public UIDocument document;

        private string text;
        private bool skipped = false;
        private float typingSpeed = 0.02f;

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
            narrativePanel = document.rootVisualElement.Q("narrativePanel");

            choicePanel = document.rootVisualElement.Q("choicePanel");
            choiceButtonHolder = document.rootVisualElement.Q("choiceButtonHolder");

            textElement = document.rootVisualElement.Q("characterText") as TextElement;
            characterNameElement = document.rootVisualElement.Q("characterName") as TextElement;
            characterSprite = document.rootVisualElement.Q("charSprite");

            skipElement = document.rootVisualElement.Q("skip");
            skipButton = document.rootVisualElement.Q("skipButton") as Button;

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
            Cleanup();
            
            narrativePanel.style.display = DisplayStyle.None;
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
            ChoiceMade?.Invoke(node);
        }

        public void SetText(string text)
        {
            Cleanup();

            this.text = text;
        }

        private void Cleanup()
        {
            choicePanel.style.display = DisplayStyle.None;
            narrativePanel.style.display = DisplayStyle.Flex;

            textElement.text = "";
            skipElement.style.visibility = Visibility.Hidden;
            skipped = false;
        }

        public void SetCharacterName(string name)
        {
            characterNameElement.text = name;
        }

        public void SetCharacterSprite(Sprite sprite)
        {
            characterSprite.style.backgroundImage = new StyleBackground(sprite);
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
