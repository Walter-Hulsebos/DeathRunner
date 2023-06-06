using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Sisus.ComponentNames.EditorOnly
{
    internal static class ObjectNamesUtility
    {
        private static Dictionary<Type, String> internalInspectorTitlesCache = null;

        public static Dictionary<Type, String> InternalInspectorTitlesCache
        {
            get
            {
                if(internalInspectorTitlesCache == null)
                {
                    Type inspectorTitlesType = typeof(ObjectNames).GetNestedType("InspectorTitles", BindingFlags.Static | BindingFlags.NonPublic);
                    FieldInfo inspectorTitlesField = inspectorTitlesType.GetField("s_InspectorTitles", BindingFlags.Static | BindingFlags.NonPublic);
                    internalInspectorTitlesCache = (Dictionary<Type, String>)inspectorTitlesField.GetValue(null);
                }

                return internalInspectorTitlesCache;
            }
        }
    }
}