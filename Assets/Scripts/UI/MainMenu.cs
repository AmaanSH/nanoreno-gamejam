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

        private void Start()
        {
            document = GetComponent<UIDocument>();

            startButton = document.rootVisualElement.Q("start") as Button;
            loadButton = document.rootVisualElement.Q("load") as Button;

            startButton.clicked += onStartClicked;
        }

        private void OnDisable()
        {
            startButton.clicked -= onStartClicked;
        }

        private void onStartClicked()
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}
