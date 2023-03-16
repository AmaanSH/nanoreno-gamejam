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

        public LogEntryBuilder(VisualTreeAsset asset) : base(asset) { }

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

        public override VisualElement Build()
        {
            return null;
        }
    }
}