#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using MegaPint.Editor.Scripts.GUI.Utility;
using MegaPint.Editor.Scripts.Windows.ValidatorViewContent;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public static Action onRefresh;

    private static Button s_btnSceneMode;
    private static Button s_btnProjectMode;
    private static Button s_btnErrors;
    private static Button s_btnWarnings;
    private static Button s_btnOk;
    private static Button s_btnFixAll;

    private static VisualElement s_noBehaviours;
    private static VisualElement s_noSelection;

    private static ListView s_gameObjectsView;

    private static bool s_isSceneMode;

    private static VisualElement s_root;

    private static List <ValidatableMonoBehaviourStatus> s_gameObjectsItems;

    private static int s_displayedListIndex = -1;

    private static List <ValidatableMonoBehaviourStatus> s_errorGameObjects;
    private static List <ValidatableMonoBehaviourStatus> s_warningGameObjects;
    private static List <ValidatableMonoBehaviourStatus> s_okGameObjects;

    private static List <ValidatableMonoBehaviourStatus> s_displayedItems = new();

    private static ToolbarSearchField s_searchField;

    private VisualTreeAsset _baseWindow;
    private VisualTreeAsset _errorItem;
    private VisualTreeAsset _gameObjectItem;
    private VisualTreeAsset _invalidBehaviourItem;

    #region Public Methods

    public static void ScheduleRefreshCall()
    {
        s_root.schedule.Execute(
            () => {onRefresh?.Invoke();});
    }

    public static void UpdateBehaviourBasedOnState(
        ValidatableMonoBehaviourStatus behaviour,
        bool suppressGUIRefresh = false)
    {
        RemoveFromOldList(behaviour);

        switch (behaviour.State)
        {
            case ValidationState.Unknown:
                break;

            case ValidationState.Ok:
                s_okGameObjects.Add(behaviour);

                break;

            case ValidationState.Warning:
                s_warningGameObjects.Add(behaviour);

                break;

            case ValidationState.Error:
                s_errorGameObjects.Add(behaviour);

                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        if (!suppressGUIRefresh)
            UpdateLeftPaneGUI();
    }

    public override EditorWindowBase ShowWindow()
    {
        titleContent.text = "Validator View";

        minSize = new Vector2(700, 350);

        s_isSceneMode = true;

        if (!SaveValues.Validators.ApplyPSValidatorView)
            return this;

        this.CenterOnMainWin(800, 450);
        SaveValues.Validators.ApplyPSValidatorView = false;

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

        s_root = rootVisualElement;

        VisualElement content = GUIUtility.Instantiate(_baseWindow);

        var leftPane = content.Q <VisualElement>("LeftPane");

        s_btnSceneMode = leftPane.Q <Button>("BTN_Scene");
        s_btnProjectMode = leftPane.Q <Button>("BTN_Project");
        s_btnFixAll = leftPane.Q <Button>("BTN_FixAll");

        s_btnErrors = leftPane.Q <Button>("BTN_Errors");
        s_btnWarnings = leftPane.Q <Button>("BTN_Warnings");
        s_btnOk = leftPane.Q <Button>("BTN_Ok");

        s_noBehaviours = leftPane.Q <VisualElement>("NoBehaviours");
        s_noSelection = leftPane.Q <VisualElement>("NoSelection");

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

        s_root.schedule.Execute(
            () => {ChangeMode(s_isSceneMode);});

        s_root.Add(content);
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
        onRefresh += UpdateLeftPane;

        s_btnSceneMode.clickable = new Clickable(() => {ChangeMode(true);});
        s_btnProjectMode.clickable = new Clickable(() => {ChangeMode(false);});

        s_btnFixAll.clicked += FixAll;

        s_btnErrors.clicked += OnErrorButton;
        s_btnWarnings.clicked += OnWarningButton;
        s_btnOk.clicked += OnOkButton;

        s_searchField.RegisterValueChangedCallback(OnSearchFieldChanged);

        s_gameObjectsView.itemsChosen += OnGameObjectSelectionConfirmed;
        s_gameObjectsView.selectedIndicesChanged += OnGameObjectSelected;

        s_gameObjectsView.makeItem = () => GUIUtility.Instantiate(_gameObjectItem);

        s_gameObjectsView.bindItem = (element, i) =>
        {
            var status = (ValidatableMonoBehaviourStatus)s_gameObjectsView.itemsSource[i];
            var label = element.Q <Label>("ObjectName");

            label.text = status.gameObject.name;
            label.tooltip = GetParentPath(status.transform);

            var isMissingFixAction = status.invalidBehaviours.Any(
                behaviour => behaviour.errors.Any(error => error.fixAction == null));

            var noFixAction = element.Q <VisualElement>("NoFixAction");
            noFixAction.style.display = isMissingFixAction ? DisplayStyle.Flex : DisplayStyle.None;
        };

        EditorSceneManager.sceneOpened += OnSceneLoaded;
        EditorSceneManager.sceneClosed += OnSceneClosed;
    }

    private static void OnSceneClosed(Scene scene)
    {
        if (s_isSceneMode)
            UpdateLeftPane();
    }

    private static void OnSceneLoaded(Scene scene, OpenSceneMode mode)
    {
        if (s_isSceneMode)
            UpdateLeftPane();
    }

    protected override void UnRegisterCallbacks()
    {
        onRefresh -= UpdateLeftPane;

        s_btnSceneMode.clickable = null;
        s_btnProjectMode.clickable = null;

        s_btnFixAll.clicked -= FixAll;

        s_btnErrors.clicked -= OnErrorButton;
        s_btnWarnings.clicked -= OnWarningButton;
        s_btnOk.clicked -= OnOkButton;

        s_searchField.UnregisterValueChangedCallback(OnSearchFieldChanged);

        s_gameObjectsView.itemsChosen -= OnGameObjectSelectionConfirmed;
        s_gameObjectsView.selectedIndicesChanged -= OnGameObjectSelected;

        EditorSceneManager.sceneOpened -= OnSceneLoaded;
        EditorSceneManager.sceneClosed -= OnSceneClosed;
    }

    #endregion

    #region Private Methods

    private static bool CollectInvalidGameObjects(
        out List <ValidatableMonoBehaviourStatus> errors,
        out List <ValidatableMonoBehaviourStatus> warnings,
        out List <ValidatableMonoBehaviourStatus> ok)
    {
        if (s_isSceneMode)
        {
            if (!LeftPaneSceneMode.CollectInvalidObjects(out errors, out warnings, out ok))
                return false;
        }
        else
        {
            if (!LeftPaneProjectMode.CollectInvalidObjects(out errors, out warnings, out ok))
                return false;
        }

        return true;
    }

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
        UpdateFixAllButton();
        s_gameObjectsView.ClearSelection();
    }

    private static ValidatableMonoBehaviourStatus[] GetFixableBehaviours()
    {
        return s_displayedItems.Where(
                                    gameObject =>
                                        gameObject.invalidBehaviours.Any(
                                            behaviour => behaviour.errors.Any(error => error.fixAction != null))).
                                ToArray();
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

    private static void OnErrorButton()
    {
        s_gameObjectsItems = s_errorGameObjects;
        s_displayedListIndex = 0;

        DisplayBySearchField();
        UpdateBehaviourButtons();
        RightPane.Clear();
    }

    private static void OnOkButton()
    {
        s_gameObjectsItems = s_okGameObjects;
        s_displayedListIndex = 2;

        DisplayBySearchField();
        UpdateBehaviourButtons();
        RightPane.Clear();
    }

    private static void OnWarningButton()
    {
        s_gameObjectsItems = s_warningGameObjects;
        s_displayedListIndex = 1;

        DisplayBySearchField();
        UpdateBehaviourButtons();
        RightPane.Clear();
    }

    private static void RemoveFromOldList(ValidatableMonoBehaviourStatus behaviour)
    {
        if (s_errorGameObjects.Contains(behaviour))
        {
            s_errorGameObjects.Remove(behaviour);

            if (s_errorGameObjects.Count == 0)
                s_displayedListIndex = -1;
        }
        else if (s_warningGameObjects.Contains(behaviour))
        {
            s_warningGameObjects.Remove(behaviour);

            if (s_warningGameObjects.Count == 0)
                s_displayedListIndex = -1;
        }
        else if (s_okGameObjects.Contains(behaviour))
        {
            s_okGameObjects.Remove(behaviour);

            if (s_okGameObjects.Count == 0)
                s_displayedListIndex = -1;
        }
    }

    private static void ResetDisplayedItems()
    {
        s_gameObjectsItems = null;
        DisplayBySearchField();
    }

    private static void UpdateBehaviourButtons()
    {
        GUIUtility.ToggleActiveButtonInGroup(s_displayedListIndex, s_btnErrors, s_btnWarnings, s_btnOk);
        UpdateNoSelectionVisibility();
    }

    private static void UpdateBehavioursBasedOnState(ValidatableMonoBehaviourStatus[] behaviours)
    {
        foreach (ValidatableMonoBehaviourStatus behaviour in behaviours)
            UpdateBehaviourBasedOnState(behaviour, true);

        UpdateLeftPaneGUI();
    }

    private static void UpdateFixAllButton()
    {
        if (s_displayedListIndex == -1 || s_displayedItems.Count == 2)
        {
            s_btnFixAll.style.display = DisplayStyle.None;

            return;
        }

        var hasFixAction = GetFixableBehaviours().Length > 0;

        s_btnFixAll.style.display = hasFixAction ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private static void UpdateGameObjectsListViewVisibility()
    {
        s_gameObjectsView.style.display = s_displayedItems.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private static void UpdateLeftPane()
    {
        GUIUtility.ToggleActiveButtonInGroup(s_isSceneMode ? 0 : 1, s_btnSceneMode, s_btnProjectMode);
        GUIUtility.ToggleActiveButtonInGroup(-1, s_btnErrors, s_btnWarnings, s_btnOk);

        if (s_isSceneMode)
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
            out s_errorGameObjects,
            out s_warningGameObjects,
            out s_okGameObjects);

        s_noBehaviours.style.display = hasBehaviours ? DisplayStyle.None : DisplayStyle.Flex;

        s_displayedListIndex = -1;

        if (!hasBehaviours)
        {
            s_noSelection.style.display = DisplayStyle.None;
            ResetDisplayedItems();

            return;
        }

        UpdateLeftPaneGUI();
    }

    private static void UpdateLeftPaneGUI()
    {
        ResetDisplayedItems();

        s_btnErrors.style.display = s_errorGameObjects.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;
        s_btnWarnings.style.display = s_warningGameObjects.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;
        s_btnOk.style.display = s_okGameObjects.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;

        UpdateNoSelectionVisibility();

        s_btnErrors.text = $"Errors ({s_errorGameObjects.Count})";
        s_btnWarnings.text = $"Warnings ({s_warningGameObjects.Count})";
        s_btnOk.text = $"Ok ({s_okGameObjects.Count})";

        if (s_displayedListIndex < 0)
            return;

        s_gameObjectsItems = s_displayedListIndex switch
                             {
                                 0 => s_errorGameObjects,
                                 1 => s_warningGameObjects,
                                 2 => s_okGameObjects,
                                 var _ => s_gameObjectsItems
                             };

        DisplayBySearchField();
    }

    private static void UpdateNoSelectionVisibility()
    {
        s_noSelection.style.display = s_displayedListIndex >= 0 ? DisplayStyle.None : DisplayStyle.Flex;
    }

    private void ChangeMode(bool isSceneMode)
    {
        s_isSceneMode = isSceneMode;
        UpdateLeftPane();

        RightPane.Clear();
    }

    private void FixAll()
    {
        ValidatableMonoBehaviourStatus[] behaviours = GetFixableBehaviours();

        foreach (ValidatableMonoBehaviourStatus behaviour in behaviours)
        {
            foreach (InvalidBehaviour invalidBehaviour in behaviour.invalidBehaviours)
            {
                foreach (ValidationError error in invalidBehaviour.errors)
                    error.fixAction?.Invoke(error.gameObject);
            }

            behaviour.ValidateStatus();
        }

        RightPane.Clear();
        UpdateBehavioursBasedOnState(behaviours);
    }

    private void OnGameObjectSelected(IEnumerable <int> _)
    {
        if (s_gameObjectsView.selectedItem == null)
            return;

        var status = (ValidatableMonoBehaviourStatus)s_gameObjectsView.selectedItem;

        var parentPath = GetParentPath(status.transform);
        var path = s_isSceneMode ? parentPath : $"{AssetDatabase.GetAssetPath(status)} => {parentPath}";

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

    #endregion
}

}
#endif
