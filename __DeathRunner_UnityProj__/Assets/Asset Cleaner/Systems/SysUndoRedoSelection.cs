using System.Linq;
using System.Runtime.InteropServices;
using Leopotam.Ecs;
using UnityEditor;
using UnityEngine;
using static Asset_Cleaner.AufCtx;

namespace Asset_Cleaner {
    internal class CleanupPrevArg { }

    internal class UndoEvt { }

    internal class RedoEvt { }

    internal class SysUndoRedoSelection : IEcsRunSystem, IEcsInitSystem, IEcsDestroySystem {
        private EcsFilter<UndoEvt> UndoEvt = default;
        private EcsFilter<RedoEvt> RedoEvt = default;

        private bool _preventHistoryInsert;
        private bool _preventSelectionSet;

        public void Init() {
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
            Selection.selectionChanged += OnSelectionChanged;
            Globals<UndoRedoState>.Value = new UndoRedoState();
            if (Globals<PersistentUndoRedoState>.Value.History.Count > 0)
                _preventHistoryInsert = true;
            OnSelectionChanged(); //init selection
        }

        public void Destroy() {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
            Selection.selectionChanged -= OnSelectionChanged;
            Globals<UndoRedoState>.Value = default;
        }

        public void Run() {
            MouseInput();

            if (UndoEvt.IsEmpty() && RedoEvt.IsEmpty()) return;
            Counters(undo: !UndoEvt.IsEmpty(), redo: !RedoEvt.IsEmpty(), false);
            _preventHistoryInsert = true;
            if (!_preventSelectionSet) {
                (var history, var id) = Globals<PersistentUndoRedoState>.Value;
                SelectionEntry entry = history[id];
                if (entry.Valid())
                    Selection.objects = entry.IsGuids
                        ? entry.Guids.Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<Object>).Where(obj => obj).ToArray()
                        : entry.SceneObjects;
            }

            _preventSelectionSet = false;

            UndoEvt.AllDestroy();
            RedoEvt.AllDestroy();
        }


        private static void Counters(bool undo, bool redo, bool insertToHistory) {
            var state = Globals<PersistentUndoRedoState>.Value;
            World.NewEntityWith(out RequestRepaintEvt _);

            const int MinId = 0;
            if (insertToHistory) {
                var entry = new SelectionEntry();

                var count = state.History.Count - 1 - state.Id;
                if (count > 0)
                    state.History.RemoveRange(state.Id + 1, count);

                state.History.Add(entry);
                state.Id = MaxId();

                if (Selection.assetGUIDs.Length > 0) {
                    entry.IsGuids = true;
                    entry.Guids = Selection.assetGUIDs;
                }
                else {
                    entry.SceneObjects = Selection.objects;
                }
            }

            if (undo) {
                // loop to skip invalid 
                while (true) {
                    state.Id -= 1;
                    if (state.Id < MinId) break;
                    if (state.History[state.Id].Valid()) break;
                }
            }

            if (redo) {
                // loop to skip invalid
                while (true) {
                    state.Id += 1;
                    if (state.Id > MaxId()) break;
                    if (state.History[state.Id].Valid()) break;
                }
            }

            state.Id = Mathf.Clamp(state.Id, MinId, MaxId());

            var undoRedoState = Globals<UndoRedoState>.Value;
            undoRedoState.UndoEnabled = state.Id != MinId;
            undoRedoState.RedoEnabled = state.Id != MaxId();

            int MaxId() => Mathf.Max(0, state.History.Count - 1);
        }

        private void OnSelectionChanged() {
            World.NewEntityWith(out RequestRepaintEvt _);
            if (Globals<Config>.Value == null || Globals<Config>.Value.Locked) return;
            Counters(undo: false, redo: false, insertToHistory: !_preventHistoryInsert);
            _preventHistoryInsert = false;

            World.NewEntityWith(out SelectionChanged comp);
            World.NewEntityWith(out CleanupPrevArg _);
            var go = Selection.activeGameObject;
            if (go && go.scene.IsValid()) {
                comp.From = FindModeEnum.Scene;
                comp.Target = go;
                comp.Scene = go.scene;
            }
            else {
                var guids = Selection.assetGUIDs;
                // comp.Guids = Selection.assetGUIDs;
                bool any = guids != null && guids.Length > 0;
                if (any) {
                    comp.From = FindModeEnum.File;
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    comp.Target = AssetDatabase.LoadAssetAtPath<Object>(path);
                }
                else {
                    comp.From = FindModeEnum.None;
                    comp.Target = null;
                }
            }
        }

        // prevents selection history flooding
        private void OnUndoRedoPerformed() {
            // below is a hackish way to catch Undo/Redo from editor
            //if (AufCtx.Destroyed) return;
            if (!Undo.GetCurrentGroupName().Equals("Selection Change")) return;
            var evt = Event.current;
            if (evt == null) return;
            if (evt.rawType != EventType.KeyDown) return;

            switch (evt.keyCode) {
                case KeyCode.Z:
                    World.NewEntityWith(out UndoEvt _);
                    _preventSelectionSet = true; // prevent manual Selection set  
                    break;
                case KeyCode.Y:
                    World.NewEntityWith(out RedoEvt _);
                    _preventSelectionSet = true;
                    break;
            }
        }

        private void MouseInput() {
            if (_nextClick > EditorApplication.timeSinceStartup) return;

            var any = false;
            if (Pressed(0x5)) {
                World.NewEntityWith(out UndoEvt _);
                any = true;
            }

            if (Pressed(0x6)) {
                World.NewEntityWith(out RedoEvt _);
                any = true;
            }

            if (any)
                _nextClick = EditorApplication.timeSinceStartup + 0.25;
        }

#if UNITY_EDITOR_WIN
        [DllImport("USER32.dll")]
        private static extern short GetKeyState(int keycode);
#else
        static short GetKeyState(int keycode) => 0;
#endif

        private double _nextClick;

        // 5 back, 6 fw
        private static bool Pressed(int keyCode) => (GetKeyState(keyCode) & 0x100) != 0;
    }
}