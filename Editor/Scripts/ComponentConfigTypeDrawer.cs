using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using ValidationRequirement.Requirements.ComponentOrder;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Editor.Scripts
{

[CustomEditor(typeof(ComponentOrderConfig))]
public class ComponentConfigTypeDrawer : UnityEditor.Editor
{
    private const string BasePath = "User Interface/ComponentOrder/";
    private const string ConfigPath = BasePath + "Config";
    private const string TypeEntryPath = BasePath + "TypeEntry";

    private VisualTreeAsset _configTemplate;

    private bool _isDirty;

    private ListView _listView;
    private VisualTreeAsset _typeEntryTemplate;

    private List <ComponentOrderConfig.Type> _types;

    #region Public Methods

    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();

        _configTemplate = Resources.Load <VisualTreeAsset>(ConfigPath);
        TemplateContainer configBase = _configTemplate.Instantiate();
        root.Add(configBase);

        root.RegisterCallback <FocusOutEvent>(Save);

        var addButton = root.Q <Button>("BTN_Add");
        addButton.clicked += AddListElement;

        var removeButton = root.Q <Button>("BTN_Remove");
        removeButton.clicked += RemoveListElement;

        _listView = root.Q <ListView>();

        _typeEntryTemplate = Resources.Load <VisualTreeAsset>(TypeEntryPath);

        _listView.makeItem = () => _typeEntryTemplate.Instantiate();

        _listView.bindItem = (element, i) =>
        {
            element.userData = i;
            UpdateEntry(element, i);
        };

        _types = ((ComponentOrderConfig)target).types;
        _listView.itemsSource = _types;
        _listView.RefreshItems();

        return root;
    }

    #endregion

    #region Private Methods

    private void AddListElement()
    {
        _types.Add(new ComponentOrderConfig.Type());
        _isDirty = true;

        _listView.RefreshItems();
    }

    private void RemoveListElement()
    {
        var index = _listView.selectedItem == null ? _types.Count - 1 : _listView.selectedIndex;

        if (!_types[index].canBeModified)
            return;

        _types.RemoveAt(index);
        _isDirty = true;

        _listView.RefreshItems();
    }

    private void Save(FocusOutEvent evt)
    {
        if (!_isDirty)
            return;

        _isDirty = false;

        serializedObject.ApplyModifiedProperties();
#if UNITY_EDITOR
        EditorUtility.SetDirty(serializedObject.targetObject);
        AssetDatabase.SaveAssetIfDirty(serializedObject.targetObject);
#endif
    }

    private void UpdateEntry(VisualElement element, int i)
    {
        ComponentOrderConfig.Type entry = _types[i];

        element.tooltip = entry.tooltip;
        element.Q <Label>("Index").text = (i + 1).ToString();

        var componentName = element.Q <TextField>("ComponentName");
        componentName.style.opacity = entry.canBeModified ? 1 : .5f;
        componentName.isReadOnly = !entry.canBeModified;
        componentName.value = entry.componentName;

        componentName.RegisterValueChangedCallback(
            evt =>
            {
                _types[(int)element.userData].componentName = evt.newValue;
                _isDirty = true;
            });
    }

    #endregion
}

}
