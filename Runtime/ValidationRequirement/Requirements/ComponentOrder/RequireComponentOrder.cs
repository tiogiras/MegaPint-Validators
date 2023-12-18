using System;
using System.Collections.Generic;
using System.Linq;
using SerializeReferenceDropdown.Runtime;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditorInternal;
#endif

namespace ValidationRequirement.Requirements.ComponentOrder
{

[Serializable]
[SerializeReferenceDropdownName("Component Order", 100)]
public class RequireComponentOrder : ScriptableValidationRequirement
{
    private struct Category : IComparable <Category>
    {
        public ComponentOrderConfig.Type type;
        public List <Component> components;
        public int priority;

        public int CompareTo(Category other)
        {
            return -priority.CompareTo(other.priority);
        }
    }

    [SerializeField] private ComponentOrderConfig _config;

    private List <Category> _allCategories;

    #region Protected Methods

    protected override void OnInitialization()
    {
    }

    protected override void Validate(UnityEngine.GameObject gameObject)
    {
        if (_config == null)
            return;

        List <Component> components = GetAllComponents(gameObject);

        ConvertCategories(
            out List <Category> allCategories,
            out List <Category> categories,
            out List <Category> specificComponents,
            out Category fill);

        CollectComponentsForSpecifics(specificComponents, ref components);

        categories.Sort();

        CollectComponentsForCategories(categories, ref components);

        if (components.Count > 0)
            fill.components.AddRange(components);

        components = GetAllComponents(gameObject);

        if (CheckComponentOrder(components, allCategories))
            return;

        allCategories.Reverse();
        _allCategories = allCategories;

        AddError(
            "Incorrect Component Order",
            "",
            ValidationState.Warning,
            FixAction);
    }

    #endregion

    #region Private Methods

    private bool CheckComponentOrder(List <Component> components, List <Category> allCategories)
    {
        Category currentCategory = allCategories[0];
        var currentCategoryIndex = 0;
        var currentCategoryElementIndex = 0;

        foreach (Component component in components)
        {
            if (currentCategoryElementIndex >= currentCategory.components.Count)
            {
                currentCategory = GetNextCategory(allCategories, ref currentCategoryIndex);
                currentCategoryElementIndex = 0;
            }

            if (!currentCategory.components.Contains(component))
                return false;

            currentCategoryElementIndex++;
        }

        return true;
    }

    private void CollectComponentsForCategories(IEnumerable <Category> categories, ref List <Component> components)
    {
        foreach (Category category in categories.Where(
                     category => category.type.category.function
                                     is not (ComponentOrderConfig.CategoryFunction.NamespaceContains
                                     or ComponentOrderConfig.CategoryFunction.NamespaceEquals) ||
                                 !string.IsNullOrEmpty(category.type.category.nameSpaceString)))
        {
            for (var i = components.Count - 1; i >= 0; i--)
            {
                var nameSpace = components[i].GetType().Namespace;

                if (!IsInCategory(nameSpace, category))
                    continue;

                category.components.Add(components[i]);
                components.RemoveAt(i);
            }
        }
    }

    private void CollectComponentsForSpecifics(List <Category> specificComponents, ref List <Component> components)
    {
        foreach (Category category in specificComponents)
        {
            for (var i = components.Count - 1; i >= 0; i--)
            {
                if (!components[i].GetType().Name.Equals(category.type.componentName))
                    continue;

                category.components.Add(components[i]);
                components.RemoveAt(i);
            }
        }
    }

    private void ConvertCategories(
        out List <Category> allCategories,
        out List <Category> categories,
        out List <Category> specificComponents,
        out Category fill)
    {
        allCategories = new List <Category>();
        categories = new List <Category>();
        specificComponents = new List <Category>();
        fill = new Category();

        foreach (ComponentOrderConfig.Type type in _config.types)
        {
            var category = new Category
            {
                type = type,
                components = new List <Component>(),
                priority = type.category.function switch
                           {
                               ComponentOrderConfig.CategoryFunction.AddCategory => 0,
                               ComponentOrderConfig.CategoryFunction.NonUnityComponents => 0,
                               ComponentOrderConfig.CategoryFunction.NamespaceContains => 1,
                               ComponentOrderConfig.CategoryFunction.NamespaceEquals => 2,
                               ComponentOrderConfig.CategoryFunction.Fill => 0,
                               var _ => throw new ArgumentOutOfRangeException()
                           }
            };

            if (type.isCategory)
            {
                if (type.category.function == ComponentOrderConfig.CategoryFunction.Fill)
                    fill = category;
                else
                    categories.Add(category);
            }
            else
                specificComponents.Add(category);

            allCategories.Add(category);
        }
    }

    private void FixAction(UnityEngine.GameObject gameObject)
    {
        foreach (Component component in _allCategories.SelectMany(category => category.components))
            MoveToTop(component);
    }

    private List <Component> GetAllComponents(UnityEngine.GameObject gameObject)
    {
        List <Component> components = gameObject.GetComponents <Component>().ToList();

        if (components[0] is UnityEngine.Transform or RectTransform)
            components.RemoveAt(0);

        return components;
    }

    private Category GetNextCategory(IReadOnlyList <Category> allCategories, ref int currentCategoryIndex)
    {
        Category newCategory;

        while (true)
        {
            currentCategoryIndex++;
            newCategory = allCategories[currentCategoryIndex];

            if (newCategory.components.Count > 0)
                break;
        }

        return newCategory;
    }

    private bool IsInCategory(string nameSpace, Category category)
    {
        return category.type.category.function switch
               {
                   ComponentOrderConfig.CategoryFunction.AddCategory => false,
                   ComponentOrderConfig.CategoryFunction.Fill => false,
                   ComponentOrderConfig.CategoryFunction.NonUnityComponents => IsNonUnityComponent(nameSpace),
                   ComponentOrderConfig.CategoryFunction.NamespaceContains => NamespaceContains(nameSpace, category),
                   ComponentOrderConfig.CategoryFunction.NamespaceEquals => NamespaceEquals(nameSpace, category),
                   var _ => false
               };
    }

    private bool IsNonUnityComponent(string nameSpace)
    {
        if (string.IsNullOrEmpty(nameSpace))
            return false;

        return !nameSpace.StartsWith("UnityEngine");
    }

    private void MoveToTop(Component component)
    {
#if UNITY_EDITOR
        while (true)
        {
            if (!ComponentUtility.MoveComponentUp(component))
                return;
        }
#endif
    }

    private bool NamespaceContains(string nameSpace, Category category)
    {
        return !string.IsNullOrEmpty(nameSpace) && nameSpace.Contains(category.type.category.nameSpaceString);
    }

    private bool NamespaceEquals(string nameSpace, Category category)
    {
        return !string.IsNullOrEmpty(nameSpace) && nameSpace.Equals(category.type.category.nameSpaceString);
    }

    #endregion
}

}
