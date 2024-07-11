// TODO commenting

#if UNITY_EDITOR
using MegaPint.ValidationRequirement.Requirements.CSharp.VariablesLogic;
using UnityEditor;
using UnityEngine;

namespace MegaPint.Editor.Scripts.Internal
{

/// <summary> Drawer class for the <see cref="Variable" /> class </summary>
[CustomPropertyDrawer(typeof(Variable.Properties))]
internal class VariableDrawer : PropertyDrawer
{
    #region Public Methods

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return property.FindPropertyRelative("propertyHeight").floatValue;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var height = 0f;

        Rect nameRect = GetNewRect(position, ref height, 2);
        Rect typeRect = GetNewRect(position, ref height, 8);

        SerializedProperty nameProperty = property.FindPropertyRelative("name");
        SerializedProperty typeProperty = property.FindPropertyRelative("type");

        EditorGUI.PropertyField(nameRect, nameProperty, new GUIContent("Name"));
        EditorGUI.PropertyField(typeRect, typeProperty, new GUIContent("Type"));

        if (!property.FindPropertyRelative("fieldFound").boolValue ||
            property.FindPropertyRelative("typeIndex").intValue != typeProperty.enumValueFlag)
        {
            Rect fieldNotFoundRect = GetNewRect(position, ref height, 8);

            EditorGUI.LabelField(
                fieldNotFoundRect,
                $"No {GetVariableTypeName(typeProperty.enumValueFlag)} with the name [ <b>{nameProperty.stringValue}</b> ] found in the selected class!",
                new GUIStyle {normal = {textColor = Color.red}});
        }

        EditorGUI.DrawRect(
            new Rect(position.x, position.y + height, position.width, 1),
            new Color(0.5f, 0.5f, 0.5f, 1f));

        height += 9;

        // 0: Object 1: String 2: Bool 3: Int 4: Float
        switch (typeProperty.enumValueFlag)
        {
            case 0:
                DrawObjectProperty(position, property, ref height);

                break;

            case 1:
                DrawStringProperty(position, property, ref height);

                break;

            case 2:
                DrawBoolProperty(position, property, ref height);

                break;

            case 3:
                DrawIntProperty(position, property, ref height);

                break;

            case 4:
                DrawFloatProperty(position, property, ref height);

                break;
        }

        property.FindPropertyRelative("propertyHeight").floatValue = height;
        property.serializedObject.ApplyModifiedProperties();
    }

    #endregion

    #region Private Methods

    private static void DrawBoolProperty(Rect position, SerializedProperty property, ref float height)
    {
        Rect requirementRect = GetNewRect(position, ref height, 2);
        SerializedProperty requirementProperty = property.FindPropertyRelative("boolRequirement");
        EditorGUI.PropertyField(requirementRect, requirementProperty, new GUIContent("Requirement (Boolean)"));

        // 0: None 1: Equals
        switch (requirementProperty.enumValueFlag)
        {
            case 0:
                break;

            case 1:
                Rect targetValueRect = GetNewRect(position, ref height, 5);
                SerializedProperty targetValueProperty = property.FindPropertyRelative("targetBoolValue");
                EditorGUI.PropertyField(targetValueRect, targetValueProperty, new GUIContent("Expected Value"));

                break;
        }
    }

    private static void DrawFloatProperty(Rect position, SerializedProperty property, ref float height)
    {
        Rect requirementRect = GetNewRect(position, ref height, 2);
        SerializedProperty requirementProperty = property.FindPropertyRelative("floatRequirement");
        EditorGUI.PropertyField(requirementRect, requirementProperty, new GUIContent("Requirement (Float)"));

        // 0: None 1: Equals 2: GreaterThan 3: GreaterEqualsThan 4: LesserThan 5: LesserEqualsThan 6: Range
        switch (requirementProperty.enumValueFlag)
        {
            case 0:
                break;

            case 1 or 7:
                Rect targetValueRect = GetNewRect(position, ref height, 5);
                SerializedProperty targetValueProperty = property.FindPropertyRelative("targetFloatValue");
                EditorGUI.PropertyField(targetValueRect, targetValueProperty, new GUIContent("Expected Value"));

                break;

            case 2 or 3 or 4 or 5:
                Rect referenceValueRect = GetNewRect(position, ref height, 5);
                SerializedProperty referenceValueProperty = property.FindPropertyRelative("floatReferenceValue");
                EditorGUI.PropertyField(referenceValueRect, referenceValueProperty, new GUIContent("Reference Value"));

                break;

            case 6:
                Rect minValueRect = GetNewRect(position, ref height, 2);
                SerializedProperty minValueProperty = property.FindPropertyRelative("floatRangeMin");
                EditorGUI.PropertyField(minValueRect, minValueProperty, new GUIContent("Range (Min)"));

                Rect maxValueRect = GetNewRect(position, ref height, 5);
                SerializedProperty maxValueProperty = property.FindPropertyRelative("floatRangeMax");
                EditorGUI.PropertyField(maxValueRect, maxValueProperty, new GUIContent("Range (Max)"));

                break;
        }
    }

    private static void DrawIntProperty(Rect position, SerializedProperty property, ref float height)
    {
        Rect requirementRect = GetNewRect(position, ref height, 2);
        SerializedProperty requirementProperty = property.FindPropertyRelative("intRequirement");
        EditorGUI.PropertyField(requirementRect, requirementProperty, new GUIContent("Requirement (Integer)"));

        // 0: None 1: Equals 2: GreaterThan 3: GreaterEqualsThan 4: LesserThan 5: LesserEqualsThan 6: Range
        switch (requirementProperty.enumValueFlag)
        {
            case 0:
                break;

            case 1 or 7:
                Rect targetValueRect = GetNewRect(position, ref height, 5);
                SerializedProperty targetValueProperty = property.FindPropertyRelative("targetIntValue");
                EditorGUI.PropertyField(targetValueRect, targetValueProperty, new GUIContent("Expected Value"));

                break;

            case 2 or 3 or 4 or 5:
                Rect referenceValueRect = GetNewRect(position, ref height, 5);
                SerializedProperty referenceValueProperty = property.FindPropertyRelative("intReferenceValue");
                EditorGUI.PropertyField(referenceValueRect, referenceValueProperty, new GUIContent("Reference Value"));

                break;

            case 6:
                Rect minValueRect = GetNewRect(position, ref height, 2);
                SerializedProperty minValueProperty = property.FindPropertyRelative("intRangeMin");
                EditorGUI.PropertyField(minValueRect, minValueProperty, new GUIContent("Range (Min)"));

                Rect maxValueRect = GetNewRect(position, ref height, 5);
                SerializedProperty maxValueProperty = property.FindPropertyRelative("intRangeMax");
                EditorGUI.PropertyField(maxValueRect, maxValueProperty, new GUIContent("Range (Max)"));

                break;
        }
    }

    private static void DrawObjectProperty(Rect position, SerializedProperty property, ref float height)
    {
        Rect requirementRect = GetNewRect(position, ref height, 2);
        SerializedProperty requirementProperty = property.FindPropertyRelative("objectRequirement");
        EditorGUI.PropertyField(requirementRect, requirementProperty, new GUIContent("Requirement (Object)"));

        // 0: None 1: NotNull 2: Equals
        switch (requirementProperty.enumValueFlag)
        {
            case 0 or 1:
                break;

            case 2 or 3:
                Rect targetValueRect = GetNewRect(position, ref height, 5);
                SerializedProperty targetValueProperty = property.FindPropertyRelative("targetObjectValue");
                EditorGUI.PropertyField(targetValueRect, targetValueProperty, new GUIContent("Expected Value"));

                break;
        }
    }

    private static void DrawStringProperty(Rect position, SerializedProperty property, ref float height)
    {
        Rect requirementRect = GetNewRect(position, ref height, 2);
        SerializedProperty requirementProperty = property.FindPropertyRelative("stringRequirement");
        EditorGUI.PropertyField(requirementRect, requirementProperty, new GUIContent("Requirement (String)"));

        // 0: None 1: NotNull 2: Equals
        switch (requirementProperty.enumValueFlag)
        {
            case 0 or 1:
                break;

            case 2 or 3:
                Rect targetValueRect = GetNewRect(position, ref height, 5);
                SerializedProperty targetValueProperty = property.FindPropertyRelative("targetStringValue");
                EditorGUI.PropertyField(targetValueRect, targetValueProperty, new GUIContent("Expected Value"));

                break;
        }
    }

    private static Rect GetNewRect(Rect position, ref float height, float spacing = 0f)
    {
        var rect = new Rect(
            position.x,
            position.y + height,
            position.width,
            EditorGUIUtility.singleLineHeight);

        height += EditorGUIUtility.singleLineHeight + spacing;

        return rect;
    }

    private static string GetVariableTypeName(int type)
    {
        return type switch
               {
                   0 => "Object",
                   1 => "String",
                   2 => "Bool",
                   3 => "Integer",
                   4 => "Float",
                   var _ => "TypeNotFound"
               };
    }

    #endregion
}

}
#endif
