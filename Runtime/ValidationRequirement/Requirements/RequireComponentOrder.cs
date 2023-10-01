using System;
using System.Collections.Generic;
using SerializeReferenceDropdown.Runtime;
using UnityEngine;
using ValidationRequirement.Requirements.ComponentOrder;

namespace ValidationRequirement.Requirements
{

[Serializable]
[SerializeReferenceDropdownName("Component Order")]
public class RequireComponentOrder : ValidationRequirementMetaData, IValidationRequirement
{
    [SerializeField] private ComponentOrderConfig _config;
    
    public ValidationState Validate(GameObject gameObject, out List <ValidationError> errors)
    {
        errors = new List <ValidationError>();
        
        var components = gameObject.GetComponents<Component>();

        foreach (Component component in components)
        {
            Debug.Log(component.GetType().Name);
        }

        return ValidationState.Unknown;
    }

    public void OnValidate()
    {
        TryInitialize();
    }
    
    protected override void Initialize()
    {
        
    }
}

}
