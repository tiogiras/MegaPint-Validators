#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using MegaPint.SerializeReferenceDropdown.Runtime;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace MegaPint.SerializeReferenceDropdown.Editor
{

[CustomPropertyDrawer(typeof(SerializeReferenceDropdownAttribute))]
public class SerializeReferenceDropdownPropertyDrawer : PropertyDrawer
{
    private const string NullName = "None";
    private List <Type> _assignableTypes;

    #region Public Methods

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = new VisualElement();

        if (property.propertyType != SerializedPropertyType.ManagedReference)
            root.Add(new PropertyField(property));

        return root;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(rect, label, property);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        if (property.propertyType == SerializedPropertyType.ManagedReference)
            DrawIMGUITypeDropdown(rect, property, label);
        else
            EditorGUI.PropertyField(rect, property, label, true);

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    #endregion

    #region Private Methods

    private static void ApplyValueToProperty(object value, SerializedProperty property)
    {
        property.managedReferenceValue = value;
        property.serializedObject.ApplyModifiedProperties();
        property.serializedObject.Update();
    }

    private static List <Type> GetAssignableTypes(SerializedProperty property)
    {
        Type propertyType = TypeUtils.ExtractTypeFromString(property.managedReferenceFieldTypename);

        List <Type> nonUnityTypes =
            TypeCache.GetTypesDerivedFrom(propertyType).Where(IsAssignableNonUnityType).ToList();

        nonUnityTypes.Insert(0, null);

        return nonUnityTypes;

        bool IsAssignableNonUnityType(Type type)
        {
            return TypeUtils.IsFinalAssignableType(type) && !type.IsSubclassOf(typeof(Object));
        }
    }

    private static SerializeReferenceDropdownNameAttribute GetType(Type type)
    {
        if (type == null)
            return new SerializeReferenceDropdownNameAttribute(NullName, -30);

        /*var typesWithNames = TypeCache.GetTypesWithAttribute(typeof(SerializeReferenceDropdownNameAttribute));
        
        if (!typesWithNames.Contains(type)) 
            return new SerializeReferenceDropdownAdvancedDropdown.Type
            {
                name = ObjectNames.NicifyVariableName(type.Name)
            };*/

        return type.GetCustomAttribute <SerializeReferenceDropdownNameAttribute>();
    }

    private static string GetTypeTooltip(Type type)
    {
        if (type == null)
            return string.Empty;

        TypeCache.TypeCollection typesWithTooltip = TypeCache.GetTypesWithAttribute(typeof(TypeTooltipAttribute));

        if (!typesWithTooltip.Contains(type))
            return string.Empty;

        var tooltipAttribute = type.GetCustomAttribute <TypeTooltipAttribute>();

        return tooltipAttribute.tooltip;
    }

    private void DrawIMGUITypeDropdown(Rect rect, SerializedProperty property, GUIContent label)
    {
        _assignableTypes ??= GetAssignableTypes(property);
        Type referenceType = TypeUtils.ExtractTypeFromString(property.managedReferenceFullTypename);

        Rect dropdownRect = GetDropdownIMGUIRect(rect);

        EditorGUI.EndDisabledGroup();

        var dropdownTypeContent = new GUIContent(
            GetType(referenceType).name,
            GetTypeTooltip(referenceType));

        if (EditorGUI.DropdownButton(dropdownRect, dropdownTypeContent, FocusType.Keyboard))
        {
            var dropdown = new SerializeReferenceDropdownAdvancedDropdown(
                new AdvancedDropdownState(),
                _assignableTypes.Select(GetType),
                index => WriteNewInstanceByIndexType(index, property));

            dropdown.Show(dropdownRect);
        }

        EditorGUI.PropertyField(rect, property, label, true);

        Rect GetDropdownIMGUIRect(Rect mainRect)
        {
            var dropdownOffset = EditorGUIUtility.labelWidth;
            var mRect = new Rect(mainRect);
            mRect.width -= dropdownOffset;
            mRect.x += dropdownOffset;
            mRect.height = EditorGUIUtility.singleLineHeight;

            return mRect;
        }
    }

    private void WriteNewInstanceByIndexType(int typeIndex, SerializedProperty property)
    {
        Type newType = _assignableTypes[typeIndex];
        var newObject = newType != null ? FormatterServices.GetUninitializedObject(newType) : null;
        ApplyValueToProperty(newObject, property);
    }

    #endregion
}

}
#endif
