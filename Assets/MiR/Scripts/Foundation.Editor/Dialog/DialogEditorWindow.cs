
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.Collections.Generic;

namespace Foundation.Editor
{
    public sealed class DialogEditorWindow : EditorWindow
    {
        enum State
        {
            Normal,
            DraggingBackground,
            DraggingNode,
            ResizingNode,
            CreatingConnection,
        }

        const float LineThickness = 2.0f;

        Dialog dialog;
        State state = State.Normal;

        Vector2 nodeCreatePosition;
        Vector2 draggingSize;
        DialogNode draggingNode;

        Texture2D nodeTexture;
        Texture2D dragHandleTexture;
        GUIStyle nodeStyle;
        GUIStyle dragHandleStyle;

        DialogNode selectedNode;

        int nodeIndex;
        int nextIndex;

        static readonly Vector2 DragHandleSize = new Vector2(10.0f, 10.0f);

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Draw

        void OnGUI()
        {
            if (Application.isPlaying) {
                CenterMessage("Dialogue editor is not available in play mode.");
                return;
            }
            if (dialog == null) {
                CenterMessage("No dialogue selected.");
                return;
            }

            HandleEvents();

            DrawGrid(dialog.EditorTransform, 20.0f, Color.gray.WithAlpha(0.3f));
            DrawGrid(dialog.EditorTransform, 100.0f, Color.gray.WithAlpha(0.4f));

            var localizationOptions = LocalizationData.EditorGetLocalizationIDs();

            if (dialog.Nodes != null) {
                foreach (var node in dialog.Nodes) {
                    var r = node.Bounds.TransformedBy(dialog.EditorTransform);
                    GUI.Box(r, "", nodeStyle);

                    Rect rect = r;
                    rect.xMin += 10;
                    rect.xMax -= 10;
                    rect.yMin += 10;
                    rect.yMax = rect.yMin + EditorGUIUtility.singleLineHeight;

                    var oldID = node.Text.LocalizationID;
                    int oldSelectedIndex = Array.IndexOf(localizationOptions, oldID);
                    int newSelectedIndex = EditorGUI.Popup(rect, oldSelectedIndex, localizationOptions);
                    if (newSelectedIndex != oldSelectedIndex) {
                        Undo.RegisterCompleteObjectUndo(dialog, "Edit node text");
                        node.Text.LocalizationID = localizationOptions[newSelectedIndex];
                    }

                    rect.y += EditorGUIUtility.singleLineHeight + 10;

                    DialogNode.Action oldAction = node.NodeAction;
                    DialogNode.Action newAction = (DialogNode.Action)EditorGUI.EnumPopup(rect, oldAction);
                    if (oldAction != newAction) {
                        Undo.RegisterCompleteObjectUndo(dialog, "Change node action");
                        node.NodeAction = newAction;
                    }

                    switch (newAction) {
                        case DialogNode.Action.Default:
                            break;

                        case DialogNode.Action.StartQuest: {
                            rect.y += EditorGUIUtility.singleLineHeight + 10;

                            Quest oldQuest = node.ActionQuest;
                            Quest newQuest = (Quest)EditorGUI.ObjectField(rect, oldQuest, typeof(Quest), false);
                            if (oldQuest != newQuest) {
                                Undo.RegisterCompleteObjectUndo(dialog, "Change action quest");
                                node.ActionQuest = newQuest;
                            }

                            break;
                        }
                    }

                    rect.y += EditorGUIUtility.singleLineHeight + 10;

                    DialogNode.Condition oldCondition = node.NodeCondition;
                    DialogNode.Condition newCondition = (DialogNode.Condition)EditorGUI.EnumPopup(rect, oldCondition);
                    if (oldCondition != newCondition) {
                        Undo.RegisterCompleteObjectUndo(dialog, "Change node condition");
                        node.NodeCondition = newCondition;
                    }

                    switch (newCondition) {
                        case DialogNode.Condition.None:
                            break;

                        case DialogNode.Condition.QuestCompleted: {
                            rect.y += EditorGUIUtility.singleLineHeight + 10;

                            Quest oldQuest = node.ConditionQuest;
                            Quest newQuest = (Quest)EditorGUI.ObjectField(rect, oldQuest, typeof(Quest), false);
                            if (oldQuest != newQuest) {
                                Undo.RegisterCompleteObjectUndo(dialog, "Change condition quest");
                                node.ConditionQuest = newQuest;
                            }

                            break;
                        }
                    }

                    rect.y += EditorGUIUtility.singleLineHeight + 10;

                    bool oldIsPlayer = node.IsPlayer;
                    bool newIsPlayer = EditorGUI.ToggleLeft(rect, "Is player", oldIsPlayer);
                    if (oldIsPlayer != newIsPlayer) {
                        Undo.RegisterCompleteObjectUndo(dialog, "Toggle is player");
                        node.IsPlayer = newIsPlayer;
                    }

                    rect.y += EditorGUIUtility.singleLineHeight + 10;

                    bool oldAllowReuse = node.AllowReuse;
                    bool newAllowReuse = EditorGUI.ToggleLeft(rect, "Allow reuse", oldAllowReuse);
                    if (oldAllowReuse != newAllowReuse) {
                        Undo.RegisterCompleteObjectUndo(dialog, "Toggle allow reuse");
                        node.AllowReuse = newAllowReuse;
                    }

                    r = new Rect(node.Bounds.max - DragHandleSize, DragHandleSize).TransformedBy(dialog.EditorTransform);
                    GUI.Box(r, "", dragHandleStyle);
                }

                foreach (var node in dialog.Nodes) {
                    if (node.Next != null) {
                        foreach (var next in node.Next)
                            DrawConnection(node, next);
                    }
                }
            }

            if (state == State.CreatingConnection) {
                var mousePos = mousePosition(Event.current);
                var targetNode = nodeAtPosition(mousePos);

                if (targetNode != null && selectedNode.CanConnectTo(targetNode))
                    DrawConnection(selectedNode, targetNode);
                else {
                    var fromRect = selectedNode.Bounds.TransformedBy(dialog.EditorTransform);
                    var sourcePoint = new Vector2(fromRect.center.x, fromRect.yMax);
                    Handles.color = Color.yellow;
                    Handles.DrawLine(sourcePoint, Event.current.mousePosition, LineThickness);
                }
            }

            if (GUI.changed)
                Repaint();
        }

        void CenterMessage(string message)
        {
            var text = new GUIContent(message);
            var textSize = GUI.skin.label.CalcSize(text);
            EditorGUI.LabelField(new Rect((position.size - textSize) * 0.5f, textSize), text);
        }

        void DrawGrid(SimpleTransform2D transform, float spacing, Color color)
        {
            spacing *= transform.Scale;

            float w = position.width;
            float h = position.height;
            int nx = Mathf.CeilToInt(w / spacing);
            int ny = Mathf.CeilToInt(h / spacing);

            var off = new Vector2(transform.Offset.x % spacing, transform.Offset.y % spacing);

            Handles.BeginGUI();
            Handles.color = color;

            for (int i = 0; i <= nx; i++) {
                var s = off + new Vector2(spacing * i, -spacing    );
                var e = off + new Vector2(spacing * i,  spacing + h);
                Handles.DrawLine(s, e);
            }
            for (int i = 0; i <= ny; i++) {
                var s = off + new Vector2(-spacing    , spacing * i);
                var e = off + new Vector2( spacing + w, spacing * i);
                Handles.DrawLine(s, e);
            }

            Handles.EndGUI();
        }

        void DrawConnection(DialogNode sourceNode, DialogNode targetNode)
        {
            var sourceRect = sourceNode.Bounds.TransformedBy(dialog.EditorTransform);
            var sourcePoint = new Vector2(sourceRect.center.x, sourceRect.yMax);

            var targetRect = targetNode.Bounds.TransformedBy(dialog.EditorTransform);
            var targetPoint = new Vector2(targetRect.center.x, targetRect.yMin);

            Handles.color = Color.yellow;
            Handles.DrawLine(sourcePoint, targetPoint, LineThickness);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Events

        void HandleEvents()
        {
            var e = Event.current;

            switch (e.type) {
                case EventType.MouseDown:
                    switch (e.button) {
                        case 0:
                            OnLeftButtonDown(e);
                            break;

                        case 1:
                            OnRightButtonDown(e);
                            break;

                        case 2:
                            OnMiddleButtonDown();
                            break;
                    }
                    break;

                case EventType.MouseUp:
                    OnMouseUp();
                    break;

                case EventType.MouseMove:
                    OnMouseMove(e);
                    break;

                case EventType.MouseDrag:
                    OnMouseDrag(e);
                    break;

                case EventType.ScrollWheel:
                    OnMouseWheel(e);
                    break;
            }
        }

        void OnLeftButtonDown(Event e)
        {
            switch (state) {
                case State.Normal:
                    Vector2 mouse = mousePosition(e);
                    if (dialog.Nodes != null) {
                        // FIXME: replace with nodeAtPosition
                        foreach (var node in dialog.Nodes) {
                            if (node.Bounds.Contains(mouse)) {
                                draggingNode = node;
                                var r = new Rect(node.Bounds.max - DragHandleSize, DragHandleSize);
                                if (!r.Contains(mouse))
                                    state = State.DraggingNode;
                                else {
                                    state = State.ResizingNode;
                                    draggingSize = node.Bounds.size;
                                }
                                GUI.changed = true;
                                break;
                            }
                        }
                    }
                    break;

                case State.CreatingConnection: {
                    state = State.Normal;

                    Vector2 mousePos = mousePosition(e);
                    DialogNode overNode = nodeAtPosition(mousePos);

                    if (overNode != null && selectedNode.CanConnectTo(overNode)) {
                        Undo.RegisterCompleteObjectUndo(dialog, "Create connection");
                        if (selectedNode.Next == null)
                            selectedNode.Next = new List<DialogNode>();
                        selectedNode.Next.Add(overNode);
                    }

                    GUI.changed = true;
                    break;
                }
            }
        }

        void OnRightButtonDown(Event e)
        {
            GenericMenu menu;

            state = State.Normal;

            Vector2 mousePos = mousePosition(e);
            DialogNode overNode = nodeAtPosition(mousePos);

            if (overNode != null) {
                selectedNode = overNode;
                menu = new GenericMenu();
                menu.AddItem(new GUIContent("Create connection"), false, CreateConnection);
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete node"), false, DeleteNode);
                menu.ShowAsContext();
                return;
            }

            if (dialog.Nodes != null) {
                nodeIndex = 0;
                foreach (var node in dialog.Nodes) {
                    if (node.Next != null) {
                        Vector2 sourcePoint = new Vector2(node.Bounds.center.x, node.Bounds.yMax);
                        nextIndex = 0;
                        foreach (var next in node.Next) {
                            var targetRect = next.Bounds;
                            var targetPoint = new Vector2(targetRect.center.x, targetRect.yMin);

                            if (HandleUtility.DistancePointLine(mousePos, sourcePoint, targetPoint) < 5.0f) {
                                menu = new GenericMenu();
                                menu.AddItem(new GUIContent("Delete connection"), false, DeleteConnection);
                                menu.ShowAsContext();
                                return;
                            }

                            ++nextIndex;
                        }
                    }
                    ++nodeIndex;
                }
            }

            nodeCreatePosition = mousePos;
            menu = new GenericMenu();
            menu.AddItem(new GUIContent("Create node"), false, CreateNode);
            menu.ShowAsContext();
        }

        void OnMiddleButtonDown()
        {
            if (state == State.Normal) {
                state = State.DraggingBackground;
                GUI.changed = true;
            }
        }

        void OnMouseUp()
        {
            switch (state) {
                case State.Normal:
                case State.CreatingConnection:
                    break;

                case State.DraggingBackground:
                case State.DraggingNode:
                case State.ResizingNode:
                    state = State.Normal;
                    GUI.changed = true;
                    break;
            }
        }

        void OnMouseMove(Event e)
        {
            switch (state) {
                case State.CreatingConnection:
                    GUI.changed = true;
                    break;
            }
        }

        void OnMouseDrag(Event e)
        {
            switch (state) {
                case State.Normal:
                    break;

                case State.DraggingBackground:
                    dialog.EditorTransform.Offset += e.delta / dialog.EditorTransform.Scale;
                    EditorUtility.SetDirty(dialog);
                    GUI.changed = true;
                    break;

                case State.DraggingNode:
                    Undo.RegisterCompleteObjectUndo(dialog, "Drag Node");
                    draggingNode.Bounds = new Rect(
                        draggingNode.Bounds.min + e.delta / dialog.EditorTransform.Scale,
                        draggingNode.Bounds.size);
                    GUI.changed = true;
                    break;

                case State.ResizingNode: {
                    Undo.RegisterCompleteObjectUndo(dialog, "Resize Node");
                    draggingSize += e.delta / dialog.EditorTransform.Scale;
                    Vector2 size = draggingSize;
                    if (size.x < 50)
                        size.x = 50;
                    if (size.y < 50)
                        size.y = 50;
                    draggingNode.Bounds.size = size;
                    GUI.changed = true;
                    break;
                }

                case State.CreatingConnection:
                    GUI.changed = true;
                    break;
            }
        }

        void OnMouseWheel(Event e)
        {
            if (state == State.Normal) {
                dialog.EditorTransform.AdjustScale(e.delta.y * 0.05f, mousePosition(e));
                EditorUtility.SetDirty(dialog);
                GUI.changed = true;
            }
        }

        void CreateNode()
        {
            var size = new Vector2(200, 100);

            Undo.RegisterCompleteObjectUndo(dialog, "Create node");

            var node = new DialogNode();
            node.UniqueId = dialog.NextUniqueId++;
            node.Bounds = new Rect(nodeCreatePosition - size * 0.5f, size);
            node.Next = new List<DialogNode>();
            node.NextIds = new List<int>();

            if (dialog.Nodes == null)
                dialog.Nodes = new List<DialogNode>();

            dialog.Nodes.Add(node);
        }

        void DeleteNode()
        {
            Undo.RegisterCompleteObjectUndo(dialog, "Delete node");

            int outerIndex = dialog.Nodes.Count;
            while (outerIndex-- > 0) {
                var node = dialog.Nodes[outerIndex];
                if (node == selectedNode) {
                    dialog.Nodes.RemoveAt(outerIndex);
                    continue;
                }

                if (node.Next != null) {
                    int innerIndex = node.Next.Count;
                    while (innerIndex-- > 0) {
                        if (node.Next[innerIndex] == selectedNode)
                            node.Next.RemoveAt(innerIndex);
                    }
                }
            }
        }

        void CreateConnection()
        {
            state = State.CreatingConnection;
        }

        void DeleteConnection()
        {
            Undo.RegisterCompleteObjectUndo(dialog, "Delete connection");
            dialog.Nodes[nodeIndex].Next.RemoveAt(nextIndex);
        }

        Vector2 mousePosition(Event e)
        {
            return e.mousePosition.InverseTransformedBy(dialog.EditorTransform);
        }

        DialogNode nodeAtPosition(Vector2 pos)
        {
            if (dialog.Nodes != null) {
                foreach (var node in dialog.Nodes) {
                    if (node.Bounds.Contains(pos))
                        return node;
                }
            }

            return null;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Data serialization

        public void LoadAsset(UnityEngine.Object asset)
        {
            dialog = asset as Dialog;
            UpdateTitle();
            Repaint();
        }

        void UpdateTitle()
        {
            if (dialog == null)
                titleContent = new GUIContent("<Dialogue Not Open>");
            else
                titleContent = new GUIContent(((UnityEngine.Object)dialog).name);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Unity callbacks

        void OnEnable()
        {
            state = State.Normal;
            UpdateTitle();

            nodeStyle = new GUIStyle();
            nodeTexture = TextureUtility.CreateSolidColorTexture(new Color(0.3f, 0.3f, 0.3f));
            nodeStyle.normal.background = nodeTexture;

            var f = Color.black;
            var _ = new Color(0.3f, 0.3f, 0.3f);
            dragHandleTexture = TextureUtility.CreateTexture(10, 10, new Color[] {
                    _,_,_,_,_,_,_,_,_,_,
                    _,_,_,_,_,_,_,_,_,_,
                    _,_,f,f,f,f,f,_,_,_,
                    _,_,f,f,f,f,_,_,_,_,
                    _,_,f,f,f,_,_,_,f,_,
                    _,_,f,f,_,_,_,f,f,_,
                    _,_,f,_,_,_,f,f,f,_,
                    _,_,_,_,_,f,f,f,f,_,
                    _,_,_,_,f,f,f,f,f,_,
                    _,_,_,_,_,_,_,_,_,_,
                });

            dragHandleStyle = new GUIStyle();
            dragHandleStyle.normal.background = dragHandleTexture;

            Undo.undoRedoPerformed += OnUndoRedoPerformed;

            wantsMouseMove = true;
        }

        void OnDisable()
        {
            state = State.Normal;

            DestroyImmediate(nodeTexture);
            DestroyImmediate(dragHandleTexture);

            dragHandleStyle = null;
            dragHandleTexture = null;
            nodeStyle = null;
            nodeTexture = null;

            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
        }

        void OnUndoRedoPerformed()
        {
            Repaint();
        }

        [OnOpenAsset(1)]
        static bool OpenDialogue(int assetInstanceID, int line)
        {
            var dialogue = EditorUtility.InstanceIDToObject(assetInstanceID) as Dialog;
            if (dialogue == null)
                return false;

            GetWindow<DialogEditorWindow>().LoadAsset(dialogue);
            return true;
        }
    }
}
