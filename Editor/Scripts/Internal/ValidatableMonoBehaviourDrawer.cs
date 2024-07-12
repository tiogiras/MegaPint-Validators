#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MegaPint.SerializeReferenceDropdown.Runtime;
using MegaPint.ValidationRequirement;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MegaPint.Editor.Scripts.Internal
{

/// <summary> Drawer class for the <see cref="ValidatableMonoBehaviour" /> class </summary>
[CustomEditor(typeof(ValidatableMonoBehaviour), true)]
internal class ValidatableMonoBehaviourDrawer : UnityEditor.Editor
{
    private static readonly string[] s_exclusion = {"m_Script", "_importedSettings"};
    private static readonly string[] s_exclusionFull = {"m_Script", "_requirements"};

    private bool _listening;

    #region Public Methods

    public override void OnInspectorGUI()
    {
        var castedTarget = (ValidatableMonoBehaviour)target;

        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Export Requirements"))
            ExportRequirements();

        if (GUILayout.Button("Import Requirements"))
        {
            _listening = true;
            var controlID = GUIUtility.GetControlID(FocusType.Passive);
            EditorGUIUtility.ShowObjectPicker <ValidatorSettings>(null, false, "", controlID);
        }

        if (Event.current.commandName == "ObjectSelectorClosed" && _listening)
        {
            _listening = false;

            castedTarget.ImportSetting(
                (ValidatorSettings)EditorGUIUtility.GetObjectPickerObject());

            castedTarget.OnValidate();
        }

        EditorGUILayout.EndHorizontal();

        var hasImportedSettings = castedTarget.HasImportedSettings;

        if (hasImportedSettings)
        {
            SerializedProperty foldoutState = serializedObject.FindProperty("importedSettingsFoldout");

            foldoutState.boolValue =
                EditorGUILayout.BeginFoldoutHeaderGroup(foldoutState.boolValue, "Imported Requirements");

            if (foldoutState.boolValue)
            {
                List <ValidatorSettings> importedSettings = castedTarget.GetImportedSettings();

                if (importedSettings.Count > 0)
                {
                    List <ValidatorSettings> list = castedTarget.GetImportedSettings();

                    for (var i = list.Count - 1; i >= 0; i--)
                    {
                        ValidatorSettings setting = list[i];
                        DrawImportedSetting(setting);
                    }
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        DrawPropertiesExcluding(serializedObject, /*importedSettings ? s_exclusionFull : */s_exclusion);

        serializedObject.ApplyModifiedProperties();
    }

    #endregion

    #region Private Methods

    private void DrawImportedSetting(ValidatorSettings setting)
    {
        if (setting == null)
            return;

        var castedTarget = (ValidatableMonoBehaviour)serializedObject.targetObject;

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

        EditorGUILayout.BeginVertical(GUILayout.Width(15));
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("X", GUILayout.Width(15), GUILayout.Height(15)))
            castedTarget.RemoveImportedSetting(setting);

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();

        EditorGUILayout.LabelField(setting.name);

        List <ScriptableValidationRequirement> activeRequirements =
            castedTarget.ActiveRequirements;

        List <ScriptableValidationRequirement> sourceRequirements = setting.Requirements(true);

        if (sourceRequirements.Count == 0)
        {
            EditorGUILayout.EndHorizontal();
            
            return;    
        }

        List <ScriptableValidationRequirement> disabledRequirements = sourceRequirements.
                                                                      Where(
                                                                          requirement =>
                                                                              !activeRequirements.Any(
                                                                                  r => r.uniqueID.Equals(
                                                                                      requirement.uniqueID))).
                                                                      ToList();

        if (disabledRequirements.Count > 0)
        {
            List <SerializeReferenceDropdownNameAttribute> attributes = disabledRequirements.ConvertAll(
                requirement =>
                    requirement.GetType().
                                GetCustomAttribute <
                                    SerializeReferenceDropdownNameAttribute>());

            var tooltip = string.Join("\n", attributes.Select(attr => attr.name));

            Color color = UnityEngine.GUI.color;
            UnityEngine.GUI.color = Color.red;

            EditorGUILayout.LabelField(
                new GUIContent
                {
                    text =
                        "Disabled requirements due to higher priority requirements.",
                    tooltip = tooltip
                });

            UnityEngine.GUI.color = color;
        }

        EditorGUILayout.EndHorizontal();
    }

    /// <summary> Export the saved requirements to an external file </summary>
    private void ExportRequirements()
    {
        try
        {
            List <ScriptableValidationRequirement> sourceRequirements =
                ((ValidatableMonoBehaviour)target).Requirements(true);

            if (sourceRequirements.Count == 0)
            {
                Debug.LogWarning("Nothing to export!");
                return;
            }
            
            var path = EditorUtility.SaveFilePanelInProject(
                "Export Requirements",
                "Requirements",
                "asset",
                "Hello World");

            if (string.IsNullOrEmpty(path))
                return;
            
            var settingsFile = CreateInstance <ValidatorSettings>();

            List <ScriptableValidationRequirement> requirements = new();

            foreach (ScriptableValidationRequirement requirement in sourceRequirements)
            {
                ScriptableValidationRequirement clone = requirement.Clone();
                clone.GenerateUniqueID();

                requirements.Add(clone);
            }

            settingsFile.SetRequirements(requirements);

            AssetDatabase.CreateAsset(settingsFile, path);
            AssetDatabase.Refresh();
        }
        catch (Exception)
        {
            // ignored
        }
    }

    #endregion
}

}

#endif
