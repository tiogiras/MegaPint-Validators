using System;
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
[SerializeReferenceDropdownName("GameObject/Component", typeof(RequireComponent), true, -30, 20)]
public class RequireComponent : ScriptableValidationRequirement
{
    public Properties properties;

    private List <Type> _types;

    #region Protected Methods

    protected override void OnInitialization()
    {
    }

    protected override void Validate(GameObject gameObject)
    {
        if (_types == null)
            GetTypes();

        if (string.IsNullOrEmpty(properties.typeName))
            return;

        Type type = GetTypeByName();

        AddErrorIf(
            !gameObject.GetComponent(type),
            "Missing Component",
            $"The gameObject is missing a component of type {type}",
            ValidationState.Warning,
            FixAction);
    }

    #endregion

    #region Private Methods

    private void FixAction(GameObject gameObject)
    {
        gameObject.AddComponent(GetTypeByName());
    }

    private Type GetTypeByName()
    {
        var typeName = properties.typeFullName;

        return TypeCache.GetTypesDerivedFrom <Component>().
                         FirstOrDefault(t => t.FullName != null && t.FullName.Equals(typeName));
    }

    private void GetTypes()
    {
        _types = new List <Type>();

        _types.AddRange(TypeCache.GetTypesDerivedFrom <Component>());

        properties.types.Clear();

        foreach (Type type in _types)
            properties.types.Add(type.FullName);
    }

    #endregion
}

}
