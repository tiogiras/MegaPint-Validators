using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace com.tiogiras.megapint_validators.Editor.Scripts
{
    [CustomPropertyDrawer(typeof(SubClassAttribute))]
    public class SubClassList : PropertyDrawer
    {
        private float LineHeight => EditorGUIUtility.singleLineHeight * 1.1f;
        
        SubClassAttribute _attribute;
        private ReorderableList _list;
        private bool _initialized;
        private Type[] _types;
        
        private List<float> _heights = new();
        private string _name;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            
        }


        private class SubClassAttribute : PropertyAttribute
        {
            public Type Type { get; private set; }

            public SubClassAttribute(Type type)
                => Type = type;
        }
    }
}
