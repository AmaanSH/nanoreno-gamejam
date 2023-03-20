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
        public static string lastControlNodeId;
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
            PlayerPrefs.SetString($"save-{slot}-lastControlNodeId", SaveState.lastControlNodeId);
        }

        public void LoadProgress(int slot)
        {
            int chapterIndex = PlayerPrefs.GetInt($"save-{slot}-chapter");
            string textUniqueId = PlayerPrefs.GetString($"save-{slot}-textUniqueId");
            string lastControlNodeId = PlayerPrefs.GetString($"save-{slot}-lastControlNodeId");

            SaveState.chapterIndex = chapterIndex;
            SaveState.textUniqueId = textUniqueId;
            SaveState.lastControlNodeId = lastControlNodeId;

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
