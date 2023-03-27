using Nanoreno.Characters;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.WSA;

namespace Nanoreno.UI
{
    public class CharacterHolder
    {
        public VisualElement element;
        public string characterName;
        public SpritePosition position;
    }

    public class CharacterPanel : MonoBehaviour
    {
        private UI uiPanel;

        private List<CharacterHolder> characterHolders = new List<CharacterHolder>();
        public void Setup()
        {
            uiPanel = new UI("overlay");
        }

        public void CreateHolder(VisualElement element, string characterName, SpritePosition position)
        {
            CharacterHolder holder = new CharacterHolder();
            holder.element = element;
            holder.characterName = characterName;
            holder.position = position;

            uiPanel.Element.Add(holder.element);
            characterHolders.Add(holder);
        }

        public IEnumerator Fade(bool fadeOut, VisualElement element)
        {
            if (fadeOut)
            {
                element.style.opacity = 0;
            }
            else
            {
                element.style.opacity = 1.0f;
            }

            yield return new WaitForSeconds(1.0f);
        }

        public IEnumerator ClearHolder(CharacterHolder holder)
        {
            characterHolders.Remove(holder);

            yield return Fade(true, holder.element);
            uiPanel.Element.Remove(holder.element);
        }

        public void RemoveHolder(CharacterHolder holder)
        {
            characterHolders.Remove(holder);
            uiPanel.Element.Remove(holder.element);
        }

        public CharacterHolder GetHolderWithCharacter(string characterName)
        {
            return characterHolders.Find(x => x.characterName == characterName);
        }

        private CharacterHolder GetHolderAtPosition(SpritePosition position)
        {
            return characterHolders.Find(x => x.position == position);
        }

        public void ClearAllSlots()
        {
            foreach(CharacterHolder holder in characterHolders)
            {
                RemoveHolder(holder);
            }
        }

        public void PlaceCharacterInSlot(SpritePosition position, Character character, int percentage = 0)
        {
            CharacterHolder currentHolder = GetHolderWithCharacter(character.GetName());
            if (currentHolder != null)
            {
                if (currentHolder.position == position) { return; } // they are already in the right slot, no need to resetup

                StartCoroutine(ClearHolder(currentHolder));
            }

            CharacterHolder positionHolder = GetHolderAtPosition(position);
            if (positionHolder != null)
            {
                RemoveHolder(positionHolder);
            }

            if (position == SpritePosition.None) { return; }

            character.SetSpritePosition(position);

            VisualElement holder = new VisualElement();
            holder.AddToClassList("characterStyle");
            holder.style.backgroundImage = new StyleBackground(character.GetSprite());

            if (position != SpritePosition.Custom)
            {
                holder.style.alignSelf = GetAlignmentForPosition(position);
            }
            else
            {
                holder.style.alignSelf = Align.FlexStart;
                holder.style.left = Length.Percent(percentage);
            }

            CreateHolder(holder, character.GetName(), position);
        }

        public void SetSpeakingSprite(Character currentlySpeaking)
        {
            CharacterHolder speakingHolder = GetHolderWithCharacter(currentlySpeaking.GetName());
            if (speakingHolder != null)
            {
                speakingHolder.element.style.opacity = 1.0f;
            }

            foreach (CharacterHolder holder in characterHolders)
            {
                if (holder == speakingHolder) { continue; }

                holder.element.style.opacity = 0.3f;
            }
        }

        private Align GetAlignmentForPosition(SpritePosition position)
        {
            switch(position)
            {
                case SpritePosition.Left:
                    return Align.FlexStart;
                case SpritePosition.Centre:
                    return Align.Center;
                case SpritePosition.Right:
                    return Align.FlexEnd;
            }

            return Align.Auto;
        }
    }
}
