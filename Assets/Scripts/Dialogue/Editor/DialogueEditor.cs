using Nanoreno.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEngine;

namespace Nanoreno.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        Chapter selectedDialogue = null;

        [NonSerialized]
        GUIStyle nodeStyle;

        [NonSerialized]
        GUIStyle controlNodeStyle;

        [NonSerialized]
        Vector2 draggingOffset;
        [NonSerialized]
        Vector2 draggingControlNodeOffset;

        [NonSerialized]
        DialogueNode creatingNode = null;
        [NonSerialized]
        DialogueNode creatingControlNode = null;
        [NonSerialized]
        DialogueNode deletingControlNode = null;
        [NonSerialized]
        DialogueNode deletingNode = null;
        [NonSerialized]
        DialogueNode draggingNode = null;

        [NonSerialized]
        DialogueNode linkingParentNode = null;
        [NonSerialized]
        ControlNode linkingControlParentNode = null;

        Vector2 scrollPosition;

        [NonSerialized]
        bool draggingCanvas = false;

        [NonSerialized]
        Vector2 draggingCanvasOffset;

        const float CANVAS_SIZE = 100000;
        const float BACKGROUND_SIZE = 50;

        CharacterManifest characterManifest;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Chapter dialogue = EditorUtility.InstanceIDToObject(instanceID) as Chapter;
            if (dialogue != null)
            {
                ShowEditorWindow();
                return true;
            }

            return false;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.normal.textColor = Color.white;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);

            controlNodeStyle = new GUIStyle();
            controlNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            controlNodeStyle.normal.textColor = Color.white;
            controlNodeStyle.padding = new RectOffset(20, 20, 20, 20);
            controlNodeStyle.border = new RectOffset(12, 12, 12, 12);

            characterManifest = Resources.Load("character_manifest") as CharacterManifest;
        }

        private void OnSelectionChanged()
        {
            Chapter newDialogue = Selection.activeObject as Chapter;
            if (newDialogue != null)
            {
                selectedDialogue = newDialogue;
                Repaint();
            }
        }

        private void OnGUI()
        {
            if (selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected");
            }
            else
            {
                ProcessEvents();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                Rect canvas = GUILayoutUtility.GetRect(CANVAS_SIZE, CANVAS_SIZE);
                Texture2D backgroundTex = Resources.Load("background") as Texture2D;
                Rect texCoords = new Rect(0, 0, CANVAS_SIZE / BACKGROUND_SIZE, CANVAS_SIZE / BACKGROUND_SIZE);

                GUI.DrawTextureWithTexCoords(canvas, backgroundTex, texCoords);

                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawConnections(node);
                }
                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);

                    if (node.GetControlNode() != null)
                    {
                        DrawControlNode(node.GetControlNode());

                        DrawControlNodeConnections(node.GetControlNode());
                    }
                }

                EditorGUILayout.EndScrollView();

                if (creatingNode != null)
                {
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }
                if (creatingControlNode != null)
                {
                    selectedDialogue.CreateControlNode(creatingControlNode);
                    creatingControlNode = null;
                }
                if (deletingControlNode != null)
                {
                    selectedDialogue.RemoveControlNode(deletingControlNode);
                    deletingControlNode = null;
                }
                if (deletingNode != null)
                {
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }
            }
        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                if (draggingNode != null)
                {
                    draggingOffset = draggingNode.GetRect().position - Event.current.mousePosition;

                    if (draggingNode.GetControlNode())
                    {
                        draggingControlNodeOffset = draggingNode.GetControlNode().GetRect().position - Event.current.mousePosition;
                    }
                    else
                    {
                        draggingControlNodeOffset = Vector2.zero;
                    }

                    Selection.activeObject = draggingNode;
                }
                else
                {
                    draggingCanvas = true;
                    draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                    Selection.activeObject = selectedDialogue;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                draggingNode.SetPosition(Event.current.mousePosition + draggingOffset);

                if (draggingNode.GetControlNode())
                {
                    draggingNode.GetControlNode().SetPosition(Event.current.mousePosition + draggingControlNodeOffset);
                }

                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseDrag && draggingCanvas)
            {
                scrollPosition =  draggingCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;
            }
            else if (Event.current.type == EventType.MouseUp && draggingCanvas)
            {
                draggingCanvas = false;
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode foundNode = null;

            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if (node.GetRect().Contains(point))
                {
                    foundNode = node;
                }
            }

            return foundNode;
        }

        private void DrawNode(DialogueNode node)
        {
            GUIStyle style = nodeStyle;
            GUILayout.BeginArea(node.GetRect(), style);

            node.SetCharacter(EditorGUILayout.Popup(node.GetCharacterIndex(), GetCharacters()));
            node.SetText(EditorGUILayout.TextField(node.GetText()));

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("x"))
            {
                deletingNode = node;
            }

            DrawLinkButton(node);

            if (GUILayout.Button("+"))
            {
                creatingNode = node;
            }

            GUILayout.EndHorizontal();

            if (node.GetControlNode())
            {
                if (GUILayout.Button("Remove Control Node"))
                {
                    deletingControlNode = node;
                }
            }
            else
            {
                if (GUILayout.Button("Create Control Node"))
                {
                    creatingControlNode = node;
                }
            }

            GUILayout.EndArea();
        }

        private void DrawControlNode(ControlNode controlNode)
        {
            GUIStyle style = controlNodeStyle;
            GUILayout.BeginArea(controlNode.GetRect(), style);

            DrawLinkButtonControlNode(controlNode);

            controlNode.screenEffect = (ScreenEffect)EditorGUILayout.EnumPopup(controlNode.screenEffect);

            controlNode.clearCharacters = EditorGUILayout.Toggle("Hide Characters", controlNode.clearCharacters);

            EditorGUILayout.LabelField("Background");
            controlNode.backgroundImage = (Sprite)EditorGUILayout.ObjectField(controlNode.backgroundImage, typeof(Sprite), true);

            EditorGUILayout.LabelField("BGM");
            controlNode.BGM = (AudioClip)EditorGUILayout.ObjectField(controlNode.BGM, typeof(AudioClip), true);

            EditorGUILayout.LabelField("SFX");
            controlNode.SFX = (AudioClip)EditorGUILayout.ObjectField(controlNode.SFX, typeof(AudioClip), true);

            EditorGUILayout.LabelField("Character Pos");
            ReorderableList list = new ReorderableList(controlNode.characterPositions, typeof(CharacterPosition), false, false, true, true);

            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                CharacterPosition element = list.list[index] as CharacterPosition;
                if (element != null)
                {
                    int elementWidth = (element.screenPosition == SpritePosition.Custom) ? 90 : 60;

                    element.screenPosition = (SpritePosition)EditorGUI.EnumPopup(
                        new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
                        element.screenPosition
                    );

                    element.character = (Character)EditorGUI.ObjectField(
                        new Rect(rect.x + 60, rect.y, rect.width - elementWidth, EditorGUIUtility.singleLineHeight), 
                        element.character, typeof(Character), 
                        true
                    );

                    if (element.screenPosition == SpritePosition.Custom)
                    {
                        element.customPercentage = EditorGUI.IntField(
                            new Rect(rect.x + 100, rect.y, rect.width - 90, EditorGUIUtility.singleLineHeight),
                            element.customPercentage
                        );
                    }
                }
            };

            list.DoLayoutList();

            GUILayout.EndArea();
        }

        private string[] GetCharacters()
        {
            List<string> characterNames = new List<string>();

            foreach (Character character in characterManifest.GetCharacters())
            {
                string name = character.GetName();
                if (string.IsNullOrEmpty(name))
                {
                    name = "No Name";
                }

                characterNames.Add(name);
            }

            return characterNames.ToArray();
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y);

            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {
                Vector3 endPosition = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y);
                
                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.8f;

                Handles.DrawBezier(
                    startPosition, endPosition, 
                    startPosition + controlPointOffset, endPosition - controlPointOffset, 
                    Color.white, null, 4f
                );
            }
        }

        private void DrawControlNodeConnections(ControlNode node)
        {
            Vector3 startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y);

            foreach (ControlNode childNode in node.GetChildren())
            {
                Vector3 endPosition = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y);

                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.8f;

                Handles.DrawBezier(
                    startPosition, endPosition,
                    startPosition + controlPointOffset, endPosition - controlPointOffset,
                    Color.red, null, 4f
                );
            }
        }

        private void DrawLinkButtonControlNode(ControlNode node)
        {
            if (linkingControlParentNode == null)
            {
                if (GUILayout.Button("link"))
                {
                    linkingControlParentNode = node;
                }
            }
            else if (linkingControlParentNode == node)
            {
                if (GUILayout.Button("cancel"))
                {
                    linkingControlParentNode = null;
                }
            }
            else if (linkingControlParentNode.GetChildren().Contains(node))
            {
                if (GUILayout.Button("unlink"))
                {
                    linkingControlParentNode.RemoveChild(node);
                    linkingControlParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("child"))
                {
                    linkingControlParentNode.AddChild(node);
                    linkingControlParentNode = null;
                }
            }
        }

        private void DrawLinkButton(DialogueNode node)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("link"))
                {
                    linkingParentNode = node;
                }
            }
            else if (linkingParentNode == node)
            {
                if (GUILayout.Button("cancel"))
                {
                    linkingParentNode = null;
                }
            }
            else if (linkingParentNode.GetChildren().Contains(node.name))
            {
                if (GUILayout.Button("unlink"))
                {
                    linkingParentNode.RemoveChild(node.name);
                    linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("child"))
                {
                    linkingParentNode.AddChild(node.name);
                    linkingParentNode = null;
                }
            }
        }
    }
}
