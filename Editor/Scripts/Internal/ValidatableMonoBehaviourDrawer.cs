using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.Internal
{

[CustomEditor(typeof(ValidatableMonoBehaviour), true)]
public class ValidatableMonoBehaviourDrawer : UnityEditor.Editor
{
    private static readonly string[] s_exclusion = {"m_Script", "_importedSettings"};
    private static readonly string[] s_exclusionFull = {"m_Script", "_requirements"};

    private bool _listening;
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var importedSettings = ((ValidatableMonoBehaviour)target)._importedSettings != null;

        if (!importedSettings)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Export Requirements"))
            {
                try
                {
                    var path = EditorUtility.SaveFilePanelInProject("Export Requirements", "Requirements", "asset", "Hello World");

                    var requirements = CreateInstance <ValidatorSettings>();
                    requirements.SetRequirements(((ValidatableMonoBehaviour)target)._requirements);
                    
                    AssetDatabase.CreateAsset(requirements, path);
                    AssetDatabase.Refresh();

                    ((ValidatableMonoBehaviour)target)._importedSettings = requirements;
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            if (GUILayout.Button("Import Requirements"))
            {
                _listening = true;
                var controlID = GUIUtility.GetControlID (FocusType.Passive);
                EditorGUIUtility.ShowObjectPicker<ValidatorSettings> (null, false, "", controlID);
            }
            
            var commandName = Event.current.commandName;

            if (commandName == "ObjectSelectorClosed" && _listening)
            {
                _listening = false;
                ((ValidatableMonoBehaviour)target)._importedSettings = (ValidatorSettings)EditorGUIUtility.GetObjectPickerObject();   
                ((ValidatableMonoBehaviour)target).OnValidate();
            }

            EditorGUILayout.EndHorizontal();
        }

        DrawPropertiesExcluding(serializedObject, importedSettings ? s_exclusionFull : s_exclusion);

        serializedObject.ApplyModifiedProperties();
    }
}

}
