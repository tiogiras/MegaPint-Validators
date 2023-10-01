using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ValidationRequirement.Requirements.ComponentOrder;

namespace Editor.Scripts
{

[CustomEditor(typeof(ComponentOrderConfig))]
public class ComponentConfigTypeDrawer : UnityEditor.Editor
{
    private const string BasePath = "User Interface/ComponentOrder/";
    private const string ConfigPath = BasePath + "Config";
    private const string TypeEntryPath = BasePath + "TypeEntry";

    private VisualTreeAsset _configTemplate;
    private VisualTreeAsset _typeEntryTemplate;
    
    private ListView _listView;

    private List<ComponentOrderConfig.Type> _types;

    private bool _isDirty;
    
    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
        
        _configTemplate = Resources.Load <VisualTreeAsset>(ConfigPath);
        TemplateContainer configBase = _configTemplate.Instantiate();
        root.Add(configBase);

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
            });
    }
}

}
