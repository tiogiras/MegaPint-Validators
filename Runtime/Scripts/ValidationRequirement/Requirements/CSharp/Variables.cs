using System;
using System.Collections.Generic;
using System.Reflection;
using MegaPint.SerializeReferenceDropdown.Runtime;
using MegaPint.ValidationRequirement.Requirements.CSharp.VariablesLogic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MegaPint.ValidationRequirement.Requirements.CSharp
{

/// <summary> Validation requirement for variables checking for NotNull </summary>
[Serializable]
[ValidationRequirementTooltip(
    "With this requirement you can specify a class with it's assembly qualified name (Namespace, Class, Assembly) and enforce different settings for specific variables.")]
[ValidationRequirementName("C#/Variables", typeof(Variables), true, -40)]
internal class Variables : ScriptableValidationRequirement
{
    [SerializeField] private string _classNamespace;
    [SerializeField] private string _className;
    [SerializeField] private string _assemblyName;

    public ClassValidation validation;

    [FormerlySerializedAs("_variableName")]
    [Space]
    [SerializeField] private List <Variable> _variables;

    #region Protected Methods

    protected override void OnInitialization()
    {
    }

    protected override void Validate(GameObject gameObject)
    {
        validation.foundClass = false;

        if (!TryGetClassType(out Type type))
            return;

        if (!TryGetClassComponent(gameObject, type, out Component comp))
            return;

        validation.foundClass = true;

        if (_variables.Count == 0)
            return;

        foreach (Variable variable in _variables)
        {
            var variableName = variable.properties.name;

            if (string.IsNullOrEmpty(variableName))
                continue;

            FieldInfo field = type.GetField(
                variableName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default);

            var fieldFound = field != null;

            variable.properties.fieldFound = fieldFound;

            if (!fieldFound)
                continue;

            if (!variable.Validate(field.GetValue(comp), out List <ValidationError> errors))
                AddErrors(errors);
        }
    }

    #endregion

    #region Private Methods

    /// <summary> Try to get the component of the set class type </summary>
    /// <param name="gameObject"> Target gameObject </param>
    /// <param name="type"> Target type </param>
    /// <param name="component"> Found component </param>
    /// <returns> If a component of the target type was found on the target gameObject </returns>
    private bool TryGetClassComponent(GameObject gameObject, Type type, out Component component)
    {
        component = gameObject.GetComponent(type);

        if (component == null)
        {
            Debug.LogWarning(
                "No component of the given type could be found on the gameObject. Make sure to add the component to the gameObject.");
        }

        return component != null;
    }

    /// <summary> Try to get the type of the specified class </summary>
    /// <param name="type"> Found type </param>
    /// <returns> If the type was found </returns>
    private bool TryGetClassType(out Type type)
    {
        type = null;

        if (string.IsNullOrEmpty(_className))
            return false;

        var namespaceString = $"{_classNamespace}{(string.IsNullOrEmpty(_classNamespace) ? "" : ".")}";
        var assemblyString = $"{(string.IsNullOrEmpty(_assemblyName) ? ", Assembly-CSharp" : ", ")}{_assemblyName}";

        type = Type.GetType($"{namespaceString}{_className}{assemblyString}");

        return type != null;
    }

    #endregion
}

}
