using System;
using System.Collections.Generic;
using System.Linq;
using SerializeReferenceDropdown.Runtime;
using UnityEditorInternal;
using UnityEngine;

namespace ValidationRequirement.Requirements.ComponentOrder
{

[Serializable]
[SerializeReferenceDropdownName("Component Order")]
public class RequireComponentOrder : ScriptableValidationRequirement
{
    [SerializeField] private ComponentOrderConfig _config;

    #region Protected Methods

    protected override void OnInitialization()
    {
    }

    protected override void Validate(GameObject gameObject)
    {
        if (_config == null)
            return;

        List <Component> components = gameObject.GetComponents <Component>().ToList();

        if (components[0] is Transform or RectTransform)
            components.RemoveAt(0);

        GetComponentStructure(out List <string> aboveFillTypes, out List <string> belowFillTypes);

        var valid = CheckOrderFromTop(components, aboveFillTypes);

        if (valid)
            valid = CheckOrderFromBottom(components, belowFillTypes);

        if (valid)
            return;

        AddError(
            "Incorrect Component Order",
            "",
            ValidationState.Warning,
            FixAction);
    }

    #endregion

    #region Private Methods

    private bool CheckOrderFromBottom(List <Component> components, List <string> componentTypes)
    {
        List <string> existingComponents = GetExistingComponents(components, componentTypes);
        existingComponents.Reverse();

        for (var i = 0; i < existingComponents.Count; i++)
        {
            if (!components[^(i + 1)].GetType().Name.Equals(existingComponents[i]))
                return false;
        }

        return true;
    }

    private bool CheckOrderFromTop(IReadOnlyList <Component> components, List <string> componentTypes)
    {
        List <string> existingComponents = GetExistingComponents(components, componentTypes);

        for (var i = 0; i < existingComponents.Count; i++)
        {
            if (!components[i].GetType().Name.Equals(existingComponents[i]))
                return false;
        }

        return true;
    }

    private void FixAction(GameObject gameObject)
    {
        if (_config == null)
            return;

        List <Component> components = gameObject.GetComponents <Component>().ToList();
        Dictionary <string, List <Component>> componentDictionary = new();

        foreach (Component component in components)
        {
            var componentName = component.GetType().Name;
            componentDictionary.TryAdd(componentName, new List <Component>());

            componentDictionary[componentName].Add(component);
        }

        if (components[0] is Transform or RectTransform)
            components.RemoveAt(0);

        GetComponentStructure(out List <string> aboveFillTypes, out List <string> belowFillTypes);

        List <string> aboveComponents = GetExistingComponents(components, aboveFillTypes);
        aboveComponents.Reverse();

        MoveAllToTop(componentDictionary, aboveComponents);

        List <string> belowComponents = GetExistingComponents(components, belowFillTypes);

        MoveAllToBottom(componentDictionary, belowComponents);
    }

    private void GetComponentStructure(out List <string> aboveFillTypes, out List <string> belowFillTypes)
    {
        aboveFillTypes = new List <string>();
        belowFillTypes = new List <string>();

        var reachedFill = false;

        foreach (ComponentOrderConfig.Type type in _config.types)
        {
            if (type.function == ComponentOrderConfig.SpecialFunction.Fill)
            {
                reachedFill = true;

                continue;
            }

            if (!reachedFill)
                aboveFillTypes.Add(type.componentName);
            else
                belowFillTypes.Add(type.componentName);
        }
    }

    private List <string> GetExistingComponents(IEnumerable <Component> components, List <string> componentTypes)
    {
        Dictionary <string, int> types = new();

        foreach (var componentType in componentTypes)
        {
            types.TryAdd(componentType, 0);
            types[componentType]++;
        }

        List <string> componentTypeNames = components.Select(component => component.GetType().Name).ToList();
        componentTypeNames = componentTypeNames.Distinct().ToList();

        foreach (var componentType in componentTypeNames)
        {
            types.TryAdd(componentType, 0);
            types[componentType]++;
        }

        List <string> existingComponents = new();

        foreach (var componentType in componentTypes)
        {
            try
            {
                if (types[componentType] > 1)
                    existingComponents.Add(componentType);
            }
            catch (KeyNotFoundException)
            {
            }
        }

        return existingComponents;
    }

    private void MoveAllToBottom(IReadOnlyDictionary <string, List <Component>> componentDictionary, List <string> belowComponents)
    {
        foreach (var belowComponent in belowComponents)
        {
            try
            {
                foreach (Component component in componentDictionary[belowComponent])
                    MoveToBottom(component);
            }
            catch (KeyNotFoundException)
            {
            }
        }
    }

    private void MoveAllToTop(IReadOnlyDictionary <string, List <Component>> componentDictionary, List <string> aboveComponents)
    {
        foreach (var aboveComponent in aboveComponents)
        {
            try
            {
                foreach (Component component in componentDictionary[aboveComponent])
                    MoveToTop(component);
            }
            catch (KeyNotFoundException)
            {
            }
        }
    }

    private void MoveToBottom(Component component)
    {
#if UNITY_EDITOR
        while (true)
        {
            if (!ComponentUtility.MoveComponentDown(component))
                return;
        }
#endif
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

    #endregion
}

}
