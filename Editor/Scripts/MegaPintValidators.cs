using System.Collections.Generic;
using Editor.Scripts.Windows;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts
{

public class MegaPintValidators : MegaPintEditorWindowBase
{
    /// <summary> Loaded uxml references </summary>
    private VisualTreeAsset _baseWindow;
    private VisualTreeAsset _mainListEntry;

    private ListView _mainList;
    private Button _btnScene;
    private Button _btnProject;

    private List <ValidatableMonoBehaviourStatus> _validatableMonoBehaviours = new();

    protected override string BasePath() => "User Interface/ValidatorView/ValidatorView";

    private const string MainListEntryPath = "User Interface/ValidatorView/ValidatorViewElement";

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

        #endregion

        #region MainList

        _mainList.makeItem = () => _mainListEntry.Instantiate();

        _mainList.bindItem = (element, i) =>
        {
            element.Q <Label>("ObjectName").text = _validatableMonoBehaviours[i].gameObject.name;
            
            
            element.Q <Label>("Status").text = $"[{_validatableMonoBehaviours[i].State}]";
        };

        _mainList.unbindItem = (element, i) =>
        {
            
        };

        #endregion
        
        RegisterCallbacks();
        
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
        _btnScene.clickable = new Clickable(() => {PerformSearch(true);});
        _btnProject.clickable = new Clickable(() => {PerformSearch(false);});
    }

    protected override void UnRegisterCallbacks()
    {
        _btnScene.clickable = null;
        _btnProject.clickable = null;
    }

    private void PerformSearch(bool sceneMode)
    {
        _validatableMonoBehaviours.Clear();
        _mainList.Clear();

        if (sceneMode)
        {
            _validatableMonoBehaviours.AddRange(FindObjectsOfType <ValidatableMonoBehaviourStatus>());
        }
        else
        {
            
        }

        foreach (ValidatableMonoBehaviourStatus validatableMonoBehaviourStatus in _validatableMonoBehaviours)
        {
            validatableMonoBehaviourStatus.ValidateStatus();
        }

        _validatableMonoBehaviours.Sort();

        _mainList.itemsSource = _validatableMonoBehaviours;
        _mainList.RefreshItems();
    }
}

}
