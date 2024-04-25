#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts.Internal
{

[CustomEditor(typeof(ValidatableMonoBehaviour), true)]
internal class ValidatableMonoBehaviourDrawer : UnityEditor.Editor
{
    private static readonly string[] s_exclusion = {"m_Script", "_importedSettings"};
    private static readonly string[] s_exclusionFull = {"m_Script", "_requirements"};

    private bool _listening;

    #region Public Methods

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var importedSettings = ((ValidatableMonoBehaviour)target).HasImportedSettings;

        if (!importedSettings)
        {
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
                ((ValidatableMonoBehaviour)target).SetImportedSettings((ValidatorSettings)EditorGUIUtility.GetObjectPickerObject());
                ((ValidatableMonoBehaviour)target).OnValidate();
            }
            
            EditorGUILayout.EndHorizontal();
        }

        DrawPropertiesExcluding(serializedObject, importedSettings ? s_exclusionFull : s_exclusion);

        serializedObject.ApplyModifiedProperties();
    }

    #endregion

    #region Private Methods

    private void ExportRequirements()
    {
        try
        {
            var path = EditorUtility.SaveFilePanelInProject("Export Requirements", "Requirements", "asset", "Hello World");

            if (string.IsNullOrEmpty(path))
                return;
            
            var requirements = CreateInstance <ValidatorSettings>();
            requirements.SetRequirements(((ValidatableMonoBehaviour)target).Requirements());

            AssetDatabase.CreateAsset(requirements, path);
            AssetDatabase.Refresh();

            ((ValidatableMonoBehaviour)target).SetImportedSettings(requirements);
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
