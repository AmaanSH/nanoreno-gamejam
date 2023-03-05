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
        private List<Chapter> chapters = new List<Chapter>();

        private Chapter currentChapter;
        private int currentChapterIndex;

        public void Start()
        {
            dialogeManager.OnChapterEnd += OnChapterEnded;

            StartChapter(0);
        }

        public void StartChapter(int index)
        {
            currentChapterIndex = index;
            currentChapter = chapters[index];
            
            dialogeManager.Setup(currentChapter);
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
