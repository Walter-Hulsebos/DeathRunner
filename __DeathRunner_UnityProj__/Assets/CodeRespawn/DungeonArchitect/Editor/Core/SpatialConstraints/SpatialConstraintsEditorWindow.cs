//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEditor;
using DungeonArchitect.SpatialConstraints;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Impl.UnityEditor;

namespace DungeonArchitect.Editors.SpatialConstraints
{

    public class SpatialConstraintsEditorWindow : EditorWindow
    {
        public static void ShowWindow()
        {
            GetWindowWithRect<SpatialConstraintsEditorWindow>(new Rect(0, 0, 256, 256));
        }

        public UISystem uiSystem { get; private set; }
        private UIRenderer renderer;

        [SerializeField] private SpatialConstraintsGraphEditor graphEditor;
        public SpatialConstraintsGraphEditor GraphEditor
        {
            get { return graphEditor; }
        }

        public void Init(SpatialConstraintAsset assetBeingEdited, SpatialConstraintsEditorAssignmentState assignmentState)
        {
            CreateUISystem();
            graphEditor = CreateInstance<SpatialConstraintsGraphEditor>();
            var graph = (assetBeingEdited != null) ? assetBeingEdited.Graph : null;
            graphEditor.Init(graph, position, graph, uiSystem);
            graphEditor.AssignmentState = assignmentState;
            graphEditor.AssetBeingEdited = assetBeingEdited;

            Repaint();
        }

        private void CreateUISystem()
        {
            uiSystem = new UnityEditorUISystem();
            renderer = new UnityEditorUIRenderer();
        }

        private void OnEnable()
        {
            wantsMouseMove = true;
            UpdateTitle();

            Init(null, SpatialConstraintsEditorAssignmentState.NotAssigned);
            graphEditor.OnEnable();
            uiSystem = new UnityEditorUISystem();
        }

        private void OnDisable()
        {
            if (graphEditor != null)
            {
                graphEditor.OnDisable();
            }
        }

        private void OnDestroy()
        {
            if (graphEditor != null)
            {
                graphEditor.OnDisable();
                graphEditor.OnDestroy();
                graphEditor = null;
            }
        }

        private void Update()
        {
            if (graphEditor != null)
            {
                graphEditor.Update();
            }
        }

        private void OnGUI()
        {
            if (uiSystem == null || renderer == null)
            {
                CreateUISystem();
            }

            if (graphEditor != null)
            {
                var guiState = new GUIState(renderer);

                var bounds = new Rect(Vector2.zero, position.size);
                graphEditor.UpdateWidget(uiSystem, bounds);
                graphEditor.Draw(uiSystem, renderer);

                guiState.Restore();
                DrawToolbar();

                HandleInput(Event.current);
            }
        }

        private void HandleInput(Event e)
        {
            if (uiSystem == null || renderer == null)
            {
                CreateUISystem();
            }

            graphEditor.HandleInput(e, uiSystem);
            
            switch (e.type)
            {
                case EventType.MouseMove:
                case EventType.MouseDrag:
                case EventType.MouseDown:
                case EventType.MouseUp:
                case EventType.KeyDown:
                case EventType.Layout:
                case EventType.MouseEnterWindow:
                case EventType.MouseLeaveWindow:
                    Repaint();
                    break;
            }
        }

        private void UpdateTitle()
        {
            titleContent = new GUIContent("Spatial Constraints");
        }

        private void DrawToolbar()
        {
            var graphValid = (graphEditor != null && graphEditor.Graph != null);

            if (graphValid)
            {
                var spatialGraphEditor = graphEditor as SpatialConstraintsGraphEditor;

                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

                GUILayout.FlexibleSpace();
                spatialGraphEditor.RealtimeUpdate = GUILayout.Toggle(spatialGraphEditor.RealtimeUpdate, "Realtime Update", EditorStyles.toolbarButton);

                EditorGUILayout.EndHorizontal();
            }

        }

    }
}