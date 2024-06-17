#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MegaPint.Editor.Scripts.Windows.ValidatorViewContent;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using GUIUtility = MegaPint.Editor.Scripts.GUI.Utility.GUIUtility;

namespace MegaPint.Editor.Scripts.Windows
{

/// <summary>
///     This window is used to display all <see cref="ValidatableMonoBehaviour" /> in either the scene or the whole
///     project and to show their status as well as an option to automatically fix any occuring issues
/// </summary>
internal class ValidatorView : EditorWindowBase
{
    public static Action onRefresh; // TODO reload with new search on this event
    
    private VisualTreeAsset _baseWindow;
    private VisualTreeAsset _gameObjectItem;
    private VisualTreeAsset _invalidBehaviourItem;
    private VisualTreeAsset _errorItem;

    private static Button _btnSceneMode;
    private static Button _btnProjectMode;
    private static Button s_btnErrors;
    private static Button s_btnWarnings;
    private static Button s_btnOk;

    private static VisualElement s_noBehaviours;
    private static VisualElement s_noSelection;

    private static ListView s_gameObjectsView;

    private static bool _isSceneMode;

    #region Public Methods

    public override EditorWindowBase ShowWindow()
    {
        titleContent.text = "Validator View";

        _isSceneMode = true;

        return this;
    }

    #endregion

    #region Protected Methods

    protected override string BasePath()
    {
        return Constants.Validators.UserInterface.ValidatorView;
    }

    protected override void CreateGUI()
    {
        base.CreateGUI();

        VisualElement root = rootVisualElement;

        VisualElement content = GUIUtility.Instantiate(_baseWindow);

        var leftPane = content.Q <VisualElement>("LeftPane");
        
        _btnSceneMode = leftPane.Q <Button>("BTN_Scene");
        _btnProjectMode = leftPane.Q <Button>("BTN_Project");

        s_btnErrors = leftPane.Q <Button>("BTN_Errors");
        s_btnErrors.style.display = DisplayStyle.None;
        
        s_btnWarnings = leftPane.Q <Button>("BTN_Warnings");
        s_btnWarnings.style.display = DisplayStyle.None;
        
        s_btnOk = leftPane.Q <Button>("BTN_Ok");
        s_btnOk.style.display = DisplayStyle.None;

        s_noBehaviours = leftPane.Q <VisualElement>("NoBehaviours");
        s_noBehaviours.style.display = DisplayStyle.None;
        
        s_noSelection = leftPane.Q <VisualElement>("NoSelection");
        s_noSelection.style.display = DisplayStyle.None;
        
        s_gameObjectsView = leftPane.Q <ListView>("GameObjects");

        s_searchField = leftPane.Q <ToolbarSearchField>("SearchField");

        var refs = new LeftPaneReferences
        {
            btnChangePath = leftPane.Q <Button>("BTN_Change"),
            path = leftPane.Q <Label>("FolderPath"),
            parentChangePath = leftPane.Q <VisualElement>("ChangeButtonParent"),
            searchMode = leftPane.Q <DropdownField>("ProjectSearchMode"),
            showChildren = leftPane.Q <Toggle>("ShowChildren")
        };
        
        LeftPaneSceneMode.SetReferences(refs);
        LeftPaneProjectMode.SetReferences(refs);

        RightPane.CreateGUI(content.Q <VisualElement>("RightPane"), _invalidBehaviourItem, _errorItem);
        
        RegisterCallbacks();

        root.schedule.Execute(
            () =>
            {
                ChangeMode(_isSceneMode);
            });

        root.Add(content);
    }

    protected override bool LoadResources()
    {
        _baseWindow = Resources.Load <VisualTreeAsset>(BasePath());
        _gameObjectItem = Resources.Load <VisualTreeAsset>(Constants.Validators.UserInterface.ValidatorViewItem);
        _invalidBehaviourItem = Resources.Load <VisualTreeAsset>(Constants.Validators.UserInterface.StatusBehaviour);
        _errorItem = Resources.Load <VisualTreeAsset>(Constants.Validators.UserInterface.StatusError);

        return _baseWindow != null && _gameObjectItem != null;
    }

    protected override void RegisterCallbacks()
    {
        _btnSceneMode.clickable = new Clickable(() => {ChangeMode(true);});
        _btnProjectMode.clickable = new Clickable(() => {ChangeMode(false);});

        s_searchField.RegisterValueChangedCallback(OnSearchFieldChanged);

        s_gameObjectsView.itemsChosen += OnGameObjectSelectionConfirmed;
        s_gameObjectsView.selectedIndicesChanged += OnGameObjectSelected;
        
        s_gameObjectsView.makeItem = () => GUIUtility.Instantiate(_gameObjectItem);

        s_gameObjectsView.bindItem = (element, i) =>
        {
            var status = (ValidatableMonoBehaviourStatus) s_gameObjectsView.itemsSource[i];
            var label = element.Q <Label>("ObjectName"); 
            
            label.text = status.gameObject.name;
            label.tooltip = GetParentPath(status.transform);
        };
    }

    protected override void UnRegisterCallbacks()
    {
        _btnSceneMode.clickable = null;
        _btnProjectMode.clickable = null;
        
        s_searchField.UnregisterValueChangedCallback(OnSearchFieldChanged);
        
        s_gameObjectsView.itemsChosen -= OnGameObjectSelectionConfirmed;
        s_gameObjectsView.selectedIndicesChanged -= OnGameObjectSelected;
    }

    private void OnGameObjectSelected(IEnumerable <int> _)
    {
        if (s_gameObjectsView.selectedItem == null)
            return;
        
        var status = (ValidatableMonoBehaviourStatus)s_gameObjectsView.selectedItem;

        var path = _isSceneMode ? GetParentPath(status.transform) : AssetDatabase.GetAssetPath(status);

        RightPane.Display(status, path);
    }

    private void OnGameObjectSelectionConfirmed(IEnumerable <object> _)
    {
        var status = (ValidatableMonoBehaviourStatus)s_gameObjectsView.selectedItem;
        
        Selection.activeObject = status;
    }

    private void OnSearchFieldChanged(ChangeEvent <string> _)
    {
        DisplayBySearchField();
    }

    private void ChangeMode(bool isSceneMode)
    {
        _isSceneMode = isSceneMode;
        UpdateLeftPane();
        
        RightPane.Clear();
    }

    private static void UpdateLeftPane()
    {
        GUIUtility.ToggleActiveButtonInGroup(_isSceneMode ? 0 : 1, _btnSceneMode, _btnProjectMode);
        GUIUtility.ToggleActiveButtonInGroup(-1, s_btnErrors, s_btnWarnings, s_btnOk);

        if (_isSceneMode)
        {
            LeftPaneSceneMode.UpdateGUI();
            
            LeftPaneProjectMode.UnRegisterCallbacks();
            LeftPaneSceneMode.RegisterCallbacks();
        }
        else
        {
            LeftPaneProjectMode.UpdateGUI();
            
            LeftPaneSceneMode.UnRegisterCallbacks();
            LeftPaneProjectMode.RegisterCallbacks();
        }

        var hasBehaviours = CollectInvalidGameObjects(
            out var hasErrors,
            out List <ValidatableMonoBehaviourStatus> errors,
            out var hasWarnings,
            out List <ValidatableMonoBehaviourStatus> warnings,
            out var hasOk,
            out List <ValidatableMonoBehaviourStatus> ok);
        
        s_noBehaviours.style.display = hasBehaviours ? DisplayStyle.None : DisplayStyle.Flex;

        s_btnErrors.style.display = hasErrors ? DisplayStyle.Flex : DisplayStyle.None;
        s_btnWarnings.style.display = hasWarnings ? DisplayStyle.Flex : DisplayStyle.None;
        s_btnOk.style.display = hasOk ? DisplayStyle.Flex : DisplayStyle.None;

        ResetDisplayedItems();
        UpdateGameObjectsListViewVisibility();

        if (!hasBehaviours)
        {
            s_noSelection.style.display = DisplayStyle.None;
            return;
        }

        UpdateNoSelectionVisibility();

        s_btnErrors.text = $"Errors ({errors.Count})";
        s_btnWarnings.text = $"Warnings ({warnings.Count})";
        s_btnOk.text = $"Ok ({ok.Count})";
        
        ChangeErrorButtonCallback(errors);
        ChangeWarningButtonCallback(warnings);
        ChangeOkButtonCallback(ok);
    }

    private static void UpdateNoSelectionVisibility()
    {
        s_noSelection.style.display = s_gameObjectsItems is {Count: > 0} ? DisplayStyle.None : DisplayStyle.Flex;
    }

    private static void UpdateGameObjectsListViewVisibility()
    {
        s_gameObjectsView.style.display = s_displayedItems.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;
    }
    
    private static void ResetDisplayedItems()
    {
        s_gameObjectsItems = null;
        DisplayBySearchField();
    }

    private static void UpdateBehaviourButtons(int index)
    {
        GUIUtility.ToggleActiveButtonInGroup(index, s_btnErrors, s_btnWarnings, s_btnOk);
        UpdateNoSelectionVisibility();
    }
    
    private static void ChangeErrorButtonCallback(List <ValidatableMonoBehaviourStatus> errors)
    {
        s_btnErrors.clickable = new Clickable(() =>
        {
            s_gameObjectsItems = errors;
            DisplayBySearchField();
            UpdateBehaviourButtons(0);
        });
    }

    private static List <ValidatableMonoBehaviourStatus> s_gameObjectsItems;

    private static void ChangeWarningButtonCallback(List <ValidatableMonoBehaviourStatus> warnings)
    {
        s_btnWarnings.clickable = new Clickable(() =>
        {
            s_gameObjectsItems = warnings;
            DisplayBySearchField();
            UpdateBehaviourButtons(1);
        });
    }
    
    private static void ChangeOkButtonCallback(List <ValidatableMonoBehaviourStatus> ok)
    {
        s_btnOk.clickable = new Clickable(() =>
        {
            s_gameObjectsItems = ok;
            DisplayBySearchField();
            UpdateBehaviourButtons(2);
        });
    }

    #endregion

    #region Private Methods

    private static bool CollectInvalidGameObjects(out bool hasErrors, out List <ValidatableMonoBehaviourStatus> errors, out bool hasWarnings, out List <ValidatableMonoBehaviourStatus> warnings, out bool hasOk, out List <ValidatableMonoBehaviourStatus> ok)
    {
        hasErrors = false;
        hasWarnings = false;
        hasOk = false;
        
        if (_isSceneMode)
        {
            if (!LeftPaneSceneMode.CollectInvalidObjects(out errors, out warnings, out ok))
                return false;
        }
        else
        {
            if (!LeftPaneProjectMode.CollectInvalidObjects(out errors, out warnings, out ok))
                return false;
        }
        
        hasErrors = errors.Count > 0;
        hasWarnings = warnings.Count > 0;
        hasOk = ok.Count > 0;
            
        return true;
    }

    /// <summary> Get path of the transform in their parent hierarchy </summary>
    /// <param name="startTransform"> Transform the path starts at </param>
    /// <returns> Path in the local hierarchy of the given transform </returns>
    private static string GetParentPath(Transform startTransform)
    {
        Transform transform = startTransform;
        var path = transform.name;

        if (transform.parent == null)
            return path;

        while (transform.parent != null)
        {
            transform = transform.parent;
            path = $"{transform.name}/{path}";
        }

        return path;
    }





    private static List <ValidatableMonoBehaviourStatus> s_displayedItems = new();

    private static ToolbarSearchField s_searchField;
    
    /// <summary> Filter the displayed behaviours based on the content of the search field </summary>
    private static void DisplayBySearchField()
    {
        if (s_gameObjectsItems is not {Count: > 0})
            s_displayedItems = new List <ValidatableMonoBehaviourStatus>();
        else
        {
            if (string.IsNullOrEmpty(s_searchField.value))
                s_displayedItems = s_gameObjectsItems;
            else
            {
                s_displayedItems = s_gameObjectsItems.Where(
                                                          behaviour =>
                                                              behaviour.gameObject.name.ToLower().
                                                                        Contains(s_searchField.value.ToLower())).
                                                      ToList();
            }
        }

        s_displayedItems.Sort();
        s_gameObjectsView.itemsSource = s_displayedItems;

        UpdateGameObjectsListViewVisibility();
        s_gameObjectsView.ClearSelection();
    }

    #endregion
}

}
#endif
