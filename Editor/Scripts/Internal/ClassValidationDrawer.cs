﻿#if UNITY_EDITOR
using MegaPint.ValidationRequirement.Requirements.CSharp.VariablesLogic;
using UnityEditor;
using UnityEngine;

namespace MegaPint.Editor.Scripts.Internal
{

/// <summary> Drawer class for the <see cref="ClassValidation" /> class </summary>
[CustomPropertyDrawer(typeof(ClassValidation))]
internal class ClassValidationDrawer : PropertyDrawer
{
    #region Public Methods

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return property.FindPropertyRelative("propertyHeight").floatValue;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var height = 0f;

        if (!property.FindPropertyRelative("foundClass").boolValue)
        {
            var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            height += EditorGUIUtility.singleLineHeight;

            EditorGUI.LabelField(
                rect,
                "No class could be found with the specified parameters!",
                new GUIStyle {normal = {textColor = Color.red}});
        }

        property.FindPropertyRelative("propertyHeight").floatValue = height;
        property.serializedObject.ApplyModifiedProperties();
    }

    #endregion
}

}
#endif
