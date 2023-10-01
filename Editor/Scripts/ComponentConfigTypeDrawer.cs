using System.Collections;
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
            ComponentOrderConfig.Type entry = _types[i];

            /*element.Q <TextField>("NiceName").RegisterValueChangedCallback(
                evt =>
                {
                    entry.name = evt.newValue;
                    UpdateEntry(element, i);
                });
            
            element.Q <TextField>("ComponentName").RegisterValueChangedCallback(
                evt =>
                {
                    entry.componentName = evt.newValue;
                    UpdateEntry(element, i);
                });*/
            
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
        
        element.Q <Foldout>().text = entry.name;

        var niceName = element.Q <TextField>("NiceName");
        niceName.value = entry.name;
        niceName.style.display = entry.canBeModified ? DisplayStyle.Flex : DisplayStyle.None;

        var componentName = element.Q <TextField>("ComponentName");
        componentName.value = entry.componentName;
        componentName.style.display = entry.canBeModified ? DisplayStyle.Flex : DisplayStyle.None;

        var notModification = element.Q <VisualElement>("NoModification");
        notModification.style.display = !entry.canBeModified ? DisplayStyle.Flex : DisplayStyle.None;
    }
}

}
