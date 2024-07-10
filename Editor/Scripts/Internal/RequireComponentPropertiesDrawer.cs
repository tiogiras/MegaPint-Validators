using MegaPint.ValidationRequirement.Requirements.GameObjectValidation.RequireComponentDropdown;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace MegaPint.Editor.Scripts.Internal
{

/// <summary> Drawer class for the <see cref="Properties" /> class </summary>
[CustomPropertyDrawer(typeof(Properties))]
internal class RequireComponentPropertiesDrawer : PropertyDrawer
{
    #region Public Methods

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return property.FindPropertyRelative("propertyHeight").floatValue;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty propertyHeight = property.FindPropertyRelative("propertyHeight");
        propertyHeight.floatValue = 5;

        var labelRect = new Rect(
            position.x,
            position.y + propertyHeight.floatValue,
            position.width,
            EditorGUIUtility.singleLineHeight);

        propertyHeight.floatValue += EditorGUIUtility.singleLineHeight + 2;

        var dropDownRect = new Rect(
            position.x,
            position.y + propertyHeight.floatValue,
            position.width,
            EditorGUIUtility.singleLineHeight);

        propertyHeight.floatValue += EditorGUIUtility.singleLineHeight + 10;

        SerializedProperty typeNameProperty = property.FindPropertyRelative("typeName");
        SerializedProperty typeFullNameProperty = property.FindPropertyRelative("typeFullName");

        var dropDownGUIContent = new GUIContent
        {
            text = string.IsNullOrEmpty(typeNameProperty.stringValue)
                ? "Select a type"
                : typeNameProperty.stringValue,
            tooltip = typeFullNameProperty.stringValue
        };

        var indentLevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        EditorGUI.LabelField(labelRect, "Select the required component in the dropdown below.");

        if (EditorGUI.DropdownButton(dropDownRect, dropDownGUIContent, FocusType.Passive))
        {
            SerializedProperty typesProperty = property.FindPropertyRelative("types");

            var types = new string[typesProperty.arraySize];

            for (var i = 0; i < typesProperty.arraySize; i++)
                types[i] = typesProperty.GetArrayElementAtIndex(i).stringValue;

            var dropdown = new Dropdown(
                new AdvancedDropdownState(),
                types,
                (name, fullname) =>
                {
                    typeNameProperty.stringValue = name;
                    typeFullNameProperty.stringValue = fullname;
                    property.serializedObject.ApplyModifiedProperties();
                });

            dropdown.Show(dropDownRect);
        }

        EditorGUI.indentLevel = indentLevel;

        property.serializedObject.ApplyModifiedProperties();
    }

    #endregion
}

}
