using Nanoreno.Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nanoreno.Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private DialogeManager dialogeManager;

        [SerializeField]
        private List<DialogueHolder> chapters = new List<DialogueHolder>();

        private DialogueHolder currentChapter;

        private int currentChapterIndex;

        public void Start()
        {
            dialogeManager.OnChapterEnd += OnChapterEnded;
            StartChapter(0);
        }

        public void LoadSave()
        {
            dialogeManager.Cleanup();

            currentChapterIndex = SaveState.chapterIndex;
            currentChapter = chapters[SaveState.chapterIndex];

            DialogueNode node = dialogeManager.FindNodeWithUniqueId(SaveState.textUniqueId);

            dialogeManager.SetChapter(currentChapter);
            dialogeManager.SetNode(node);

            dialogeManager.TypeText();
        }

        public void StartChapter(int index)
        {
            currentChapterIndex = index;
            currentChapter = chapters[index];

            SaveState.chapterIndex = currentChapterIndex;

            dialogeManager.SetChapter(currentChapter);
            dialogeManager.SetNode();

            dialogeManager.TypeText();
        }
        
        public void OnChapterEnded()
        {
            if (currentChapterIndex + 1 < chapters.Count)
            {
                StartChapter(currentChapterIndex + 1);
            }
            else
            {
                Debug.Log("No More Chapters");
            }
        }
    }
}
