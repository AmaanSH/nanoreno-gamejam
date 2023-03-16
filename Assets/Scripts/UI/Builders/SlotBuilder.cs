using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nanoreno.UI.Builder
{
    public class SlotBuilder : Builder
    {
        protected int slotNumber;
        protected string title;
        protected string chapterName;
        protected int chapterNumber;
        protected Sprite slotImage;
 
        const string SLOT_TITLE = "slotTitle";
        const string CHAPTER_LABEL = "chapterLabel";
        const string CHAPTER_NAME_LABEL = "chapterName";
        const string CHAPTER_IMAGE = "image";

        public SlotBuilder(VisualTreeAsset asset) : base(asset) { }

        public SlotBuilder SetSlotNumber(int slotNumber)
        {
            this.slotNumber = slotNumber;
            return this;
        }

        public SlotBuilder SetChapterNumber(int chapterNumber)
        {
            this.chapterNumber = chapterNumber;
            return this;
        }

        public SlotBuilder SetChapterName(string chapterName)
        {
            this.chapterName = chapterName;
            return this;
        }

        public SlotBuilder SetImage(Sprite slotImage)
        {
            this.slotImage = slotImage;
            return this;
        }

        public override VisualElement Build()
        {
            VisualElement element = treeAsset.CloneTree();

            element.Q(CHAPTER_IMAGE).style.backgroundImage = new StyleBackground(slotImage);

            Label slotLabel = element.Q(SLOT_TITLE) as Label;
            Label chapterLabel = element.Q(CHAPTER_LABEL) as Label;
            Label chapterNameLabel = element.Q(CHAPTER_NAME_LABEL) as Label;

            slotLabel.text = $"Slot {slotNumber}";
            chapterLabel.text = $"Chapter {chapterNumber}";
            chapterNameLabel.text = chapterName;

            return element;
        }
    }
}
