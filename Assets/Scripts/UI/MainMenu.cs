using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Nanoreno.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class MainMenu : MonoBehaviour
    {
        UIDocument document;

        Button startButton;
        Button loadButton;
        Button creditsButton;
        Button backButton;

        VisualElement menu;
        VisualElement credits;

        private void Start()
        {
            document = GetComponent<UIDocument>();

            startButton = document.rootVisualElement.Q("start") as Button;
            loadButton = document.rootVisualElement.Q("load") as Button;
            creditsButton = document.rootVisualElement.Q("credits") as Button;
            backButton = document.rootVisualElement.Q("back") as Button;

            menu = document.rootVisualElement.Q("menu");
            credits = document.rootVisualElement.Q("creditsPanel");
  
            startButton.clicked += onStartClicked;
            creditsButton.clicked += onCreditsClicked;
            backButton.clicked += onBackButtonClicked;
        }

        private void OnDisable()
        {
            startButton.clicked -= onStartClicked;
        }

        private void onStartClicked()
        {
            SceneManager.LoadScene("GameScene");
        }

        private void onCreditsClicked()
        {
            menu.style.display = DisplayStyle.None;
            credits.style.display = DisplayStyle.Flex;
        }

        private void onBackButtonClicked()
        {
            menu.style.display = DisplayStyle.Flex;
            credits.style.display = DisplayStyle.None;
        }
    }
}
