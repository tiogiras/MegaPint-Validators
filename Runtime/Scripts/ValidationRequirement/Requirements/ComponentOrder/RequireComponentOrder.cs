using System;
using System.Collections.Generic;
using System.Linq;
using MegaPint.SerializeReferenceDropdown.Runtime;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MegaPint.ValidationRequirement.Requirements.ComponentOrder
{

/// <summary> Validation requirement that enforces a specific order of the components on a gameObject </summary>
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

    protected override void Validate(GameObject gameObject)
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

    /// <summary> Check the order of the components </summary>
    /// <param name="components"> Found components </param>
    /// <param name="allCategories"> All categories of the set <see cref="ComponentOrderConfig" /> </param>
    /// <returns> If the component order is valid </returns>
    private bool CheckComponentOrder(List <Component> components, IReadOnlyList <Category> allCategories)
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

    /// <summary> Collect all components for the corresponding categories </summary>
    /// <param name="categories"> All specified categories </param>
    /// <param name="components"> All components </param>
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

    /// <summary> Collect all components for specific categories </summary>
    /// <param name="specificComponents"> Categories </param>
    /// <param name="components"> All components </param>
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

    /// <summary> Convert the given categories </summary>
    /// <param name="allCategories"> Output all found categories </param>
    /// <param name="categories"> Output categories </param>
    /// <param name="specificComponents"> Output specific categories </param>
    /// <param name="fill"> Output the fill category </param>
    /// <exception cref="ArgumentOutOfRangeException"> Function not found </exception>
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

    /// <summary> Fix the componentOrder </summary>
    /// <param name="gameObject"> Targeted gameObject </param>
    private void FixAction(GameObject gameObject)
    {
#if UNITY_EDITOR
        PrefabInstanceStatus status = PrefabUtility.GetPrefabInstanceStatus(gameObject);

        if (status == PrefabInstanceStatus.Connected)
        {
            GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
            var assetPath = AssetDatabase.GetAssetPath(prefab);

            var statusComp = prefab.GetComponent <ValidatableMonoBehaviourStatus>();
            statusComp.ValidateStatus();

            statusComp.FixAll();

            PrefabUtility.SaveAsPrefabAsset(prefab, assetPath);

            // TODO prefabUtility is editor only

            return;
        }
#endif

        foreach (Component component in _allCategories.SelectMany(category => category.components))
            MoveToTop(component);
    }

    /// <summary> Get all components of a gameObject </summary>
    /// <param name="gameObject"> Targeted gameObject </param>
    /// <returns> All components on the targeted gameObject </returns>
    private List <Component> GetAllComponents(GameObject gameObject)
    {
        List <Component> components = gameObject.GetComponents <Component>().ToList();

        if (components[0] is Transform or RectTransform)
            components.RemoveAt(0);

        return components;
    }

    /// <summary> Get the next category </summary>
    /// <param name="allCategories"> All categories </param>
    /// <param name="currentCategoryIndex"> Current category index </param>
    /// <returns> Next category </returns>
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

    /// <summary> Check if the namespace is in the category </summary>
    /// <param name="nameSpace"> Targeted namespace </param>
    /// <param name="category"> Targeted category </param>
    /// <returns> If the namespace corresponds to the category </returns>
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

    /// <summary> Check if the namespace is a non unity component </summary>
    /// <param name="nameSpace"> Targeted namespace </param>
    /// <returns> If the namespace is a non unity component </returns>
    private bool IsNonUnityComponent(string nameSpace)
    {
        if (string.IsNullOrEmpty(nameSpace))
            return false;

        return !nameSpace.StartsWith("UnityEngine");
    }

    /// <summary> Move a component to the top </summary>
    /// <param name="component"> Component to move </param>
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

    /// <summary> Check if the namespace contains the set string </summary>
    /// <param name="nameSpace"> Targeted namespace </param>
    /// <param name="category"> Targeted category </param>
    /// <returns> If the namespace contains the set string </returns>
    private bool NamespaceContains(string nameSpace, Category category)
    {
        return !string.IsNullOrEmpty(nameSpace) && nameSpace.Contains(category.type.category.nameSpaceString);
    }

    /// <summary> Check if the namespace equals the set string </summary>
    /// <param name="nameSpace"> Targeted namespace </param>
    /// <param name="category"> Targeted category </param>
    /// <returns> If the namespace equals the set string </returns>
    private bool NamespaceEquals(string nameSpace, Category category)
    {
        return !string.IsNullOrEmpty(nameSpace) && nameSpace.Equals(category.type.category.nameSpaceString);
    }

    #endregion
}

}
