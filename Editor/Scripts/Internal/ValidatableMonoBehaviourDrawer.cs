#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using MegaPint.SerializeReferenceDropdown.Runtime;
using MegaPint.ValidationRequirement;
using UnityEditor;
using UnityEngine;

[assembly: InternalsVisibleTo("tiogiras.megapint.editor")]
namespace MegaPint.Editor.Scripts.Internal
{

/// <summary> Drawer class for the <see cref="ValidatableMonoBehaviour" /> class </summary>
[CustomEditor(typeof(ValidatableMonoBehaviour), true)]
internal class ValidatableMonoBehaviourDrawer : UnityEditor.Editor
{
    public static Action <string> onImport;
    public static Action <string> onExport;
    
    private static readonly string[] s_exclusion = {"m_Script", "_importedSettings"};

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

            var obj = (ValidatorSettings)EditorGUIUtility.GetObjectPickerObject();
            
            onImport?.Invoke(AssetDatabase.GetAssetPath(obj));
            
            castedTarget.ImportSetting(obj);
            castedTarget.OnValidate();
        }

        EditorGUILayout.EndHorizontal();

        var hasImportedSettings = castedTarget.HasImportedSettings;

        if (hasImportedSettings)
            DrawImportedSettings(castedTarget);

        DrawPropertiesExcluding(serializedObject, s_exclusion);

        serializedObject.ApplyModifiedProperties();
    }

    #endregion

    #region Private Methods

    /// <summary> Draw one imported setting </summary>
    /// <param name="setting"> Targeted setting </param>
    /// <param name="isDefaultImport"> If the setting is imported via the class </param>
    private void DrawImportedSetting(ValidatorSettings setting, bool isDefaultImport = false)
    {
        if (setting == null)
            return;

        var windowWidth = EditorGUIUtility.currentViewWidth - (isDefaultImport ? 30 : 0);

        var castedTarget = (ValidatableMonoBehaviour)serializedObject.targetObject;

        List <ScriptableValidationRequirement> activeRequirements =
            castedTarget.GetActiveRequirements();

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

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

        if (!isDefaultImport)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(15));
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("X", GUILayout.Width(15), GUILayout.Height(15)))
                castedTarget.RemoveImportedSetting(setting);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();   
        }

        var labelWidth = windowWidth * (disabledRequirements.Count > 0 ? .6f : 1f);

        EditorGUILayout.BeginVertical();
        GUILayout.FlexibleSpace();

        EditorGUILayout.LabelField(setting.name, GUILayout.MaxWidth(labelWidth));

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();

        if (disabledRequirements.Count > 0)
        {
            List <ValidationRequirementAttribute> attributes = disabledRequirements.ConvertAll(
                requirement =>
                    requirement.GetType().
                                GetCustomAttribute <
                                    ValidationRequirementAttribute>());

            var tooltip = $"- {string.Join("\n- ", attributes.Select(attr => attr.name))}";

            Color color = UnityEngine.GUI.color;
            UnityEngine.GUI.color = Color.red;

            GUIStyle style = EditorStyles.label;
            style.wordWrap = true;

            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            EditorGUILayout.LabelField(
                new GUIContent
                {
                    text =
                        "Disabled requirements!",
                    tooltip = tooltip
                },
                style,
                GUILayout.MaxWidth(windowWidth * .4f));

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            UnityEngine.GUI.color = color;
        }

        if (!isDefaultImport)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(15));
            GUILayout.FlexibleSpace();

            var priority = castedTarget.GetSettingPriority(setting);
            var priorityOptions = castedTarget.GetPriorityOptions(out List <int> priorityValues);

            var newPriority = EditorGUILayout.IntPopup(
                "",
                priority,
                priorityOptions,
                priorityValues.ToArray(),
                GUILayout.Width(35));

            if (newPriority != priority)
                castedTarget.SetSettingPriority(setting, newPriority);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();   
        }

        EditorGUILayout.EndHorizontal();
    }

    /// <summary> Draw all imported settings </summary>
    /// <param name="castedTarget"> Target as ValidatableMonoBehaviour </param>
    private void DrawImportedSettings(ValidatableMonoBehaviour castedTarget)
    {
        SerializedProperty foldoutState = serializedObject.FindProperty("importedSettingsFoldout");

        foldoutState.boolValue =
            EditorGUILayout.BeginFoldoutHeaderGroup(foldoutState.boolValue, "Imported Requirements");

        if (foldoutState.boolValue)
        {
            DrawImportedSettingsFor(castedTarget.defaultSettings, true);
            DrawImportedSettingsFor(castedTarget.GetImportedSettings());
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    /// <summary> Draw all settings in the targeted list </summary>
    /// <param name="settings"> Targeted settings </param>
    /// <param name="isDefaultImport"> If the settings are imported via the class </param>
    private void DrawImportedSettingsFor(List <ValidatorSettings> settings, bool isDefaultImport = false)
    {
        if (settings.Count == 0)
            return;

        ValidatorSettings[] reversedSettings = new ValidatorSettings[settings.Count];
        settings.CopyTo(reversedSettings);

        reversedSettings = reversedSettings.Reverse().ToArray();

        for (var i = reversedSettings.Length - 1; i >= 0; i--)
        {
            ValidatorSettings setting = reversedSettings[i];
            DrawImportedSetting(setting, isDefaultImport);
        }
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

            onExport?.Invoke(path);

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
