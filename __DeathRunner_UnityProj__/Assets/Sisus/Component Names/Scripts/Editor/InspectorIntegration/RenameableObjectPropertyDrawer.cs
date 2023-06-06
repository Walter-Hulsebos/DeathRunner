using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sisus.ComponentNames.EditorOnly
{
    [CustomPropertyDrawer(typeof(Object), true)]
    public class RenameableObjectPropertyDrawer : PropertyDrawer
    {
        private static Color ObjectFieldBackgroundColor => EditorGUIUtility.isProSkin ? new Color32(42, 42, 42, 255) : new Color32(237, 237, 237, 255);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Component objectFieldValue = property.objectReferenceValue as Component;
            if(objectFieldValue == null || property.serializedObject.isEditingMultipleObjects)
            {
                DrawDefaultObjectField(position, property, label);
                return;
            }

            Component componentWithField = property.serializedObject.targetObject as Component;
            String textInsideField = GetTextInsideField(objectFieldValue, componentWithField);

            Rect fieldRect = EditorGUI.PrefixLabel(position, label);
            DrawDefaultObjectField(fieldRect, property, GUIContent.none);
            fieldRect.x += 16f;
            fieldRect.width -= 35f;
            fieldRect.y += 2f;
            fieldRect.height -= 3f;
            EditorGUI.DrawRect(fieldRect, ObjectFieldBackgroundColor);
            GUI.Label(fieldRect, textInsideField);
        }

        private static String GetTextInsideField(Component objectFieldValue, Component componentWithField)
        {
            if(objectFieldValue == componentWithField)
            {
                return "This Component";
            }

            String componentName = ComponentName.Get(objectFieldValue);

            if(componentWithField != null && objectFieldValue.gameObject == componentWithField.gameObject)
            {
                return String.Concat(componentName, " (this GameObject)");
            }

            String gameObjectName = objectFieldValue.gameObject.name;
            return String.Concat(componentName, " (", gameObjectName, ")");
        }

        protected virtual void DrawDefaultObjectField(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.ObjectField(position, property, label);
        }
    }
}