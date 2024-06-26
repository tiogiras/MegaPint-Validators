﻿#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MegaPint.Editor.Scripts.Windows.ValidatorViewContent
{

/// <summary> Handles the display of the left pane of <see cref="ValidatorView" /> in project mode </summary>
internal static class LeftPaneProjectMode
{
    private static LeftPaneReferences s_refs;

    private static bool s_canBeChanged;

    #region Public Methods

    /// <summary> Collect all <see cref="ValidatableMonoBehaviourStatus" /> based on the current settings </summary>
    /// <param name="errors"> Found behaviours with severity = error </param>
    /// <param name="warnings"> Found behaviours with severity = warning </param>
    /// <param name="ok"> Found behaviours with severity = ok </param>
    /// <returns> If any behaviours where found </returns>
    /// <exception cref="ArgumentOutOfRangeException"> State of the behaviour not found </exception>
    public static bool CollectValidatableObjects(
        out List <ValidatableMonoBehaviourStatus> errors,
        out List <ValidatableMonoBehaviourStatus> warnings,
        out List <ValidatableMonoBehaviourStatus> ok)
    {
        ValidatableMonoBehaviourStatus[] behaviours = CollectBehaviours();

        errors = new List <ValidatableMonoBehaviourStatus>();
        warnings = new List <ValidatableMonoBehaviourStatus>();
        ok = new List <ValidatableMonoBehaviourStatus>();

        s_canBeChanged = true;

        if (behaviours is not {Length: > 0})
            return false;

        foreach (ValidatableMonoBehaviourStatus behaviour in behaviours)
        {
            behaviour.ValidateStatus();

            switch (behaviour.State)
            {
                case ValidationState.Unknown:
                    break;

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

    /// <summary> Register all callbacks </summary>
    public static void RegisterCallbacks()
    {
        s_refs.showChildren.RegisterValueChangedCallback(OnShowChildrenChanged);
        s_refs.searchMode.RegisterValueChangedCallback(OnSearchModeChanged);

        s_refs.btnChangePath.clicked += OnChangeButton;
    }

    /// <summary> Set the values of the visual references </summary>
    /// <param name="refs"> Visual references </param>
    public static void SetReferences(LeftPaneReferences refs)
    {
        s_refs = refs;
    }

    /// <summary> Unregister all callbacks </summary>
    public static void UnRegisterCallbacks()
    {
        s_refs.showChildren.UnregisterValueChangedCallback(OnShowChildrenChanged);
        s_refs.searchMode.UnregisterValueChangedCallback(OnSearchModeChanged);

        s_refs.btnChangePath.clicked -= OnChangeButton;
    }

    /// <summary> Update the gui </summary>
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

    #endregion

    #region Private Methods

    /// <summary> Collect all <see cref="ValidatableMonoBehaviourStatus" /> </summary>
    /// <returns> All found behaviours </returns>
    private static ValidatableMonoBehaviourStatus[] CollectBehaviours()
    {
        string[] guids;

        switch (SaveValues.Validators.SearchMode)
        {
            case 0:
                guids = AssetDatabase.FindAssets("t:prefab");

                break;

            case 1 or 2:

                guids = CollectGUIDsInFolder();

                break;

            default:
                return null;
        }

        if (guids is not {Length: > 0})
            return null;

        IEnumerable <ValidatableMonoBehaviourStatus> behaviours = ConvertGUIDsToValidatableMonoBehaviours(guids).
            Where(behaviour => !IsChildValidation(behaviour.transform));

        return behaviours.ToArray();
    }

    /// <summary> Collect all guids in the selected folder </summary>
    /// <returns> List of found guids </returns>
    private static string[] CollectGUIDsInFolder()
    {
        var searchFolder = SaveValues.Validators.SearchFolder;

        if (string.IsNullOrEmpty(searchFolder))
            return null;

        if (SaveValues.Validators.SearchMode == 1)
        {
            var path = Path.Combine(Application.dataPath, searchFolder[(searchFolder.Length > 6 ? 7 : 6)..]);
            var files = Directory.GetFiles(path, "*.prefab");

            return files.Select(file => AssetDatabase.AssetPathToGUID(file.Replace(Application.dataPath, "Assets"))).
                         ToArray();
        }

        List <string> folders = new() {searchFolder};

        folders.AddRange(GetSubFolders(searchFolder));

        return AssetDatabase.FindAssets("t:prefab", folders.ToArray());
    }

    /// <summary> Filter and collect all <see cref="ValidatableMonoBehaviour" /> of the guids </summary>
    /// <param name="guids"> Guids to filter </param>
    private static IEnumerable <ValidatableMonoBehaviourStatus> ConvertGUIDsToValidatableMonoBehaviours(string[] guids)
    {
        if (guids is not {Length: > 0})
            return null;

        List <ValidatableMonoBehaviourStatus> validatableMonoBehaviours = new();

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath <GameObject>(path);

            if (SaveValues.Validators.ShowChildrenProject)
            {
                ValidatableMonoBehaviourStatus[] validators =
                    asset.GetComponentsInChildren <ValidatableMonoBehaviourStatus>(true);

                if (validators is {Length: > 0})
                    validatableMonoBehaviours.AddRange(validators);

                continue;
            }

            if (asset.TryGetComponent(out ValidatableMonoBehaviourStatus validator))
                validatableMonoBehaviours.Add(validator);
        }

        return validatableMonoBehaviours.ToArray();
    }

    /// <summary> Get all sub folders in the given folder </summary>
    /// <param name="folderPath"> Path to the targeted folder </param>
    /// <returns> List of all found sub folders of the targeted folder </returns>
    private static IEnumerable <string> GetSubFolders(string folderPath)
    {
        List <string> subFolders = new();

        subFolders.AddRange(AssetDatabase.GetSubFolders(folderPath).ToList());

        if (subFolders.Count == 0)
            return subFolders;

        for (var i = 0; i < subFolders.Count; i++)
        {
            var subFolder = subFolders[i];
            subFolders.AddRange(GetSubFolders(subFolder));
        }

        return subFolders;
    }

    /// <summary> If this object is required for a child validation in any parent </summary>
    /// <param name="transform"> Transform of the targeted object </param>
    /// <returns> If the object is validated by a parent </returns>
    private static bool IsChildValidation(Transform transform)
    {
        if (transform.parent == null)
            return false;

        ValidatableMonoBehaviourStatus[] behaviours =
            transform.parent.GetComponentsInParent <ValidatableMonoBehaviourStatus>();

        return behaviours.Length != 0 && behaviours.Any(behaviour => behaviour.ValidatesChildren());
    }

    /// <summary> Change Button Callback </summary>
    private static void OnChangeButton()
    {
        if (!s_canBeChanged)
            return;

        var path = EditorUtility.OpenFolderPanel("Select a target folder", SaveValues.Validators.SearchFolder, "");

        if (!path.IsPathInProject(out var relativePath))
        {
            EditorUtility.DisplayDialog("Invalid Path", "The selected folder is not in the project", "Ok");

            return;
        }

        UpdateFolderPath(relativePath);
        SaveValues.Validators.SearchFolder = relativePath;
        s_canBeChanged = false;

        ValidatorView.ScheduleRefreshCall();
    }

    /// <summary> Searchbar callback </summary>
    /// <param name="evt"> Callback event </param>
    private static void OnSearchModeChanged(ChangeEvent <string> evt)
    {
        var newValue = TranslateSearchMode(evt.newValue);

        if (newValue == SaveValues.Validators.SearchMode)
            return;

        UpdateChangeButtonParent(newValue);
        SaveValues.Validators.SearchMode = newValue;
        ValidatorView.onRefresh?.Invoke();
    }

    /// <summary> Show children toggle callback </summary>
    /// <param name="evt"> Callback event </param>
    private static void OnShowChildrenChanged(ChangeEvent <bool> evt)
    {
        if (evt.newValue == SaveValues.Validators.ShowChildrenProject)
            return;

        SaveValues.Validators.ShowChildrenProject = evt.newValue;
        ValidatorView.onRefresh?.Invoke();
    }

    /// <summary> Translate the searchMode string to the searchMode index </summary>
    /// <param name="mode"> Source searchMode string </param>
    /// <returns> SearchMode as index </returns>
    /// <exception cref="ArgumentException"> SearchMode not found </exception>
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

    /// <summary> Update the visibility of the change button parent visualElement </summary>
    /// <param name="searchMode"> Current searchMode </param>
    private static void UpdateChangeButtonParent(int searchMode)
    {
        s_refs.parentChangePath.style.display = searchMode == 0 ? DisplayStyle.None : DisplayStyle.Flex;
    }

    /// <summary> Update the folder path visuals </summary>
    /// <param name="folderPath"> New folderPath </param>
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

    #endregion
}

}
#endif
