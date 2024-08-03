using System;
using System.Collections.Generic;
using MegaPint.SerializeReferenceDropdown.Runtime;
using MegaPint.ValidationRequirement.Requirements.GameObjectValidation.RequireComponentDropdown;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

namespace MegaPint.ValidationRequirement.Requirements.GameObjectValidation
{

/// <summary> Validation requirement requiring a custom name based on a defined regex ruleset </summary>
[Serializable]
[Tooltip("With this requirement you can specify a component that is required on this gameObject.")]
[ValidationRequirement("GameObject/Component", typeof(RequireComponent), true, -30, 20)]
internal class RequireComponent : ScriptableValidationRequirement
{
    public Properties properties;

    private List <Type> _types;

    #region Protected Methods

    protected override void OnInitialization()
    {
    }

    protected override void OnRequirementValidation()
    {
        if (_types == null)
            GetTypes();
    }

    protected override void Validate(GameObject gameObject)
    {
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

    /// <summary> Add the missing component to the fameObject </summary>
    /// <param name="gameObject"> Target gameObject </param>
    private void FixAction(GameObject gameObject)
    {
        gameObject.AddComponent(GetTypeByName());
    }

    /// <summary> Get the type by the name specified in the properties </summary>
    /// <returns> Found type </returns>
    private Type GetTypeByName()
    {
        var typeName = properties.typeFullName;

#if UNITY_EDITOR
        return TypeCache.GetTypesDerivedFrom <Component>().
                         FirstOrDefault(t => t.FullName != null && t.FullName.Equals(typeName));
#else
        return null;
#endif
    }

    private void GetTypes()
    {
        _types = new List <Type>();
#if UNITY_EDITOR
        _types.AddRange(TypeCache.GetTypesDerivedFrom <Component>());
#endif
        properties.types.Clear();

        foreach (Type type in _types)
            properties.types.Add(type.FullName);
    }

    #endregion
}

}
