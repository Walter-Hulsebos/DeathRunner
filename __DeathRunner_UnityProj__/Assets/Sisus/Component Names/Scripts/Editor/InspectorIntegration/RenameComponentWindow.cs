using System;
using UnityEditor;
using UnityEngine;
using Sisus.Shared.EditorOnly;

namespace Sisus.ComponentNames.EditorOnly
{
    internal class RenameComponentWindow : EditorWindow
    {
        private static readonly Color lineColorDarkSkin = new Color32(26, 26, 26, 255);
        private static readonly Color lineColorLightSkin = new Color32(127, 127, 127, 255);
        
        private static Color LineColor => EditorGUIUtility.isProSkin ? lineColorDarkSkin : lineColorLightSkin;

        [NonSerialized]
        private Component component;
        [NonSerialized]
        private String oldName;
        [NonSerialized]
        private String newName;

        public static void Open(Rect position, Component component)
        {
            NameContainer.NowRenaming = true;

            RenameComponentWindow window = CreateInstance<RenameComponentWindow>();
            
            window.component = component;
            String oldName = NameContainer.TryGet(component, out NameContainer nameContainer) && nameContainer.NameOverride is
            {
                Length: > 0
            } nameOverride ? nameOverride : component.GetName();
            
            if(nameContainer != null && nameContainer.TooltipOverride is { Length: > 0 } tooltipOverride)
			{
                oldName += " | " + tooltipOverride;
            }

            window.oldName = oldName;
            window.newName = oldName;

            Rect buttonRect = position;
            buttonRect.height = 0f;
            Vector2 windowSize = position.size;

            window.ShowAsDropDown(buttonRect, windowSize);
        }

		private void OnGUI()
        {
            if(component == null)
            {
                Close();
                return;
            }

            if(Event.current.type == EventType.KeyDown)
            {
                switch(Event.current.keyCode)
                {
                    case KeyCode.Escape:
                        newName = oldName;
                        Close();
                        return;
                    case KeyCode.Return:
                    case KeyCode.KeypadEnter:
                        Close();
                        return;
                }
            }

            GUI.SetNextControlName("Rename Component Field");
            GUI.FocusControl("Rename Component Field");
            newName = EditorGUILayout.TextField(newName);

            // Fix for the TextField clipping over the very top line
            // of the component header component below it.
            Rect lineRect = new(0f, 0f, Screen.width, 1f);
            EditorGUI.DrawRect(lineRect, LineColor);
        }

        private void OnLostFocus() => Close();

		private void OnDestroy()
        {
            if(String.Equals(newName, oldName))
			{
                NameContainer.NowRenaming = false;
                return;
            }

            Int32 tooltipSeparator = newName.IndexOf('|');
			if(tooltipSeparator == -1)
			{
                Boolean hadTooltip = oldName.IndexOf('|') != -1;
                if(hadTooltip)
				{
                    component.SetName(new GUIContent(newName, ""));
                }
                else
                {
                    component.SetName(newName);
                }
			}
			else
			{
                String newTooltip = newName[(tooltipSeparator + 1)..].TrimStart();
                newName = newName.Substring(0, tooltipSeparator).TrimEnd();
				component.SetName(new GUIContent(newName, newTooltip));
			}

            InspectorContents.Repaint();
            NameContainer.NowRenaming = false;
        }
	}
}