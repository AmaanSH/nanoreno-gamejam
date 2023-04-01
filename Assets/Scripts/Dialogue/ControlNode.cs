using Nanoreno.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nanoreno.Dialogue
{
    [System.Serializable]
    public enum Emotion
    {
        Happy,
        Sad,
        Pensive,
        Angry
    }

    [System.Serializable]
    public class CharacterPosition
    {
        public Character character;
        public Emotion emotion;
        public SpritePosition screenPosition;
        public int customPercentage;
    }

    [System.Serializable]
    public class LayerAudio
    {
        public bool Play;
        public AudioClip audioClip;
    }

    [System.Serializable]
    public enum ScreenEffect
    {
        None,
        Shake
    }

    [System.Serializable]
    public enum Transition
    {
        None,
        FadeToBlack
    }


    public class ControlNode : ScriptableObject
    {
        [SerializeField]
        private List<ControlNode> children = new List<ControlNode>();

        [SerializeField]
        Rect rect = new Rect(0, 0, 200, 400);

        public List<CharacterPosition> characterPositions = new List<CharacterPosition>();
        public List<LayerAudio> layeredAudio = new List<LayerAudio>();

        public ScreenEffect screenEffect;
        public Transition transition;
        public Sprite backgroundImage;

        public AudioClip BGM;
        public AudioClip SFX;
        public bool clearCharacters = false;

        public bool stopBGM;
        public bool stopAllLayers;

        public IEnumerable<CharacterPosition> GetCharacterPositions()
        {
            return characterPositions;
        }

        public Rect GetRect()
        {
            return rect;
        }

#if UNITY_EDITOR
        public void SetPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Move Control Node");
            rect.position = newPosition;
            EditorUtility.SetDirty(this);
        }

        public List<ControlNode> GetChildren()
        {
            return children;
        }

        public void AddChild(ControlNode child)
        {
            Undo.RecordObject(this, "Add Control Node Link");
            children.Add(child);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(ControlNode child)
        {
            Undo.RecordObject(this, "Remove Control Node Link");
            children.Remove(child);
            EditorUtility.SetDirty(this);
        }

        public void SetHeight(float ySize)
        {
            rect.height = ySize;
        }

        public void AddCharacterPosition(CharacterPosition characterPos)
        {
            Undo.RecordObject(this, "Add Character Position");
            characterPositions.Add(characterPos);
            EditorUtility.SetDirty(this);
        }

        public void RemoveCharacterPosition(CharacterPosition characterPos)
        {
            Undo.RecordObject(this, "Removed Character Position");
            characterPositions.Remove(characterPos);
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
