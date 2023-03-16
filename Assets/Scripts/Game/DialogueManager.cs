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

        [SerializeField]
        private LogPanel logPanel;

        private DialogueHolder chapter;
        private Dialogue.Dialogue currentDialogues;
        private DialogueNode currentNode;

        private bool showChoicesOnNextPrompt;
        private int currentIndex = 0;

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

        public void Setup(DialogueHolder chapter)
        {       
            this.chapter = chapter;
            currentNode = chapter.dialogues[currentIndex].GetAllNodes().ToList()[0];
            currentDialogues = chapter.dialogues[currentIndex];

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

            logPanel.AddEntry(character.GetName(), currentNode.GetText(), character.GetSprite());

            StartCoroutine(dialoguePanel.Type());
        }

        public List<DialogueNode> BuildChoices()
        {
            List<DialogueNode> choices = new List<DialogueNode>();

            foreach(string nodeID in currentNode.GetChildren())
            {
                choices.Add(currentDialogues.GetChild(nodeID));
            }

            return choices;
        }

        public void Next()
        {
            List<string> children = currentNode.GetChildren();
            if (children.Count == 0)
            {
                if (currentIndex + 1 < chapter.dialogues.Count)
                {
                    SetNextIndex();
                    return;
                }

                OnChapterEnded();
            }
            else
            {
                if (!showChoicesOnNextPrompt)
                {
                    currentNode = currentDialogues.GetChild(children[0]);
                }

                TypeText();
            }
        }

        private void SetNextIndex()
        {
            currentIndex++;
            currentDialogues = chapter.dialogues[currentIndex];
            currentNode = chapter.dialogues[currentIndex].GetAllNodes().ToList()[0];

            Next();
        }

        public void OnChoiceMade(DialogueNode choice)
        {
            Debug.Log($"Choice Picked: {choice.GetText()}");
            currentNode = currentDialogues.GetChild(choice.GetChildren()[0]);

            TypeText();
        }

        public void OnChapterEnded()
        {
            OnChapterEnd?.Invoke();
        }
    }
}
