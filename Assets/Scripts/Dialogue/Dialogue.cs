using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace Nanoreno.Dialogue
{
    [CreateAssetMenu(fileName = "New Chapter", menuName = "Nanoreno/Dialogue/Create Dialogue", order = 0)]
    public class Chapter : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        List<DialogueNode> nodes = new List<DialogueNode>();

        [SerializeField]
        Sprite background;

        [SerializeField]
        Vector2 newNodeOffset = new Vector2(250, 0);

        [SerializeField]
        Vector2 newControlNodeOffset = new Vector2(0, 200);

        Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

        private void Awake()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            nodeLookup.Clear();
            foreach(DialogueNode node in GetAllNodes())
            {
                nodeLookup[node.name] = node;
            }
        }

        public DialogueNode GetNode(int index)
        {
            return nodes[index];
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        {
            return nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach (string childID in parentNode.GetChildren())
            {
                if (nodeLookup.ContainsKey(childID))
                {
                    yield return nodeLookup[childID];
                }
            }
        }

        public DialogueNode GetChild(string childID)
        {
            if (nodeLookup.ContainsKey(childID))
            {
                return nodeLookup[childID];
            }

            return null;
        }

#if UNITY_EDITOR
        public void CreateNode(DialogueNode parent)
        {
            DialogueNode newNode = MakeNode(parent);

            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            Undo.RecordObject(this, "Added Dialogue Node");

            AddNode(newNode);
        }

        public void CreateControlNode(DialogueNode parent)
        {
            ControlNode newNode = MakeControlNode(parent);

            Undo.RegisterCreatedObjectUndo(newNode, "Created Control Node");
            Undo.RecordObject(this, "Added Control Node");

            OnValidate();
        }

        public void RemoveControlNode(DialogueNode parent)
        {
            Undo.RecordObject(this, "Deleting Control Node");
            parent.RemoveControlNode();
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Deleting Dialogue Node");
            nodes.Remove(nodeToDelete);
            
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete.name);
            }
        }

        private DialogueNode MakeNode(DialogueNode parent)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();
            if (parent != null)
            {
                parent.AddChild(newNode.name);
                newNode.SetPosition(parent.GetRect().position + newNodeOffset);
            }

            return newNode;
        }

        private ControlNode MakeControlNode(DialogueNode parent)
        {
            ControlNode newNode = CreateInstance<ControlNode>();
            newNode.name = $"{parent.name}-CONTROL";

            parent.SetControlNode(newNode);
            newNode.SetPosition(parent.GetRect().position + newControlNodeOffset);

            return newNode;
        }

        private void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);
            OnValidate();
        }
#endif
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (nodes.Count == 0)
            {
                DialogueNode newNode = MakeNode(null);
                AddNode(newNode);
            }

            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach(DialogueNode node in GetAllNodes())
                {
                    if (AssetDatabase.GetAssetPath(node) == "")
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                    
                    if (node.GetControlNode())
                    {
                        if (AssetDatabase.GetAssetPath(node.GetControlNode()) == "")
                        {
                            AssetDatabase.AddObjectToAsset(node.GetControlNode(), this);
                        }
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {

        }
    }
}
