using Leopotam.Ecs;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Asset_Cleaner {
    internal class RequestRepaintEvt { }

    internal class SysRepaintWindow : IEcsRunSystem, IEcsInitSystem {
        private EcsFilter<RequestRepaintEvt> Repaint = null;

        public void Init() {
            var wd = Globals<WindowData>.Value;
            wd.SceneFoldout = new GUIContent(AssetPreview.GetMiniTypeThumbnail(typeof(SceneAsset)));
            wd.ExpandScenes = true;
            wd.ExpandFiles = true;
            wd.ScrollPos = Vector2.zero;
        }

        public void Run() {
            var wd = Globals<WindowData>.Value;

            if (Repaint.IsEmpty()) return;
            wd.Window.Repaint();
            InternalEditorUtility.RepaintAllViews();
            Repaint.AllDestroy();
        }
    }
}