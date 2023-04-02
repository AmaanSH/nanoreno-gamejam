using Nanoreno.Characters;
using Nanoreno.Dialogue;
using Nanoreno.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

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

        private UIHolder fadeToBack;
        private UIHolder blocker;

        private UIHolder debugUi;
        private Label dialogueIdText;
        private Label chapterText;
        private Label dialogueHolderText;

        public void Setup()
        {
            dialoguePanel.Setup();

            dialoguePanel.TextEndReached += Next;
            dialoguePanel.ChoiceMade += OnChoiceMade;

            characterPanel.Setup();
            logPanel.Setup();

            fadeToBack = new UIHolder("fadeToBlack");
            blocker = new UIHolder("blocker");

            debugUi = new UIHolder("debug");
            dialogueIdText = debugUi.GetElement("dialogueId") as Label;
            chapterText = debugUi.GetElement("chapter") as Label;
            dialogueHolderText = debugUi.GetElement("dialogueHolder") as Label;
        }

        private void OnDisable()
        {
            dialoguePanel.TextEndReached -= Next;
            dialoguePanel.ChoiceMade -= OnChoiceMade;
        }

        public void Cleanup()
        {
            characterPanel.ClearAllSlots();

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

        public ControlNode FindBaseControlNode()
        {
            return null;
        }

        public void SetChapter(DialogueHolder chapter)
        {       
            this.chapter = chapter;

            currentIndex = 0;
            showChoicesOnNextPrompt = false;

            chapterText.text = chapter.name;

            SetupLayeredAudio(chapter);
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
                dialogueHolderText.text = currentDialogues.name;
            }

            SaveState.chapterDialogueIndex = currentIndex;
            SaveState.chapterDialogueName = currentDialogues.name;
        }

        private void SetupLayeredAudio(DialogueHolder holder)
        {
            AudioManager.Instance.ClearAllLayers();

            var chapters = holder.dialogues;
            foreach (Chapter chapter in chapters)
            {
                foreach (DialogueNode node in chapter.GetAllNodes())
                {
                    ControlNode controlNode = node.GetControlNode();
                    if (controlNode != null && controlNode.layeredAudio.Count > 0)
                    {
                        foreach (LayerAudio layer in controlNode.layeredAudio)
                        {
                            AudioManager.Instance.AddAudioToLayers(layer.audioClip);
                        }
                    }
                }
            }
        }

        public void TypeText()
        {
            dialogueIdText.text = currentNode.name;

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

                if (controlNode.clearCharacters)
                {
                    characterPanel.ClearAllSlots();
                }

                foreach (CharacterPosition characterPosition in controlNode.GetCharacterPositions())
                {
                    characterPanel.PlaceCharacterInSlot(characterPosition.screenPosition, characterPosition.character);
                }

                AudioManager.Instance.SetupAudioForSection(controlNode);
            }

            logPanel.AddEntry(character.GetName(), currentNode.GetText(), character.GetSprite());

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

                StartCoroutine(OnChapterEnded());
            }
            else
            {
                ControlNode controlNode = currentNode.GetControlNode();
                if (controlNode != null && controlNode.transition == Transition.FadeToBlack)
                {
                    StartCoroutine(FadeToBlack());
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
        }

        private IEnumerator FadeToBlack()
        {
            blocker.Element.style.display = DisplayStyle.Flex;

            fadeToBack.Element.style.backgroundColor = new Color(0, 0, 0, 1);

            List<string> children = currentNode.GetChildren();
            currentNode = currentDialogues.GetChild(children[0]);

            yield return new WaitForSeconds(2.5f);

            fadeToBack.Element.style.backgroundColor = new Color(0, 0, 0, 0);

            TypeText();

            yield return new WaitForSeconds(0.5f);

            blocker.Element.style.display = DisplayStyle.None;
        }

        private void SetNextIndex()
        {
            currentIndex++;
            SaveState.chapterDialogueIndex = currentIndex;

            currentDialogues = chapter.dialogues[currentIndex];
            dialogueHolderText.text = currentDialogues.name;

            currentNode = chapter.dialogues[currentIndex].GetAllNodes().ToList()[0];

            SaveState.chapterDialogueName = currentDialogues.name;

            TypeText();
        }

        public void SetIndex(int index)
        {
            currentIndex = index;
            currentDialogues = chapter.dialogues[currentIndex];
            currentNode = chapter.dialogues[currentIndex].GetAllNodes().ToList()[0];
        }

        public void OnChoiceMade(DialogueNode choice)
        {
            Debug.Log($"Choice Picked: {choice.GetText()}");
            currentNode = currentDialogues.GetChild(choice.GetChildren()[0]);

            TypeText();
        }

        public IEnumerator OnChapterEnded()
        {
            blocker.Element.style.display = DisplayStyle.Flex;
            fadeToBack.Element.style.backgroundColor = new Color(0, 0, 0, 1);

            yield return new WaitForSeconds(2.5f);

            fadeToBack.Element.style.backgroundColor = new Color(0, 0, 0, 0);

            OnChapterEnd?.Invoke();

            yield return new WaitForSeconds(0.5f);
            blocker.Element.style.display = DisplayStyle.None;
        }
    }
}
