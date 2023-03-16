using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanoreno.Game
{
    public static class SaveState
    {
        public static int chapterIndex;
        public static string textUniqueId; 
    }

    public class SaveManager : MonoBehaviour
    {
        public GameManager gameManager;

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

            gameManager.LoadSave();
        }
    }
}
