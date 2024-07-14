﻿using System;
using System.Collections.Generic;
using System.Linq;
using MegaPint.SerializeReferenceDropdown.Runtime;
using MegaPint.ValidationRequirement.Requirements.GameObjectValidation.RequireComponentDropdown;
using UnityEditor;
using UnityEngine;

namespace MegaPint.ValidationRequirement.Requirements.GameObjectValidation
{

/// <summary> Validation requirement requiring a custom name based on a defined regex ruleset </summary>
[Serializable]
[Tooltip(
    "With this requirement you can specify a list of components of which at least one must be on the gameObject.\nWhen fixed automatically the first selected component will be added to the gameObject.")]
[SerializeReferenceDropdownName("GameObject/Component (Any)", typeof(RequireAnyComponent), true, -30, 21)]
public class RequireAnyComponent : ScriptableValidationRequirement
{
    public List <Properties> properties;

    private List <Type> _types;

    #region Protected Methods

    protected override void OnInitialization()
    {
    }

    protected override void OnRequirementValidation()
    {
        if (properties.Count == 0)
            return;

        GetTypes();
    }

    protected override void Validate(GameObject gameObject)
    {
        if (properties.Count == 0)
            return;

        var isValid = false;
        var hasNonNullType = false;

        foreach (Properties property in properties)
        {
            if (string.IsNullOrEmpty(property.typeName))
                continue;

            hasNonNullType = true;

            Type type = GetTypeByName(property);

            if (!gameObject.GetComponent(type))
                continue;

            isValid = true;

            break;
        }

        if (!hasNonNullType)
            return;

        var componentNames = string.Join(
            "\n- ",
            properties.Where(p => !string.IsNullOrEmpty(p.typeName)).Select(p => p.typeName));

        AddErrorIf(
            !isValid,
            "Missing Component",
            $"The gameObject is missing a component. Add at least one component of the following types!\n- {componentNames}",
            ValidationState.Warning,
            FixAction);
    }

    #endregion

    #region Private Methods

    private void FixAction(GameObject gameObject)
    {
        if (string.IsNullOrEmpty(properties[0].typeName))
        {
            Debug.LogWarning("Cannot add the component to the gameObject. You have no component selected in the first list element!");

            return;
        }
        
        gameObject.AddComponent(GetTypeByName(properties[0]));
    }

    private Type GetTypeByName(Properties property)
    {
        var typeName = property.typeFullName;

        return TypeCache.GetTypesDerivedFrom <Component>().
                         FirstOrDefault(t => t.FullName != null && t.FullName.Equals(typeName));
    }

    private void GetTypes()
    {
        if (_types == null)
        {
            _types = new List <Type>();
            _types.AddRange(TypeCache.GetTypesDerivedFrom <Component>());
        }

        foreach (Properties property in properties)
        {
            property.types.Clear();

            foreach (Type type in _types)
                property.types.Add(type.FullName);
        }
    }

    #endregion
}

}
