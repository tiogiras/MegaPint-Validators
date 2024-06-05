#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
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
internal class Validators : EditorWindowBase
{
    private enum SearchMode
    {
        None, Scene, Project
    }

    private static readonly string s_baseResourcePath = Constants.Validators.UserInterface.ValidatorView;

    private readonly List <ValidatableMonoBehaviourStatus> _displayedItems = new();
    private readonly List <ValidatableMonoBehaviourStatus> _validatableMonoBehaviours = new();

    private VisualTreeAsset _baseWindow;

    private VisualTreeAsset _behaviourEntry;
    private Button _btnChange;
    private Button _btnFixAll;
    private Button _btnProject;
    private Button _btnScene;
    private VisualElement _changeButtonParent;

    private SearchMode _currentSearchMode;
    private VisualTreeAsset _errorEntry;
    private VisualElement _errorPanel;
    private ListView _errorView;
    private TextField _folderPath;
    private Label _gameObjectName;

    private ListView _mainList;
    private VisualTreeAsset _mainListEntry;
    private Label _noIssue;
    private Label _path;

    private DropdownField _projectSearchMode;

    private VisualElement _rightPane;

    private ToolbarSearchField _searchField;

    private Toggle _showChildren;
    private Toggle _showChildrenProject;

    #region Public Methods

    public override EditorWindowBase ShowWindow()
    {
        titleContent.text = "Validator View";

        return this;
    }

    #endregion

    #region Protected Methods

    protected override string BasePath()
    {
        return s_baseResourcePath;
    }

    protected override void CreateGUI()
    {
        base.CreateGUI();

        VisualElement root = rootVisualElement;

        VisualElement content = GUIUtility.Instantiate(_baseWindow);

        #region References

        _mainList = content.Q <ListView>("MainList");
        _btnScene = content.Q <Button>("BTN_Scene");
        _btnProject = content.Q <Button>("BTN_Project");
        _searchField = content.Q <ToolbarSearchField>("SearchField");

        _showChildren = content.Q <Toggle>("ShowChildren");
        _showChildrenProject = content.Q <Toggle>("ShowChildren_Project");

        _projectSearchMode = content.Q <DropdownField>("ProjectSearchMode");
        _changeButtonParent = content.Q <VisualElement>("ChangeButtonParent");
        _btnChange = _changeButtonParent.Q <Button>("BTN_Change");
        _folderPath = content.Q <TextField>("FolderPath");

        _rightPane = content.Q <VisualElement>("RightPane");
        _gameObjectName = content.Q <Label>("GameObjectName");
        _path = content.Q <Label>("Path");
        _errorPanel = content.Q <VisualElement>("ErrorPanel");
        _btnFixAll = content.Q <Button>("BTN_FixAll");
        _noIssue = content.Q <Label>("NoIssue");

        _errorView = content.Q <ListView>("ErrorView");

        #endregion

        #region MainList

        _mainList.makeItem = () => GUIUtility.Instantiate(_mainListEntry);

        _mainList.bindItem = (element, i) =>
        {
            ValidatableMonoBehaviourStatus item = _displayedItems[i];

            element.Q <Label>("ObjectName").text = item.gameObject.name;

            element.Q <Label>("Ok").style.display =
                item.State == ValidationState.Ok ? DisplayStyle.Flex : DisplayStyle.None;

            element.Q <Label>("Warning").style.display =
                item.State == ValidationState.Warning ? DisplayStyle.Flex : DisplayStyle.None;

            element.Q <Label>("Error").style.display =
                item.State == ValidationState.Error ? DisplayStyle.Flex : DisplayStyle.None;

            element.tooltip = GetParentPath(item.transform);
        };

        _mainList.style.display = DisplayStyle.None;
        _searchField.style.display = DisplayStyle.None;

        #endregion

        #region ErrorView

        _behaviourEntry = Resources.Load <VisualTreeAsset>(Constants.Validators.UserInterface.StatusBehaviour);
        _errorEntry = Resources.Load <VisualTreeAsset>(Constants.Validators.UserInterface.StatusError);

        _errorView.makeItem = () => GUIUtility.Instantiate(_behaviourEntry);

        _errorView.bindItem = (element, i) =>
        {
            ValidatableMonoBehaviourStatus status = _displayedItems[_mainList.selectedIndex];
            InvalidBehaviour invalidBehaviour = status.invalidBehaviours[i];

            element.Q <Foldout>().text = invalidBehaviour.behaviourName;
            var errors = element.Q <ListView>("Errors");

            errors.makeItem = () => GUIUtility.Instantiate(_errorEntry);

            errors.bindItem = (visualElement, j) =>
            {
                ValidationError error = invalidBehaviour.errors[j];

                var label = visualElement.Q <Label>("Name");
                label.text = error.errorName;
                label.tooltip = error.errorText;

                visualElement.Q <Label>("Ok").style.display = error.severity == ValidationState.Ok
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;

                visualElement.Q <Label>("Warning").style.display = error.severity == ValidationState.Warning
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;

                visualElement.Q <Label>("Error").style.display = error.severity == ValidationState.Error
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;

                var button = visualElement.Q <Button>();

                if (error.fixAction == null)
                    button.style.display = DisplayStyle.None;
                else
                {
                    button.clicked += () =>
                    {
                        error.fixAction.Invoke(error.gameObject);
                        status.ValidateStatus();
                        UpdateErrorView();
                    };
                }
            };

            errors.itemsSource = invalidBehaviour.errors;
            errors.RefreshItems();
        };

        #endregion

        _rightPane.style.display = DisplayStyle.None;

        RegisterCallbacks();

        _showChildren.value = SaveValues.Validators.ShowChildren;
        _projectSearchMode.index = SaveValues.Validators.SearchMode;
        _showChildrenProject.value = SaveValues.Validators.ShowChildrenProject;

        UpdateCurrentSearchMode(_currentSearchMode);

        root.Add(content);
    }

    protected override bool LoadResources()
    {
        _baseWindow = Resources.Load <VisualTreeAsset>(BasePath());
        _mainListEntry = Resources.Load <VisualTreeAsset>(Constants.Validators.UserInterface.ValidatorViewItem);

        return _baseWindow != null && _mainListEntry != null;
    }

    protected override void RegisterCallbacks()
    {
        _btnScene.clickable = new Clickable(() => {PerformSearch(SearchMode.Scene);});
        _btnProject.clickable = new Clickable(() => {PerformSearch(SearchMode.Project);});

        _showChildren.RegisterValueChangedCallback(OnShowChildrenChanged);

        _projectSearchMode.RegisterValueChangedCallback(OnProjectSearchModeChanged);
        _showChildrenProject.RegisterValueChangedCallback(OnShowChildrenProjectChanged);

        _searchField.RegisterValueChangedCallback(_ => {DisplayBySearchField();});

        _btnChange.clickable = new Clickable(ChangeFolderPath);

        _mainList.selectionChanged += _ => OnSelectionChange();
        _mainList.itemsChosen += OnItemChosen;

        _btnFixAll.clickable = new Clickable(FixAll);
    }

    protected override void UnRegisterCallbacks()
    {
        _btnScene.clickable = null;
        _btnProject.clickable = null;

        _showChildren.UnregisterValueChangedCallback(OnShowChildrenChanged);

        _projectSearchMode.UnregisterValueChangedCallback(OnProjectSearchModeChanged);
        _showChildrenProject.UnregisterValueChangedCallback(OnShowChildrenProjectChanged);

        _btnChange.clickable = null;

        _btnFixAll.clickable = null;
    }

    #endregion

    #region Private Methods

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

    /// <summary> Change the path of the selected folder </summary>
    private void ChangeFolderPath()
    {
        var newFolderPath = EditorUtility.OpenFolderPanel("FolderPath", "Assets", "");

        if (!newFolderPath.StartsWith(Application.dataPath))
        {
            Debug.LogWarning($"Selected path is not in the project!\n{newFolderPath}");

            return;
        }

        SaveValues.Validators.SearchFolder = newFolderPath.Replace(Application.dataPath, "Assets");

        PerformSearch(_currentSearchMode);
    }

    /// <summary> Collect all guids in the selected folder </summary>
    /// <returns> List of found guids </returns>
    private string[] CollectGUIDsInFolder()
    {
        var searchFolder = SaveValues.Validators.SearchFolder;

        if (string.IsNullOrEmpty(searchFolder))
            return null;

        if (_projectSearchMode.index == 1)
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
    private void ConvertGUIDsToValidatableMonoBehaviours(string[] guids)
    {
        if (guids is not {Length: > 0})
            return;

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath <GameObject>(path);

            if (_showChildrenProject.value)
            {
                ValidatableMonoBehaviourStatus[] validators =
                    asset.GetComponentsInChildren <ValidatableMonoBehaviourStatus>();

                if (validators is {Length: > 0})
                    _validatableMonoBehaviours.AddRange(validators);

                continue;
            }

            if (asset.TryGetComponent(out ValidatableMonoBehaviourStatus validator))
                _validatableMonoBehaviours.Add(validator);
        }
    }

    /// <summary> Filter the displayed behaviours based on the content of the search field </summary>
    private void DisplayBySearchField()
    {
        _displayedItems.Clear();

        if (string.IsNullOrEmpty(_searchField.value))
            _displayedItems.AddRange(_validatableMonoBehaviours);
        else
        {
            foreach (ValidatableMonoBehaviourStatus validatableMonoBehaviour in _validatableMonoBehaviours)
            {
                if (!validatableMonoBehaviour.gameObject.name.Contains(_searchField.value))
                    continue;

                _displayedItems.Add(validatableMonoBehaviour);
            }
        }

        _displayedItems.Sort();

        _mainList.itemsSource = _displayedItems;
        _mainList.RefreshItems();
    }

    /// <summary> Fix all issues </summary>
    private void FixAll()
    {
        ValidatableMonoBehaviourStatus status = _displayedItems[_mainList.selectedIndex];

        foreach (ValidationError error in status.invalidBehaviours.SelectMany(
                     invalidBehaviour => invalidBehaviour.errors))
        {
            if (error.fixAction == null)
                Debug.LogWarning($"No FixAction specified for [{error.errorName}], requires manual attention!");
            else
                error.fixAction.Invoke(error.gameObject);
        }

        status.ValidateStatus();
    }

    /// <summary> ListView callback </summary>
    /// <param name="obj"> Chosen item </param>
    private void OnItemChosen(IEnumerable <object> obj)
    {
        ValidatableMonoBehaviourStatus status = _displayedItems[_mainList.selectedIndex];

        Selection.SetActiveObjectWithContext(status, null);
    }

    /// <summary> Search field callback </summary>
    /// <param name="_"> Callback event </param>
    private void OnProjectSearchModeChanged(ChangeEvent <string> _)
    {
        SaveValues.Validators.SearchMode = _projectSearchMode.index;
        PerformSearch(_currentSearchMode);
    }

    /// <summary> ListView callback when changing selection </summary>
    private void OnSelectionChange()
    {
        if (_mainList.selectedItem == null)
            return;

        _rightPane.style.display = DisplayStyle.Flex;

        ValidatableMonoBehaviourStatus item = _displayedItems[_mainList.selectedIndex];

        _gameObjectName.text = item.gameObject.name;

        var path = GetParentPath(item.transform);
        _path.text = path;
        _path.tooltip = path;

        UpdateErrorView();
    }

    /// <summary> Show children toggle callback </summary>
    /// <param name="evt"> Callback event </param>
    private void OnShowChildrenChanged(ChangeEvent <bool> evt)
    {
        SaveValues.Validators.ShowChildren = evt.newValue;
        PerformSearch(_currentSearchMode);
    }

    /// <summary> Show children toggle callback </summary>
    /// <param name="evt"> Callback event </param>
    private void OnShowChildrenProjectChanged(ChangeEvent <bool> evt)
    {
        SaveValues.Validators.ShowChildrenProject = evt.newValue;
        PerformSearch(_currentSearchMode);
    }

    /// <summary> Find all <see cref="ValidatableMonoBehaviour" /> in the project based on the given settings </summary>
    private void PerformProjectSearch()
    {
        string[] prefabs;

        switch (_projectSearchMode.index)
        {
            case 0:
                prefabs = AssetDatabase.FindAssets("t:prefab");

                break;

            case 1 or 2:

                prefabs = CollectGUIDsInFolder();

                break;

            default:
                return;
        }

        ConvertGUIDsToValidatableMonoBehaviours(prefabs);
    }

    /// <summary> Find all <see cref="ValidatableMonoBehaviour" /> in the active scene based on the given settings </summary>
    private void PerformSceneSearch()
    {
        ValidatableMonoBehaviourStatus[] behaviours = Resources.FindObjectsOfTypeAll <ValidatableMonoBehaviourStatus>();
        behaviours = behaviours.Where(behaviour => behaviour.gameObject.scene.isLoaded).ToArray();

        if (SaveValues.Validators.ShowChildren)
        {
            _validatableMonoBehaviours.AddRange(behaviours);

            return;
        }

        foreach (ValidatableMonoBehaviourStatus behaviour in behaviours)
        {
            if (behaviour.transform.parent != null)
            {
                if (behaviour.transform.parent.GetComponentsInParent <ValidatableMonoBehaviourStatus>().Length > 0)
                    continue;
            }

            _validatableMonoBehaviours.Add(behaviour);
        }
    }

    /// <summary> Find the all <see cref="ValidatableMonoBehaviour" /> based on the current settings </summary>
    /// <param name="mode"> Scene or Project </param>
    private void PerformSearch(SearchMode mode)
    {
        UpdateCurrentSearchMode(mode);

        _rightPane.style.display = DisplayStyle.None;
        _mainList.ClearSelection();

        _validatableMonoBehaviours.Clear();
        _mainList.Clear();

        _mainList.style.display = DisplayStyle.None;
        _searchField.style.display = DisplayStyle.None;

        switch (mode)
        {
            case SearchMode.None:
                return;

            case SearchMode.Scene:

                PerformSceneSearch();

                break;

            case SearchMode.Project:

                PerformProjectSearch();

                break;

            default:
                return;
        }

        if (_validatableMonoBehaviours.Count == 0)
            return;

        for (var i = _validatableMonoBehaviours.Count - 1; i >= 0; i--)
        {
            ValidatableMonoBehaviourStatus validatableMonoBehaviourStatus = _validatableMonoBehaviours[i];

            if (!_showChildren.value)
            {
                List <ValidatableMonoBehaviourStatus> parentStates =
                    validatableMonoBehaviourStatus.GetComponentsInParent <ValidatableMonoBehaviourStatus>().ToList();

                parentStates.Remove(validatableMonoBehaviourStatus);

                if (parentStates.Any(state => state.ValidatesChildren()))
                {
                    _validatableMonoBehaviours.RemoveAt(i);

                    continue;
                }
            }

            validatableMonoBehaviourStatus.ValidateStatus();
        }

        DisplayBySearchField();

        _mainList.style.display = DisplayStyle.Flex;
        _searchField.style.display = DisplayStyle.Flex;
    }

    /// <summary> Update the current search mode </summary>
    /// <param name="searchMode"> New search mode </param>
    private void UpdateCurrentSearchMode(SearchMode searchMode)
    {
        _currentSearchMode = searchMode;

        _showChildren.style.display = searchMode == SearchMode.Scene ? DisplayStyle.Flex : DisplayStyle.None;

        _projectSearchMode.style.display = searchMode == SearchMode.Project ? DisplayStyle.Flex : DisplayStyle.None;
        _showChildrenProject.style.display = searchMode == SearchMode.Project ? DisplayStyle.Flex : DisplayStyle.None;

        var folderSettingsVisibility = searchMode == SearchMode.Project && SaveValues.Validators.SearchMode > 0;

        _folderPath.style.display = folderSettingsVisibility ? DisplayStyle.Flex : DisplayStyle.None;

        _folderPath.value = SaveValues.Validators.SearchFolder;

        _changeButtonParent.style.display = folderSettingsVisibility ? DisplayStyle.Flex : DisplayStyle.None;
        _btnChange.style.display = folderSettingsVisibility ? DisplayStyle.Flex : DisplayStyle.None;
    }

    /// <summary> Update the list view displaying the errors </summary>
    private void UpdateErrorView()
    {
        List <InvalidBehaviour> invalidBehaviours = _displayedItems[_mainList.selectedIndex].invalidBehaviours;
        invalidBehaviours.Sort();

        var hasErrors = invalidBehaviours.Count > 0;

        _errorPanel.style.display = !hasErrors ? DisplayStyle.None : DisplayStyle.Flex;
        _noIssue.style.display = hasErrors ? DisplayStyle.None : DisplayStyle.Flex;

        DisplayBySearchField();

        if (!hasErrors)
            return;

        _errorView.itemsSource = invalidBehaviours;
        _errorView.RefreshItems();
    }

    #endregion
}

}
#endif
