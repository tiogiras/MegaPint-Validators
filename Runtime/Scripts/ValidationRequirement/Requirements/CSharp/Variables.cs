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
[SerializeReferenceDropdownName("C#/Variables", typeof(Variables), true, -40)]
public class Variables : ScriptableValidationRequirement
{
    [HideInInspector] public string name;

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

    private bool TryGetClassComponent(GameObject gameObject, Type type, out Component component)
    {
        component = gameObject.GetComponent(type);
        
        if (component == null)
            Debug.LogWarning("No component of the given type could be found on the gameObject. Make sure to add the component to the gameObject.");

        return component != null;
    }

    private bool TryGetClassType(out Type type)
    {
        type = null;

        if (string.IsNullOrEmpty(_classNamespace))
            return false;

        if (string.IsNullOrEmpty(_className))
            return false;

        type = Type.GetType($"{_classNamespace}.{_className}, {_assemblyName}");

        return type != null;
    }

    #endregion
}

}
