#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Editor.Scripts.Settings;
using Editor.Scripts.Windows;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts
{

internal class MegaPintValidators : MegaPintEditorWindowBase
{
    private enum SearchMode
    {
        None, Scene, Project
    }

    private const string BehaviourEntry = "User Interface/ValidatableMonoBehaviour";
    private const string ErrorEntry = "User Interface/ValidationError";

    private const string MainListEntryPath = "User Interface/ValidatorView/ValidatorViewElement";
    private readonly List <ValidatableMonoBehaviourStatus> _displayedItems = new();

    private readonly List <ValidatableMonoBehaviourStatus> _validatableMonoBehaviours = new();

    /// <summary> Loaded uxml references </summary>
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

    private MegaPintSettingsBase _validatorsSettings;

    #region Public Methods

    public override MegaPintEditorWindowBase ShowWindow()
    {
        titleContent.text = "Validator View";

        return this;
    }

    #endregion

    #region Protected Methods

    protected override string BasePath()
    {
        return "User Interface/ValidatorView/ValidatorView";
    }

    protected override void CreateGUI()
    {
        base.CreateGUI();

        VisualElement root = rootVisualElement;

        VisualElement content = _baseWindow.Instantiate();

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

        _mainList.makeItem = () => _mainListEntry.Instantiate();

        _mainList.bindItem = (element, i) =>
        {
            ValidatableMonoBehaviourStatus item = _displayedItems[i];

            element.Q <Label>("ObjectName").text = item.gameObject.name;

            element.Q <Label>("Ok").style.display = item.State == ValidationState.Ok ? DisplayStyle.Flex : DisplayStyle.None;
            element.Q <Label>("Warning").style.display = item.State == ValidationState.Warning ? DisplayStyle.Flex : DisplayStyle.None;
            element.Q <Label>("Error").style.display = item.State == ValidationState.Error ? DisplayStyle.Flex : DisplayStyle.None;

            element.tooltip = GetParentPath(item.transform);
        };

        _mainList.style.display = DisplayStyle.None;
        _searchField.style.display = DisplayStyle.None;

        #endregion

        #region ErrorView

        _behaviourEntry = Resources.Load <VisualTreeAsset>(BehaviourEntry);
        _errorEntry = Resources.Load <VisualTreeAsset>(ErrorEntry);

        _errorView.makeItem = () => _behaviourEntry.Instantiate();

        _errorView.bindItem = (element, i) =>
        {
            ValidatableMonoBehaviourStatus status = _displayedItems[_mainList.selectedIndex];
            InvalidBehaviour invalidBehaviour = status.invalidBehaviours[i];

            element.Q <Foldout>().text = invalidBehaviour.behaviourName;
            var errors = element.Q <ListView>("Errors");

            errors.makeItem = () => _errorEntry.Instantiate();

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

        _validatorsSettings = MegaPintSettings.Instance.GetSetting(MegaPintValidatorsSaveData.SettingsName);

        _showChildren.value = _validatorsSettings.GetValue(
            MegaPintValidatorsSaveData.showChildren.key,
            MegaPintValidatorsSaveData.showChildren.defaultValue);

        _projectSearchMode.index = _validatorsSettings.GetValue(
            MegaPintValidatorsSaveData.searchMode.key,
            MegaPintValidatorsSaveData.searchMode.defaultValue);

        _showChildrenProject.value = _validatorsSettings.GetValue(
            MegaPintValidatorsSaveData.showChildrenProject.key,
            MegaPintValidatorsSaveData.showChildrenProject.defaultValue);

        UpdateCurrentSearchMode(_currentSearchMode);

        root.Add(content);
    }

    protected override bool LoadResources()
    {
        _baseWindow = Resources.Load <VisualTreeAsset>(BasePath());
        _mainListEntry = Resources.Load <VisualTreeAsset>(MainListEntryPath);

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

    private void ChangeFolderPath()
    {
        var newFolderPath = EditorUtility.OpenFolderPanel("FolderPath", "Assets", "");

        if (!newFolderPath.StartsWith(Application.dataPath))
        {
            Debug.LogWarning($"Selected path is not in the project!\n{newFolderPath}");

            return;
        }

        _validatorsSettings.SetValue(MegaPintValidatorsSaveData.searchFolder.key, newFolderPath.Replace(Application.dataPath, "Assets"));

        PerformSearch(_currentSearchMode);
    }

    private string[] CollectGUIDsInFolder()
    {
        var searchFolder = _validatorsSettings.GetValue(
            MegaPintValidatorsSaveData.searchFolder.key,
            MegaPintValidatorsSaveData.searchFolder.defaultValue);

        if (string.IsNullOrEmpty(searchFolder))
            return null;

        if (_projectSearchMode.index == 1)
        {
            var path = Path.Combine(Application.dataPath, searchFolder[(searchFolder.Length > 6 ? 7 : 6)..]);
            var files = Directory.GetFiles(path, "*.prefab");

            return files.Select(file => AssetDatabase.AssetPathToGUID(file.Replace(Application.dataPath, "Assets"))).ToArray();
        }

        List <string> folders = new() {searchFolder};

        folders.AddRange(GetSubFolders(searchFolder));

        return AssetDatabase.FindAssets("t:prefab", folders.ToArray());
    }

    private void ConvertGUIDsToValidatableMonoBehaviours(IEnumerable <string> guids)
    {
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath <GameObject>(path);

            if (_showChildrenProject.value)
            {
                ValidatableMonoBehaviourStatus[] validators = asset.GetComponentsInChildren <ValidatableMonoBehaviourStatus>();

                if (validators is {Length: > 0})
                    _validatableMonoBehaviours.AddRange(validators);

                continue;
            }

            if (asset.TryGetComponent(out ValidatableMonoBehaviourStatus validator))
                _validatableMonoBehaviours.Add(validator);
        }
    }

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

    private void FixAll()
    {
        ValidatableMonoBehaviourStatus status = _displayedItems[_mainList.selectedIndex];

        foreach (ValidationError error in status.invalidBehaviours.SelectMany(invalidBehaviour => invalidBehaviour.errors))
        {
            if (error.fixAction == null)
                Debug.LogWarning($"No FixAction specified for [{error.errorName}], requires manual attention!");
            else
                error.fixAction.Invoke(error.gameObject);
        }

        status.ValidateStatus();
    }

    private List <string> GetSubFolders(string folderPath)
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

    private void OnItemChosen(IEnumerable <object> obj)
    {
        ValidatableMonoBehaviourStatus status = _displayedItems[_mainList.selectedIndex];

        Selection.SetActiveObjectWithContext(status, null);
    }

    private void OnProjectSearchModeChanged(ChangeEvent <string> _)
    {
        _validatorsSettings.SetValue(MegaPintValidatorsSaveData.searchMode.key, _projectSearchMode.index);
        PerformSearch(_currentSearchMode);
    }

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

    private void OnShowChildrenChanged(ChangeEvent <bool> evt)
    {
        _validatorsSettings.SetValue(MegaPintValidatorsSaveData.showChildren.key, evt.newValue);
        PerformSearch(_currentSearchMode);
    }

    private void OnShowChildrenProjectChanged(ChangeEvent <bool> evt)
    {
        _validatorsSettings.SetValue(MegaPintValidatorsSaveData.showChildrenProject.key, evt.newValue);
        PerformSearch(_currentSearchMode);
    }

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
                _validatableMonoBehaviours.AddRange(FindObjectsOfType <ValidatableMonoBehaviourStatus>());

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

    private void UpdateCurrentSearchMode(SearchMode searchMode)
    {
        _currentSearchMode = searchMode;

        _showChildren.style.display = searchMode == SearchMode.Scene ? DisplayStyle.Flex : DisplayStyle.None;

        _projectSearchMode.style.display = searchMode == SearchMode.Project ? DisplayStyle.Flex : DisplayStyle.None;
        _showChildrenProject.style.display = searchMode == SearchMode.Project ? DisplayStyle.Flex : DisplayStyle.None;

        var folderSettingsVisibility = searchMode == SearchMode.Project &&
                                       _validatorsSettings.GetValue(
                                           MegaPintValidatorsSaveData.searchMode.key,
                                           MegaPintValidatorsSaveData.searchMode.defaultValue) >
                                       0;

        _folderPath.style.display = folderSettingsVisibility ? DisplayStyle.Flex : DisplayStyle.None;

        _folderPath.value = _validatorsSettings.GetValue(
            MegaPintValidatorsSaveData.searchFolder.key,
            MegaPintValidatorsSaveData.searchFolder.defaultValue);

        _changeButtonParent.style.display = folderSettingsVisibility ? DisplayStyle.Flex : DisplayStyle.None;
        _btnChange.style.display = folderSettingsVisibility ? DisplayStyle.Flex : DisplayStyle.None;
    }

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
