using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nanoreno.Game
{
    public static class SaveState
    {
        public static int chapterIndex;
        public static string textUniqueId; 
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
            PlayerPrefs.SetString($"save-{slot}-textUniqueId", SaveState.textUniqueId);
        }

        public void LoadProgress(int slot)
        {
            int chapterIndex = PlayerPrefs.GetInt($"save-{slot}-chapter");
            string textUniqueId = PlayerPrefs.GetString($"save-{slot}-textUniqueId");

            SaveState.chapterIndex = chapterIndex;
            SaveState.textUniqueId = textUniqueId;

            if (gameManager != null)
            {
                gameManager.LoadSave();
            }
            else
            {
                SceneManager.LoadScene(1);
            }
        }
    }
}
