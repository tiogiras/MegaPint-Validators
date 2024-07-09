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
        SerializedProperty propertyHeight = GetHeightProperty(property);
        propertyHeight.floatValue = 0;

        Rect nameRect = GetNewRect(position, propertyHeight, 2);
        Rect typeRect = GetNewRect(position, propertyHeight, 8);

        SerializedProperty nameProperty = property.FindPropertyRelative("name");
        SerializedProperty typeProperty = property.FindPropertyRelative("type");

        EditorGUI.PropertyField(nameRect, nameProperty, new GUIContent("Name"));
        EditorGUI.PropertyField(typeRect, typeProperty, new GUIContent("Type"));

        EditorGUI.DrawRect(
            new Rect(position.x, position.y + propertyHeight.floatValue, position.width, 1),
            new Color(0.5f, 0.5f, 0.5f, 1f));

        propertyHeight.floatValue += 9;

        // 0: Object 1: String 2: Bool 3: Int 4: Float
        switch (typeProperty.enumValueFlag)
        {
            case 0:
                DrawObjectProperty(position, property);

                break;

            case 1:
                DrawStringProperty(position, property);

                break;

            case 2:
                DrawBoolProperty(position, property);

                break;

            case 3:
                DrawIntProperty(position, property);

                break;

            case 4:
                DrawFloatProperty(position, property);

                break;
        }

        property.serializedObject.ApplyModifiedProperties();
    }

    #endregion

    #region Private Methods

    private void DrawBoolProperty(Rect position, SerializedProperty property)
    {
        SerializedProperty propertyHeight = GetHeightProperty(property);

        Rect requirementRect = GetNewRect(position, propertyHeight, 2);
        SerializedProperty requirementProperty = property.FindPropertyRelative("boolRequirement");
        EditorGUI.PropertyField(requirementRect, requirementProperty, new GUIContent("Requirement (Boolean)"));

        // 0: None 1: Equals
        switch (requirementProperty.enumValueFlag)
        {
            case 0:
                break;

            case 1:
                Rect targetValueRect = GetNewRect(position, propertyHeight, 5);
                SerializedProperty targetValueProperty = property.FindPropertyRelative("targetBoolValue");
                EditorGUI.PropertyField(targetValueRect, targetValueProperty, new GUIContent("Expected Value"));

                break;
        }
    }

    private void DrawFloatProperty(Rect position, SerializedProperty property)
    {
        SerializedProperty propertyHeight = GetHeightProperty(property);

        Rect requirementRect = GetNewRect(position, propertyHeight, 2);
        SerializedProperty requirementProperty = property.FindPropertyRelative("floatRequirement");
        EditorGUI.PropertyField(requirementRect, requirementProperty, new GUIContent("Requirement (Float)"));

        // 0: None 1: Equals 2: GreaterThan 3: GreaterEqualsThan 4: LesserThan 5: LesserEqualsThan 6: Range
        switch (requirementProperty.enumValueFlag)
        {
            case 0:
                break;

            case 1:
                Rect targetValueRect = GetNewRect(position, propertyHeight, 5);
                SerializedProperty targetValueProperty = property.FindPropertyRelative("targetFloatValue");
                EditorGUI.PropertyField(targetValueRect, targetValueProperty, new GUIContent("Expected Value"));

                break;

            case 2 or 3 or 4 or 5:
                Rect referenceValueRect = GetNewRect(position, propertyHeight, 5);
                SerializedProperty referenceValueProperty = property.FindPropertyRelative("floatReferenceValue");
                EditorGUI.PropertyField(referenceValueRect, referenceValueProperty, new GUIContent("Reference Value"));

                break;

            case 6:
                Rect minValueRect = GetNewRect(position, propertyHeight, 2);
                SerializedProperty minValueProperty = property.FindPropertyRelative("floatRangeMin");
                EditorGUI.PropertyField(minValueRect, minValueProperty, new GUIContent("Range (Min)"));

                Rect maxValueRect = GetNewRect(position, propertyHeight, 5);
                SerializedProperty maxValueProperty = property.FindPropertyRelative("floatRangeMax");
                EditorGUI.PropertyField(maxValueRect, maxValueProperty, new GUIContent("Range (Max)"));

                break;
        }
    }

    private void DrawIntProperty(Rect position, SerializedProperty property)
    {
        SerializedProperty propertyHeight = GetHeightProperty(property);

        Rect requirementRect = GetNewRect(position, propertyHeight, 2);
        SerializedProperty requirementProperty = property.FindPropertyRelative("intRequirement");
        EditorGUI.PropertyField(requirementRect, requirementProperty, new GUIContent("Requirement (Integer)"));

        // 0: None 1: Equals 2: GreaterThan 3: GreaterEqualsThan 4: LesserThan 5: LesserEqualsThan 6: Range
        switch (requirementProperty.enumValueFlag)
        {
            case 0:
                break;

            case 1:
                Rect targetValueRect = GetNewRect(position, propertyHeight, 5);
                SerializedProperty targetValueProperty = property.FindPropertyRelative("targetIntValue");
                EditorGUI.PropertyField(targetValueRect, targetValueProperty, new GUIContent("Expected Value"));

                break;

            case 2 or 3 or 4 or 5:
                Rect referenceValueRect = GetNewRect(position, propertyHeight, 5);
                SerializedProperty referenceValueProperty = property.FindPropertyRelative("intReferenceValue");
                EditorGUI.PropertyField(referenceValueRect, referenceValueProperty, new GUIContent("Reference Value"));

                break;

            case 6:
                Rect minValueRect = GetNewRect(position, propertyHeight, 2);
                SerializedProperty minValueProperty = property.FindPropertyRelative("intRangeMin");
                EditorGUI.PropertyField(minValueRect, minValueProperty, new GUIContent("Range (Min)"));

                Rect maxValueRect = GetNewRect(position, propertyHeight, 5);
                SerializedProperty maxValueProperty = property.FindPropertyRelative("intRangeMax");
                EditorGUI.PropertyField(maxValueRect, maxValueProperty, new GUIContent("Range (Max)"));

                break;
        }
    }

    private void DrawObjectProperty(Rect position, SerializedProperty property)
    {
        SerializedProperty propertyHeight = GetHeightProperty(property);

        Rect requirementRect = GetNewRect(position, propertyHeight, 2);
        SerializedProperty requirementProperty = property.FindPropertyRelative("objectRequirement");
        EditorGUI.PropertyField(requirementRect, requirementProperty, new GUIContent("Requirement (Object)"));

        // 0: None 1: NotNull 2: Equals
        switch (requirementProperty.enumValueFlag)
        {
            case 0 or 1:
                break;

            case 2:
                Rect targetValueRect = GetNewRect(position, propertyHeight, 5);
                SerializedProperty targetValueProperty = property.FindPropertyRelative("targetObjectValue");
                EditorGUI.PropertyField(targetValueRect, targetValueProperty, new GUIContent("Expected Value"));

                break;
        }
    }

    private void DrawStringProperty(Rect position, SerializedProperty property)
    {
        SerializedProperty propertyHeight = GetHeightProperty(property);

        Rect requirementRect = GetNewRect(position, propertyHeight, 2);
        SerializedProperty requirementProperty = property.FindPropertyRelative("stringRequirement");
        EditorGUI.PropertyField(requirementRect, requirementProperty, new GUIContent("Requirement (String)"));

        // 0: None 1: NotNull 2: Equals
        switch (requirementProperty.enumValueFlag)
        {
            case 0 or 1:
                break;

            case 2:
                Rect targetValueRect = GetNewRect(position, propertyHeight, 5);
                SerializedProperty targetValueProperty = property.FindPropertyRelative("targetStringValue");
                EditorGUI.PropertyField(targetValueRect, targetValueProperty, new GUIContent("Expected Value"));

                break;
        }
    }

    private SerializedProperty GetHeightProperty(SerializedProperty property)
    {
        return property.FindPropertyRelative("propertyHeight");
    }

    private Rect GetNewRect(Rect position, SerializedProperty property, float spacing = 0f)
    {
        var rect = new Rect(
            position.x,
            position.y + property.floatValue,
            position.width,
            EditorGUIUtility.singleLineHeight);

        property.floatValue += EditorGUIUtility.singleLineHeight + spacing;

        return rect;
    }

    #endregion

    /*public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        /*EditorGUILayout.BeginHorizontal();

        GUILayout.Button("Hello World");
        
        EditorGUILayout.EndHorizontal();#1#
    }*/

    /*
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = new VisualElement();

        var label = new Label("Hello World");
        
        root.Add(label);
        
        /*var template = Resources.Load <VisualTreeAsset>(Constants.Validators.UserInterface.Variable);

        VisualElement root = GUIUtility.Instantiate(template);
        root.style.flexGrow = 1f;
        root.style.flexShrink = 1f;#1#

        return root;
    }
    */

    /*public override VisualElement CreateInspectorGUI()
    {
    /*var template = Resources.Load <VisualTreeAsset>(Constants.Validators.UserInterface.Variable);
    
    VisualElement root = GUI.Utility.GUIUtility.Instantiate(template);
    root.style.flexGrow = 1f;
    root.style.flexShrink = 1f;
    
    return root;#1#
    }*/
}

}
#endif
