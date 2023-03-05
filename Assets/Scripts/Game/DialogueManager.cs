using Nanoreno.Characters;
using Nanoreno.Dialogue;
using Nanoreno.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nanoreno.Game
{
    public class DialogeManager : MonoBehaviour
    {
        public event Action OnChapterEnd;
        public DialoguePanel dialoguePanel;

        [SerializeField]
        private CharacterManifest characterManifest;

        private Chapter chapter;
        private DialogueNode currentNode;

        private bool showChoicesOnNextPrompt;

        private void Start()
        {
            dialoguePanel.TextEndReached += Next;
            dialoguePanel.ChoiceMade += OnChoiceMade;
        }

        private void OnDisable()
        {
            dialoguePanel.TextEndReached -= Next;
            dialoguePanel.ChoiceMade -= OnChoiceMade;
        }

        public void Setup(Chapter chapter)
        {       
            this.chapter = chapter;

            currentNode = chapter.GetAllNodes().ToList()[0];

            TypeText();
        }

        public void TypeText()
        {
            if (showChoicesOnNextPrompt)
            {
                dialoguePanel.SetupChoices(BuildChoices());
                showChoicesOnNextPrompt = false;

                return;
            }
            
            if (currentNode.GetChildren().Count > 1)
            {
                showChoicesOnNextPrompt = true;
            }

            dialoguePanel.SetText(currentNode.GetText());

            Character character = characterManifest.GetCharacterByIndex(currentNode.GetCharacterIndex());
            dialoguePanel.SetCharacterSprite(character.GetSprite());
            dialoguePanel.SetCharacterName(character.GetName());

            StartCoroutine(dialoguePanel.Type());

        }

        public List<DialogueNode> BuildChoices()
        {
            List<DialogueNode> choices = new List<DialogueNode>();

            foreach(string nodeID in currentNode.GetChildren())
            {
                choices.Add(chapter.GetChild(nodeID));
            }

            return choices;
        }

        public void Next()
        {
            List<string> children = currentNode.GetChildren();
            if (children.Count == 0)
            {
                OnChapterEnded();
            }
            else
            {
                if (!showChoicesOnNextPrompt)
                {
                    currentNode = chapter.GetChild(children[0]);
                }

                TypeText();
            }
        }

        public void OnChoiceMade(DialogueNode choice)
        {
            Debug.Log($"Choice Picked: {choice.GetText()}");
            currentNode = chapter.GetChild(choice.GetChildren()[0]);

            TypeText();
        }

        public void OnChapterEnded()
        {
            OnChapterEnd?.Invoke();
        }
    }
}
