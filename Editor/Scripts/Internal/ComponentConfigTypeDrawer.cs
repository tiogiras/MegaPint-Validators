#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ValidationRequirement.Requirements.ComponentOrder;

namespace Editor.Scripts.Internal
{

[CustomEditor(typeof(ComponentOrderConfig))]
internal class ComponentConfigTypeDrawer : UnityEditor.Editor
{
    private const string BasePath = "Validators/User Interface/ComponentOrder/";
    private const string ConfigPath = BasePath + "Config";
    private const string TypeEntryPath = BasePath + "TypeEntry";

    private const string AddCategoryNiceName = "Add Category";
    private const string NonUnityComponentsNiceName = "Non-Unity Components";
    private const string NamespaceContainsNiceName = "Namespace Contains";
    private const string NamespaceEqualsNiceName = "Namespace Equals";

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

        var categoryDropdown = root.Q <DropdownField>("CategoryDropdown");
        categoryDropdown.choices = Enum.GetNames(typeof(ComponentOrderConfig.CategoryFunction)).ToList();
        categoryDropdown.choices.Remove("Fill");

        for (var i = 0; i < categoryDropdown.choices.Count; i++)
            categoryDropdown.choices[i] = TranslateCategoryFunctions(categoryDropdown.choices[i]);

        categoryDropdown.index = 0;

        categoryDropdown.RegisterValueChangedCallback(
            evt =>
            {
                if (categoryDropdown.index == 0)
                    return;

                AddCategoryListElement(evt.newValue);

                categoryDropdown.index = 0;
            });

        _listView = root.Q <ListView>();

        _typeEntryTemplate = Resources.Load <VisualTreeAsset>(TypeEntryPath);

        _listView.makeItem = () => _typeEntryTemplate.Instantiate();

        _listView.bindItem = UpdateEntry;

        _types = ((ComponentOrderConfig)target).types;
        _listView.itemsSource = _types;
        _listView.RefreshItems();

        return root;
    }

    #endregion

    #region Private Methods

    private static string CategoryFunctionTooltip(ComponentOrderConfig.CategoryFunction categoryFunction)
    {
        return categoryFunction switch
               {
                   ComponentOrderConfig.CategoryFunction.AddCategory => "",
                   ComponentOrderConfig.CategoryFunction.NonUnityComponents =>
                       "All components that are not in the UnityEngine namespace will be placed here.",
                   ComponentOrderConfig.CategoryFunction.NamespaceContains =>
                       "All components that contain the given string in their namespace will be placed here.",
                   ComponentOrderConfig.CategoryFunction.NamespaceEquals => "All components that are in the specified namespace will be placed here",
                   var _ => throw new ArgumentOutOfRangeException(nameof(categoryFunction), categoryFunction, null)
               };
    }

    private static string TranslateCategoryFunctions(string category)
    {
        if (Enum.TryParse(category, out ComponentOrderConfig.CategoryFunction function))
        {
            return function switch
                   {
                       ComponentOrderConfig.CategoryFunction.AddCategory => AddCategoryNiceName,
                       ComponentOrderConfig.CategoryFunction.NonUnityComponents => NonUnityComponentsNiceName,
                       ComponentOrderConfig.CategoryFunction.NamespaceContains => NamespaceContainsNiceName,
                       ComponentOrderConfig.CategoryFunction.NamespaceEquals => NamespaceEqualsNiceName,
                       var _ => throw new ArgumentOutOfRangeException()
                   };
        }

        return category switch
               {
                   AddCategoryNiceName => ComponentOrderConfig.CategoryFunction.AddCategory.ToString(),
                   NonUnityComponentsNiceName => ComponentOrderConfig.CategoryFunction.NonUnityComponents.ToString(),
                   NamespaceContainsNiceName => ComponentOrderConfig.CategoryFunction.NamespaceContains.ToString(),
                   NamespaceEqualsNiceName => ComponentOrderConfig.CategoryFunction.NamespaceEquals.ToString(),
                   var _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
               };
    }

    private void AddCategoryListElement(string categoryFunction)
    {
        var function = Enum.Parse <ComponentOrderConfig.CategoryFunction>(TranslateCategoryFunctions(categoryFunction));

        _types.Add(
            new ComponentOrderConfig.Type
            {
                componentName = categoryFunction,
                tooltip = CategoryFunctionTooltip(function),
                canBeDeleted = true,
                canBeModified = true,
                isCategory = true,
                category = {function = function}
            });

        _isDirty = true;

        _listView.RefreshItems();
    }

    private void AddListElement()
    {
        _types.Add(new ComponentOrderConfig.Type());
        _isDirty = true;

        _listView.RefreshItems();
    }

    private void RemoveListElement()
    {
        var index = _listView.selectedItem == null ? _types.Count - 1 : _listView.selectedIndex;

        if (!_types[index].canBeDeleted)
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

    private void UpdateCategoryContent(VisualElement element, ComponentOrderConfig.Type entry)
    {
        switch (entry.category.function)
        {
            case ComponentOrderConfig.CategoryFunction.AddCategory:
                break;

            case ComponentOrderConfig.CategoryFunction.Fill:
                break;

            case ComponentOrderConfig.CategoryFunction.NonUnityComponents:
                break;

            case ComponentOrderConfig.CategoryFunction.NamespaceContains or
                ComponentOrderConfig.CategoryFunction.NamespaceEquals:

                var title = entry.category.function == ComponentOrderConfig.CategoryFunction.NamespaceContains
                    ? "Control String"
                    : "Namespace";

                var namespaceField = new TextField(title) {value = entry.category.nameSpaceString, style = {flexGrow = 1}};

                element.Add(namespaceField);

                namespaceField.RegisterValueChangedCallback(
                    evt =>
                    {
                        entry.category.nameSpaceString = evt.newValue;
                        _isDirty = true;
                    });

                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void UpdateEntry(VisualElement element, int i)
    {
        ComponentOrderConfig.Type entry = _types[i];

        element.tooltip = entry.tooltip;
        element.Q <Label>("Index").text = (i + 1).ToString();

        var componentContent = element.Q <GroupBox>("Component");
        var categoryContent = element.Q <GroupBox>("Category");

        componentContent.style.display = entry.isCategory ? DisplayStyle.None : DisplayStyle.Flex;
        categoryContent.style.display = entry.isCategory ? DisplayStyle.Flex : DisplayStyle.None;

        componentContent.style.opacity = entry.canBeModified ? 1 : .5f;
        categoryContent.style.opacity = entry.canBeModified ? 1 : .5f;

        if (entry.isCategory)
        {
            categoryContent.Q <Label>("CategoryTitle").text = entry.componentName;

            var customContent = categoryContent.Q <GroupBox>("CustomContent");
            customContent.Clear();

            UpdateCategoryContent(customContent, entry);
        }
        else
        {
            var componentName = componentContent.Q <TextField>();
            componentName.isReadOnly = !entry.canBeModified;
            componentName.value = entry.componentName;

            componentName.RegisterValueChangedCallback(
                evt =>
                {
                    if (_types[i].isCategory)
                        return;

                    _types[i].componentName = evt.newValue;
                    _isDirty = true;
                });
        }
    }

    #endregion
}

}

#endif
