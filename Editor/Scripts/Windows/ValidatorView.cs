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
    private static Button s_btnRefresh;

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

    /// <summary> Schedule the execution of a refresh of the left pane </summary>
    public static void ScheduleRefreshCall()
    {
        s_root.schedule.Execute(
            () => {onRefresh?.Invoke();});
    }

    /// <summary> Move a behaviour to a new list based on it's state </summary>
    /// <param name="behaviour"> Targeted behaviour </param>
    /// <param name="suppressGUIRefresh"> If true the gui will not be updated automatically </param>
    /// <exception cref="ArgumentOutOfRangeException"> Behaviour state not found </exception>
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
        s_btnRefresh = leftPane.Q <Button>("BTN_Refresh");

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
        s_btnRefresh.clicked += UpdateLeftPane;

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

    protected override void UnRegisterCallbacks()
    {
        onRefresh -= UpdateLeftPane;

        s_btnSceneMode.clickable = null;
        s_btnProjectMode.clickable = null;

        s_btnFixAll.clicked -= FixAll;
        s_btnRefresh.clicked -= UpdateLeftPane;

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

    /// <summary> Change the current mode (scene/project) </summary>
    /// <param name="isSceneMode"> New mode </param>
    private static void ChangeMode(bool isSceneMode)
    {
        s_isSceneMode = isSceneMode;
        UpdateLeftPane();

        RightPane.Clear();
    }

    /// <summary> Collect all <see cref="ValidatableMonoBehaviourStatus" /> based on the current settings </summary>
    /// <param name="errors"> Found behaviours with the severity = error </param>
    /// <param name="warnings"> Found behaviours with the severity = warning </param>
    /// <param name="ok"> Found behaviours with the severity = ok </param>
    /// <returns> If any behaviours where found </returns>
    private static bool CollectValidatableMonoBehaviours(
        out List <ValidatableMonoBehaviourStatus> errors,
        out List <ValidatableMonoBehaviourStatus> warnings,
        out List <ValidatableMonoBehaviourStatus> ok)
    {
        if (s_isSceneMode)
        {
            if (!LeftPaneSceneMode.CollectValidatableObjects(out errors, out warnings, out ok))
                return false;
        }
        else
        {
            if (!LeftPaneProjectMode.CollectValidatableObjects(out errors, out warnings, out ok))
                return false;
        }

        return true;
    }

    /// <summary> Filter the displayed behaviours based on the content of the search field </summary>
    private static void DisplayBySearchField()
    {
        StopListeningToValidationEvents();

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
        ListenToValidationEvents();

        s_gameObjectsView.itemsSource = s_displayedItems;

        UpdateGameObjectsListViewVisibility();
        UpdateFixAllButton();
        s_gameObjectsView.ClearSelection();
    }

    /// <summary> Fix all displayed behaviours that can be fixed </summary>
    private static void FixAll()
    {
        ValidatableMonoBehaviourStatus[] behaviours = GetFixableBehaviours();

        foreach (ValidatableMonoBehaviourStatus behaviour in behaviours)
            behaviour.FixAll();

        RightPane.Clear();
        UpdateBehavioursBasedOnState(behaviours);
    }

    /// <summary> Find all behaviours with a fixAction in the displayed behaviours </summary>
    /// <returns> All fixable currently displayed behaviours </returns>
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

    /// <summary> Listen to all validation events of the displayed behaviours </summary>
    private static void ListenToValidationEvents()
    {
        if (s_displayedItems.Count == 0)
            return;

        foreach (ValidatableMonoBehaviourStatus item in s_displayedItems)
            item.onStatusChanged += OnStatusChanged;
    }

    /// <summary> Error button callback </summary>
    private static void OnErrorButton()
    {
        s_gameObjectsItems = s_errorGameObjects;
        s_displayedListIndex = 0;

        DisplayBySearchField();
        UpdateBehaviourButtons();
        RightPane.Clear();
    }

    /// <summary> List selection callback </summary>
    /// <param name="_"> Callback event </param>
    private static void OnGameObjectSelected(IEnumerable <int> _)
    {
        if (s_gameObjectsView.selectedItem == null)
            return;

        var status = (ValidatableMonoBehaviourStatus)s_gameObjectsView.selectedItem;

        var parentPath = GetParentPath(status.transform);
        var path = s_isSceneMode ? parentPath : $"{AssetDatabase.GetAssetPath(status)} => {parentPath}";

        RightPane.Display(status, path);
    }

    /// <summary> List selection confirmed callback </summary>
    /// <param name="_"> Callback event </param>
    private static void OnGameObjectSelectionConfirmed(IEnumerable <object> _)
    {
        var status = (ValidatableMonoBehaviourStatus)s_gameObjectsView.selectedItem;

        Selection.activeObject = status;
    }

    /// <summary> Ok button callback </summary>
    private static void OnOkButton()
    {
        s_gameObjectsItems = s_okGameObjects;
        s_displayedListIndex = 2;

        DisplayBySearchField();
        UpdateBehaviourButtons();
        RightPane.Clear();
    }

    /// <summary> Scene closed event callback </summary>
    /// <param name="scene"> Closed scene </param>
    private static void OnSceneClosed(Scene scene)
    {
        if (s_isSceneMode)
            UpdateLeftPane();
    }

    /// <summary> Scene loaded event callback </summary>
    /// <param name="scene"> Loaded scene </param>
    /// <param name="mode"> Mode the scene was loaded in </param>
    private static void OnSceneLoaded(Scene scene, OpenSceneMode mode)
    {
        if (s_isSceneMode)
            UpdateLeftPane();
    }

    /// <summary> Searchbar callback </summary>
    /// <param name="_"> Callback event </param>
    private static void OnSearchFieldChanged(ChangeEvent <string> _)
    {
        DisplayBySearchField();
    }

    /// <summary> Validation event callback </summary>
    /// <param name="behaviour"> Validated behaviour </param>
    private static void OnStatusChanged(ValidatableMonoBehaviourStatus behaviour)
    {
        UpdateBehaviourBasedOnState(behaviour);
    }

    /// <summary> Warning button callback </summary>
    private static void OnWarningButton()
    {
        s_gameObjectsItems = s_warningGameObjects;
        s_displayedListIndex = 1;

        DisplayBySearchField();
        UpdateBehaviourButtons();
        RightPane.Clear();
    }

    /// <summary> Remove the behaviour from it's old list </summary>
    /// <param name="behaviour"> Targeted behaviour </param>
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

    /// <summary> Reset the displayed items </summary>
    private static void ResetDisplayedItems()
    {
        s_gameObjectsItems = null;
        DisplayBySearchField();
    }

    /// <summary> Stop listening to all validation events of the displayed behaviours </summary>
    private static void StopListeningToValidationEvents()
    {
        if (s_displayedItems.Count == 0)
            return;

        foreach (ValidatableMonoBehaviourStatus item in s_displayedItems)
            item.onStatusChanged -= OnStatusChanged;
    }

    /// <summary> Update the visuals of the behaviour buttons </summary>
    private static void UpdateBehaviourButtons()
    {
        GUIUtility.ToggleActiveButtonInGroup(s_displayedListIndex, s_btnErrors, s_btnWarnings, s_btnOk);
        UpdateNoSelectionVisibility();
    }

    /// <summary> Update many behaviours based on their current state </summary>
    /// <param name="behaviours"> Targeted behaviours </param>
    private static void UpdateBehavioursBasedOnState(IEnumerable <ValidatableMonoBehaviourStatus> behaviours)
    {
        foreach (ValidatableMonoBehaviourStatus behaviour in behaviours)
            UpdateBehaviourBasedOnState(behaviour, true);

        UpdateLeftPaneGUI();
    }

    /// <summary> Update the visibility of the fixAll button </summary>
    private static void UpdateFixAllButton()
    {
        if (s_displayedListIndex is -1 or 2)
        {
            s_btnFixAll.style.display = DisplayStyle.None;

            return;
        }

        var hasFixAction = GetFixableBehaviours().Length > 0;

        s_btnFixAll.style.display = hasFixAction ? DisplayStyle.Flex : DisplayStyle.None;
    }

    /// <summary> Update the visibility of the main list </summary>
    private static void UpdateGameObjectsListViewVisibility()
    {
        s_gameObjectsView.style.display = s_displayedItems.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;
    }

    /// <summary> Update the left pane </summary>
    private static void UpdateLeftPane()
    {
        Debug.Log("Update left Pane"); // TODO remove
        
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

        var hasBehaviours = CollectValidatableMonoBehaviours(
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

    /// <summary> Update the gui of the left pane </summary>
    private static void UpdateLeftPaneGUI()
    {
        Debug.Log("Update Left Pane GUI"); // TODO remove
        
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

    /// <summary> Update the visibility of the noSelection element </summary>
    private static void UpdateNoSelectionVisibility()
    {
        s_noSelection.style.display = s_displayedListIndex >= 0 ? DisplayStyle.None : DisplayStyle.Flex;
    }

    #endregion
}

}
#endif
