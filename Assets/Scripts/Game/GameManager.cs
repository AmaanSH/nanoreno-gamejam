using Nanoreno.Dialogue;
using Nanoreno.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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
            SetupGame();
            
            dialogeManager.OnChapterEnd += OnChapterEnded;
            
            if (SaveState.chapterIndex != -1)
            {
                LoadSave();
            }
            else
            {
                StartChapter(0);
            }
        }

        public void SetupGame()
        {
            dialogeManager.Setup();

            Button menuButton = UIManager.GetElement("menuButton") as Button;
            menuButton.clicked += () =>
            {
                SceneManager.LoadScene(0);
            };
        }

        public void LoadSave()
        {
            dialogeManager.Cleanup();
            AudioManager.Instance.ForceStopAllAudio();

            currentChapterIndex = SaveState.chapterIndex;
            currentChapter = chapters[SaveState.chapterIndex];
            dialogeManager.SetChapter(currentChapter);

            if (SaveState.chapterDialogueIndex > 0)
            {
                dialogeManager.SetIndex(SaveState.chapterDialogueIndex);
            }
            else
            {
                dialogeManager.SetNode();
            }

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
                SceneManager.LoadScene(0);

                Debug.Log("No More Chapters");
            }
        }
    }
}
