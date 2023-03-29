//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;

namespace DungeonArchitect.Editors
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ThemeEditorToolAttribute : Attribute
    {
        private string path;
        public string Path
        {
            get { return path; }
        }

        private int priority;
        public int Priority
        {
            get { return priority; }
        }

        public ThemeEditorToolAttribute(string path, int priority)
        {
            this.path = path;
            this.priority = priority;
        }
    }

    public delegate void ThemeEditorToolFunctionDelegate(DungeonThemeEditorWindow editor);

    public class ThemeEditorToolFunctionInfo
    {
        public ThemeEditorToolFunctionDelegate function;
        public ThemeEditorToolAttribute attribute;
    }
}
