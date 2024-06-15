﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.Windows.ValidatorViewContent
{

internal static class LeftPaneProjectMode
{
    private static LeftPaneReferences s_refs;

    public static bool CollectInvalidObjects(out List<ValidatableMonoBehaviourStatus> errors, out List<ValidatableMonoBehaviourStatus> warnings, out List <ValidatableMonoBehaviourStatus> ok)
    {
        ValidatableMonoBehaviourStatus[] behaviours = null; // TODO get stuff

        errors = new List<ValidatableMonoBehaviourStatus>();
        warnings = new List <ValidatableMonoBehaviourStatus>();
        ok = new List <ValidatableMonoBehaviourStatus>();
        
        return false; // TODO remove

        if (behaviours.Length == 0)
            return false;
        
        foreach (ValidatableMonoBehaviourStatus behaviour in behaviours)
        {
            switch (behaviour.State)
            {
                case ValidationState.Unknown: break;

                case ValidationState.Ok:
                    ok.Add(behaviour);
                    break;

                case ValidationState.Warning:
                    warnings.Add(behaviour);
                    break;

                case ValidationState.Error:
                    errors.Add(behaviour);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return true;
    }
    
    public static void SetReferences(LeftPaneReferences refs)
    {
        s_refs = refs;
    }
    
    public static void UpdateGUI()
    {
        s_refs.showChildren.value = SaveValues.Validators.ShowChildrenProject;
        
        s_refs.searchMode.style.display = DisplayStyle.Flex;
        
        s_refs.searchMode.index = SaveValues.Validators.SearchMode;
        UpdateChangeButtonParent(SaveValues.Validators.SearchMode);
        
        s_refs.btnChangePath.style.display = DisplayStyle.Flex;
        s_refs.path.style.display = DisplayStyle.Flex;
        
        UpdateFolderPath(SaveValues.Validators.SearchFolder);
    }

    private static void UpdateFolderPath(string folderPath)
    {
        if (string.IsNullOrEmpty(folderPath))
        {
            s_refs.path.text = "No folder selected";
            return;
        }

        s_refs.path.text = folderPath;
        s_refs.path.tooltip = folderPath;
    }

    private static void UpdateChangeButtonParent(int searchMode)
    {
        s_refs.parentChangePath.style.display = searchMode == 0 ? DisplayStyle.None : DisplayStyle.Flex;
    }

    public static void RegisterCallbacks()
    {
        s_refs.showChildren.RegisterValueChangedCallback(OnShowChildrenChanged);
        s_refs.searchMode.RegisterValueChangedCallback(OnSearchModeChanged);

        s_refs.btnChangePath.clicked += OnChangeButton;
    }

    public static void UnRegisterCallbacks()
    {
        s_refs.showChildren.UnregisterValueChangedCallback(OnShowChildrenChanged);
        s_refs.searchMode.UnregisterValueChangedCallback(OnSearchModeChanged);
        
        s_refs.btnChangePath.clicked -= OnChangeButton;
    }

    private static void OnChangeButton()
    {
        var path = EditorUtility.OpenFolderPanel("Select a target folder", SaveValues.Validators.SearchFolder, "");

        if (!path.IsPathInProject(out var relativePath))
        {
            EditorUtility.DisplayDialog("Invalid Path", "The selected folder is not in the project", "Ok");
            return;
        }
        
        UpdateFolderPath(relativePath);
        SaveValues.Validators.SearchFolder = relativePath;
        ValidatorView.onSettingsChanged?.Invoke();
    }

    private static void OnShowChildrenChanged(ChangeEvent <bool> evt)
    {
        if (evt.newValue == SaveValues.Validators.ShowChildrenProject)
            return;
        
        SaveValues.Validators.ShowChildrenProject = evt.newValue;
        ValidatorView.onSettingsChanged?.Invoke();
    }
    
    private static void OnSearchModeChanged(ChangeEvent<string> evt)
    {
        var newValue = TranslateSearchMode(evt.newValue);
        
        if (newValue == SaveValues.Validators.SearchMode)
            return;
        
        UpdateChangeButtonParent(newValue);
        SaveValues.Validators.SearchMode = newValue;
        ValidatorView.onSettingsChanged?.Invoke();
    }

    private static int TranslateSearchMode(string mode)
    {
        return mode switch
               {
                   "In Project" => 0,
                   "Single Folder" => 1,
                   "Including Subfolders" => 2,
                   var _ => throw new ArgumentException()
               };
    }
}

}
