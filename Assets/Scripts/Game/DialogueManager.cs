using Nanoreno.Characters;
using Nanoreno.Dialogue;
using Nanoreno.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nanoreno.Game
{
    public class DialogeManager : MonoBehaviour
    {
        public event Action OnChapterEnd;
        public DialoguePanel dialoguePanel;
        public CharacterPanel characterPanel;

        [SerializeField]
        private CharacterManifest characterManifest;

        [SerializeField]
        private LogPanel logPanel;

        private DialogueHolder chapter;
        private Chapter currentDialogues;
        private DialogueNode currentNode;

        private bool showChoicesOnNextPrompt;
        private int currentIndex = 0;

        private void Awake()
        {
            dialoguePanel.TextEndReached += Next;
            dialoguePanel.ChoiceMade += OnChoiceMade;

            characterPanel.Setup();
        }

        private void OnDisable()
        {
            dialoguePanel.TextEndReached -= Next;
            dialoguePanel.ChoiceMade -= OnChoiceMade;
        }

        public void Cleanup()
        {
            dialoguePanel.StopAllCoroutines();
        }

        public DialogueNode FindNodeWithUniqueId(string uniqueId)
        {
            foreach(Chapter node in chapter.dialogues)
            {
                DialogueNode foundNode = node.GetChild(uniqueId);
                if (foundNode != null)
                {
                    currentDialogues = node;
                    currentIndex = chapter.dialogues.IndexOf(currentDialogues);

                    return foundNode;
                }
            }

            return null;
        }

        public ControlNode SetupControlNodeWithId(string uniqueId)
        {
            foreach (Chapter node in chapter.dialogues)
            {
                DialogueNode foundNode = node.GetAllNodes().ToList().Find(x => x.GetControlNode() != null && x.GetControlNode().name == uniqueId);
                if (foundNode != null)
                {
                    ControlNode controlNode = foundNode.GetControlNode();
                    foreach (CharacterPosition characterPosition in controlNode.GetCharacterPositions())
                    {
                        characterPanel.PlaceCharacterInSlot(characterPosition.screenPosition, characterPosition.character);
                    }
                }
            }

            return null;
        }

        public void SetChapter(DialogueHolder chapter)
        {       
            this.chapter = chapter;
        }

        public void SetNode(DialogueNode node = null)
        {
            if (node != null)
            {
                currentNode = node;
            }
            else
            {
                currentNode = chapter.dialogues[currentIndex].GetAllNodes().ToList()[0];
                currentDialogues = chapter.dialogues[currentIndex];
            }
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

            dialoguePanel.SetCharacterName(character.GetName());

            if (currentNode.GetControlNode())
            {
                ControlNode controlNode = currentNode.GetControlNode();

                SaveState.lastControlNodeId = controlNode.name;

                foreach (CharacterPosition characterPosition in controlNode.GetCharacterPositions())
                {
                    characterPanel.PlaceCharacterInSlot(characterPosition.screenPosition, characterPosition.character);
                }
            }

            logPanel.AddEntry(character.GetName(), currentNode.GetText(), character.GetSprite());
            SaveState.textUniqueId = currentNode.name;

            StartCoroutine(FadeInTalkingSprite(character));
            StartCoroutine(dialoguePanel.Type());
        }

        private IEnumerator FadeInTalkingSprite(Character character)
        {
            yield return null;

            characterPanel.SetSpeakingSprite(character);
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
