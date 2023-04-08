using Nanoreno.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BackgroundController : MonoBehaviour
{
    private VisualElement background;

    public void Setup()
    {
        var uiHolder = new UIHolder("background");

        background = uiHolder.Element;
    }

    public void Set(Sprite image)
    {
        background.style.backgroundImage = new StyleBackground(image);
    }
}
