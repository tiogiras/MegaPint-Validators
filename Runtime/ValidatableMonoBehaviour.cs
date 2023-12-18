using System.Collections.Generic;
using System.Linq;
using SerializeReferenceDropdown.Runtime;
using UnityEngine;
using UnityEngine.Serialization;
using ValidationRequirement;
using ValidationRequirement.Requirements;

[RequireComponent(typeof(ValidatableMonoBehaviourStatus))]
public abstract class ValidatableMonoBehaviour : MonoBehaviour
{
    public ValidatorSettings _importedSettings;
    
    [SerializeReferenceDropdown] [SerializeReference]
    public List <IValidationRequirement> _requirements;

    private ValidatableMonoBehaviourStatus _status;

    private List <IValidationRequirement> Requiremtents => _importedSettings == null ? _requirements : _importedSettings.requirements;

    public bool ValidatesChildren()
    {
        return Requiremtents.Any(requirement => requirement.GetType() == typeof(RequireChildrenValidation));
    }
    
    public void OnValidate()
    {
        if (_status == null)
        {
            _status = GetComponent <ValidatableMonoBehaviourStatus>();
            _status.AddValidatableMonoBehaviour(this);
        }

        if (Requiremtents is not {Count: > 0})
            return;

        foreach (IValidationRequirement requirement in Requiremtents)
        {
            requirement?.OnValidate();
        }

        _status.ValidateStatus();
    }

    public ValidationState Validate(out List<ValidationError> errors)
    {
        var state = ValidationState.Ok;
        errors = new List <ValidationError>();

        if (Requiremtents is not {Count: > 0})
            return state;
        
        foreach (IValidationRequirement requirement in Requiremtents)
        {
            if (requirement == null)
                continue;

            ValidationState requirementState = requirement.Validate(gameObject, out List <ValidationError> additionalErrors);
            
            if (additionalErrors.Count > 0)
                errors.AddRange(additionalErrors);
            
            if (requirementState > state)
                state = requirementState;
        }

        return state;
    }
}
