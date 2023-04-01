using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nanoreno.Game
{
    // due to time constraints I can only save the chapter index
    public static class SaveState
    {
        public static int chapterIndex = -1;
        public static int chapterDialogueIndex = -1;
        public static string chapterDialogueName;
    }

    public class SaveManager : MonoBehaviour
    {
        public static SaveManager instance;

        public GameManager gameManager;

        private void Start()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
        }

        public void SaveProgress(int slot)
        {
            PlayerPrefs.SetInt($"save-{slot}-chapter", SaveState.chapterIndex);
            PlayerPrefs.SetInt($"save-{slot}-chapterDialogueIndex", SaveState.chapterDialogueIndex);
            PlayerPrefs.SetString($"save-{slot}-chapterDialogueName", SaveState.chapterDialogueName);
        }

        public void LoadProgress(int slot)
        {
            int chapterIndex = PlayerPrefs.GetInt($"save-{slot}-chapter");
            int chapterDialogueIndex = PlayerPrefs.GetInt($"save-{slot}-chapterDialogueIndex");

            SaveState.chapterIndex = chapterIndex;
            SaveState.chapterDialogueIndex = chapterDialogueIndex;
            SaveState.chapterDialogueName = PlayerPrefs.GetString($"save-{slot}-chapterDialogueName", "");

            if (gameManager != null)
            {
                gameManager.LoadSave();
            }
            else
            {
                // Once this scene loads the GameManager will takeover and load the save via the SaveState
                SceneManager.LoadScene(1);
            }
        }
    }
}
