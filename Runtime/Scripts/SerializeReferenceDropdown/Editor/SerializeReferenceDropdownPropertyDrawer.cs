﻿#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using MegaPint.SerializeReferenceDropdown.Runtime;
using MegaPint.ValidationRequirement;
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
        // TODO some hacking magic here 

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
            return new SerializeReferenceDropdownNameAttribute(NullName, null, Int32.MinValue + 1);

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
        if (!int.TryParse(label.text.Replace("Element ", ""), out var index))
            return;

        Type currentValue = null;
        List <Type> addedRequirements = new();
        List <IValidationRequirement> requirements = new();

        IValidationRequirement currentRequirement = null;
        
        switch (property.serializedObject.targetObject)
        {
            case ValidatableMonoBehaviour validatableMonoBehaviour:
                requirements = validatableMonoBehaviour.Requirements();

                break;

            case ValidatorSettings settings:
                requirements = settings.Requirements();

                break;
        }
        
        if (requirements is {Count: > 0})
        {
            addedRequirements.AddRange(
                from requirement in requirements
                where requirement != null
                select requirement.GetType());

            currentRequirement = requirements[index];
            currentValue = currentRequirement?.GetType();
        }

        _assignableTypes ??= GetAssignableTypes(property);
        Type referenceType = TypeUtils.ExtractTypeFromString(property.managedReferenceFullTypename);

        Rect dropdownRect = GetDropdownIMGUIRect(rect);

        EditorGUI.EndDisabledGroup();

        var dropdownTypeContent = new GUIContent(
            GetType(referenceType).name,
            GetTypeTooltip(referenceType));

        if (currentRequirement != null)
        {
            DrawSeverityOverwriteButton(dropdownRect, (ScriptableValidationRequirement)currentRequirement);
        }

        if (EditorGUI.DropdownButton(dropdownRect, dropdownTypeContent, FocusType.Keyboard))
        {
            var dropdown = new SerializeReferenceDropdownAdvancedDropdown(
                new AdvancedDropdownState(),
                _assignableTypes.Select(GetType),
                addedRequirements.ToArray(),
                currentValue,
                i => WriteNewInstanceByIndexType(i, property));

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

    private void DrawSeverityOverwriteButton(Rect dropdownRect, ScriptableValidationRequirement requirement)
    {
        // TODO continue here
        // TODO images / colors
        // TODO tooltips
        // TODO actual values
        // TODO onclick event change values
        
        const int ButtonWidth = 15;
        const int ButtonOffset = 5;

        var rect = new Rect(
            dropdownRect.x - (ButtonWidth + ButtonOffset),
            dropdownRect.y + dropdownRect.height * .125f,
            ButtonWidth,
            dropdownRect.height * .75f);

        //var bgColor = GUI.backgroundColor;
        var color = GUI.color;
        //GUI.backgroundColor = new Color(0, 0, 0,.5f);

        GUI.color = GetColorBySeverityOverwrite(color, requirement);
        
        if (GUI.Button(rect, ""))
        {
            requirement.ChangeSeverityOverwrite();
        }
        
        //GUI.backgroundColor = bgColor;
        GUI.color = color;
    }


    private Color GetColorBySeverityOverwrite(Color baseColor, ScriptableValidationRequirement requirement)
    {
        return requirement.GetSeverityOverwrite() switch
               {
                   ValidationState.Unknown or ValidationState.Ok => new Color(
                       baseColor.r,
                       baseColor.g,
                       baseColor.b,
                       .5f),
                   ValidationState.Warning => baseColor,
                   ValidationState.Error => baseColor,
                   _ => throw new ArgumentOutOfRangeException()
               };
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
