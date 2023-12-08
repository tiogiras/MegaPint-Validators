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

public class MegaPintValidators : MegaPintEditorWindowBase
{
    [MenuItem("MegaPint/Debug/Close Validators")]
    private static void ForceClose()
    {
        GetWindow<MegaPintValidators>().Close();
    }
    
    /// <summary> Loaded uxml references </summary>
    private VisualTreeAsset _baseWindow;
    private VisualTreeAsset _mainListEntry;

    private ListView _mainList;
    private Button _btnScene;
    private Button _btnProject;

    private Toggle _showChildren;
    private Toggle _showChildrenProject;

    private DropdownField _projectSearchMode;
    private VisualElement _changeButtonParent;
    private Button _btnChange;
    private TextField _folderPath;

    private ToolbarSearchField _searchField;

    private MegaPintSettingsBase _validatorsSettings;
    
    private List <ValidatableMonoBehaviourStatus> _validatableMonoBehaviours = new();

    protected override string BasePath() => "User Interface/ValidatorView/ValidatorView";

    private const string MainListEntryPath = "User Interface/ValidatorView/ValidatorViewElement";

    private SearchMode _currentSearchMode;
    
    private enum SearchMode
    {
        None, Scene, Project
    }
    
    public override MegaPintEditorWindowBase ShowWindow()
    {
        titleContent.text = "Validator View";
        return this;
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

        #endregion

        #region MainList

        _mainList.makeItem = () => _mainListEntry.Instantiate();

        _mainList.bindItem = (element, i) =>
        {
            element.Q <Label>("ObjectName").text = _validatableMonoBehaviours[i].gameObject.name;
            element.Q <Label>("Status").text = $"[{_validatableMonoBehaviours[i].State}]";
        };

        _mainList.style.display = DisplayStyle.None;
        _searchField.style.display = DisplayStyle.None;

        #endregion
        
        RegisterCallbacks();

        _validatorsSettings = MegaPintSettings.Instance.GetSetting(MegaPintValidatorsSaveData.SettingsName);

        _showChildren.value = _validatorsSettings.GetValue(MegaPintValidatorsSaveData.showChildren.key, MegaPintValidatorsSaveData.showChildren.defaultValue);
        
        _projectSearchMode.index = _validatorsSettings.GetValue(MegaPintValidatorsSaveData.searchMode.key, MegaPintValidatorsSaveData.searchMode.defaultValue);

        _showChildrenProject.value = _validatorsSettings.GetValue(
            MegaPintValidatorsSaveData.showChildrenProject.key,
            MegaPintValidatorsSaveData.showChildrenProject.defaultValue);

        UpdateCurrentSearchMode(_currentSearchMode);
        
        root.Add(content);
    }

    protected override bool LoadResources()
    {
        _baseWindow = Resources.Load<VisualTreeAsset>(BasePath());
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

        _btnChange.clickable = new Clickable(() => {ChangeFolderPath();});
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

    private void OnShowChildrenProjectChanged(ChangeEvent <bool> evt)
    {
        _validatorsSettings.SetValue(MegaPintValidatorsSaveData.showChildrenProject.key, evt.newValue);
        PerformSearch(_currentSearchMode);
    }

    private void OnShowChildrenChanged(ChangeEvent <bool> evt)
    {
        _validatorsSettings.SetValue(MegaPintValidatorsSaveData.showChildren.key, evt.newValue);
        PerformSearch(_currentSearchMode);
    }
    
    private void OnProjectSearchModeChanged(ChangeEvent <string> _)
    {
        _validatorsSettings.SetValue(MegaPintValidatorsSaveData.searchMode.key, _projectSearchMode.index);
        PerformSearch(_currentSearchMode);
    }

    protected override void UnRegisterCallbacks()
    {
        _btnScene.clickable = null;
        _btnProject.clickable = null;

        _showChildren.UnregisterValueChangedCallback(OnShowChildrenChanged);
        
        _projectSearchMode.UnregisterValueChangedCallback(OnProjectSearchModeChanged);
        _showChildrenProject.UnregisterValueChangedCallback(OnShowChildrenProjectChanged);
    }

    private void PerformSearch(SearchMode mode)
    {
        UpdateCurrentSearchMode(mode);
        
        _validatableMonoBehaviours.Clear();
        _mainList.Clear();
        
        _mainList.style.display = DisplayStyle.None;
        _searchField.style.display = DisplayStyle.None;

        switch (mode)
        {
            case SearchMode.None: return;

            case SearchMode.Scene:
                _validatableMonoBehaviours.AddRange(FindObjectsOfType <ValidatableMonoBehaviourStatus>());
                break;

            case SearchMode.Project:

                string[] prefabs;

                switch (_projectSearchMode.index)
                {
                    case 0:
                        prefabs = AssetDatabase.FindAssets("t:prefab");
                        break;
                    
                    case 1 or 2:

                        var searchFolder = _validatorsSettings.GetValue(
                            MegaPintValidatorsSaveData.searchFolder.key,
                            MegaPintValidatorsSaveData.searchFolder.defaultValue);
                        
                        if (string.IsNullOrEmpty(searchFolder))
                            return;

                        if (_projectSearchMode.index == 1)
                        {
                            var path = Path.Combine(Application.dataPath, searchFolder[(searchFolder.Length > 6 ? 7 : 6)..]);
                            var files = Directory.GetFiles(path, "*.prefab");

                            prefabs = files.Select(file => AssetDatabase.AssetPathToGUID(file.Replace(Application.dataPath, "Assets"))).ToArray();
                        }
                        else
                        {
                            List <string> folders = new() {searchFolder};

                            folders.AddRange(GetSubFolders(searchFolder));

                            prefabs = AssetDatabase.FindAssets("t:prefab", folders.ToArray());
                        }
                        
                        break;
                    
                    default:
                        return;
                }

                foreach (var guid in prefabs)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                    if (_showChildrenProject.value)
                    {
                        ValidatableMonoBehaviourStatus[] validators = asset.GetComponentsInChildren <ValidatableMonoBehaviourStatus>();
                    
                        if (validators is {Length: > 0})
                            _validatableMonoBehaviours.AddRange(validators);
                    }
                    else
                    {
                        if (asset.TryGetComponent(out ValidatableMonoBehaviourStatus validator))
                            _validatableMonoBehaviours.Add(validator);
                    }
                }
                break;

            default: return;
        }

        if (_validatableMonoBehaviours.Count == 0)
            return;
        
        for (var i = _validatableMonoBehaviours.Count - 1; i >= 0; i--)
        {
            ValidatableMonoBehaviourStatus validatableMonoBehaviourStatus = _validatableMonoBehaviours[i];

            if (!_showChildren.value)
            {
                List <ValidatableMonoBehaviourStatus> parentStates = validatableMonoBehaviourStatus.GetComponentsInParent <ValidatableMonoBehaviourStatus>().ToList();

                parentStates.Remove(validatableMonoBehaviourStatus);

                if (parentStates.Any(state => state.ValidatesChildren()))
                {
                    _validatableMonoBehaviours.RemoveAt(i);
                    continue;   
                }
            }

            validatableMonoBehaviourStatus.ValidateStatus();
        }

        _validatableMonoBehaviours.Sort();
        
        _mainList.itemsSource = _validatableMonoBehaviours;
        _mainList.RefreshItems();
        
        _mainList.style.display = DisplayStyle.Flex;
        _searchField.style.display = DisplayStyle.Flex;
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

    private void UpdateCurrentSearchMode(SearchMode searchMode)
    {
        _currentSearchMode = searchMode;

        _showChildren.style.display = searchMode == SearchMode.Scene ? DisplayStyle.Flex : DisplayStyle.None;

        _projectSearchMode.style.display = searchMode == SearchMode.Project ? DisplayStyle.Flex : DisplayStyle.None;
        _showChildrenProject.style.display = searchMode == SearchMode.Project ? DisplayStyle.Flex : DisplayStyle.None;

        var folderSettingsVisibility = searchMode == SearchMode.Project &&
                                       _validatorsSettings.GetValue(MegaPintValidatorsSaveData.searchMode.key, MegaPintValidatorsSaveData.searchMode.defaultValue) > 0;
        
        _folderPath.style.display = folderSettingsVisibility ? DisplayStyle.Flex : DisplayStyle.None;

        _folderPath.value = _validatorsSettings.GetValue(
            MegaPintValidatorsSaveData.searchFolder.key,
            MegaPintValidatorsSaveData.searchFolder.defaultValue);
        
        _changeButtonParent.style.display = folderSettingsVisibility ? DisplayStyle.Flex : DisplayStyle.None;
        _btnChange.style.display = folderSettingsVisibility ? DisplayStyle.Flex : DisplayStyle.None;
    }
    
    // TODO
    // Searchbar working
    // GameObject Name in project view as tooltip on the List Entries
}

}
