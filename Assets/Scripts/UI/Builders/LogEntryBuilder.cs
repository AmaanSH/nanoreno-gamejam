using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nanoreno.UI.Builder
{
    public class LogEntryBuilder : Builder
    {
        public string CharacterName { get; private set; }
        public string Text { get; private set; }
        public Sprite Sprite { get; private set; }

        public LogEntryBuilder SetName(string characterName)
        {
            CharacterName = characterName;
            return this;
        }

        public LogEntryBuilder SetText(string text)
        {
            Text = text;
            return this;
        }

        public LogEntryBuilder SetSprite(Sprite sprite)
        {
            Sprite = sprite;
            return this;
        }

        public override VisualElement Build()
        {
            // TODO: add to stylesheet
            VisualElement holder = new VisualElement();
            holder.AddToClassList("logPanelEntry");

            holder.style.flexDirection = FlexDirection.Row;
            holder.style.flexShrink = 1;
            holder.style.flexGrow = 0;
            holder.style.flexWrap = Wrap.Wrap;

            Image image = new Image();
            image.name = "logEntryCharacterSprite";
            holder.Add(image);

            VisualElement textAndNameHolder = new VisualElement();
            holder.Add(textAndNameHolder);

            TextElement charName = new TextElement();
            charName.name = "logEntryCharacterName";
            charName.style.whiteSpace = WhiteSpace.Normal;
            charName.style.flexGrow = 1;
            charName.style.unityFontStyleAndWeight = FontStyle.Bold;
            charName.style.color = Color.white;
            textAndNameHolder.Add(charName);

            TextElement charText = new TextElement();
            charText.name = "logEntryCharacterText";
            charText.style.flexGrow = 1;
            charText.style.flexShrink = 0;
            charText.style.whiteSpace = WhiteSpace.Normal;
            charText.style.color = Color.white;
            textAndNameHolder.Add(charText);

            return holder;
        }
    }
}