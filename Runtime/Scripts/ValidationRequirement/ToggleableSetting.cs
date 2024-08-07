﻿using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MegaPint.ValidationRequirement
{

[Serializable]
public struct ToggleableSetting <T>
{
    public bool enabled;
    public T value;
}

#if UNITY_EDITOR
/// <summary> Drawer class of the <see cref="ToggleableSetting{T}" /> </summary>
[CustomPropertyDrawer(typeof(ToggleableSetting <>))]
internal class Drawer : PropertyDrawer
{
    #region Public Methods

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("value"));
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect valuePosition = EditorGUI.IndentedRect(position);

        Rect togglePosition = position;
        togglePosition.width = EditorGUIUtility.singleLineHeight;
        togglePosition.height = EditorGUIUtility.singleLineHeight;

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        SerializedProperty enabled = property.FindPropertyRelative("enabled");

        enabled.boolValue = EditorGUI.Toggle(togglePosition, enabled.boolValue);

        EditorGUI.indentLevel = 1;
        Color color = GUI.color;
        GUI.color = color / (enabled.boolValue ? 1 : 1.5f);

        EditorGUI.PropertyField(
            valuePosition,
            property.FindPropertyRelative("value"),
            new GUIContent(ObjectNames.NicifyVariableName(property.name), null, property.tooltip),
            true);

        GUI.color = color;
        EditorGUI.indentLevel = indent;
    }

    #endregion
}
#endif

}
