using System.Collections.Generic;
using SerializeReferenceDropdown.Runtime;
using UnityEngine;
using ValidationRequirement;

[RequireComponent(typeof(ValidatableMonoBehaviourStatus))]
public abstract class ValidatableMonoBehaviour : MonoBehaviour
{
    [SerializeReferenceDropdown] [SerializeReference]
    private List <IValidationRequirement> _requirements;

    private ValidatableMonoBehaviourStatus _status;

    private void OnValidate()
    {
        if (_status == null)
        {
            _status = GetComponent <ValidatableMonoBehaviourStatus>();
            _status.AddValidatableMonoBehaviour(this);
        }

        if (_requirements is not {Count: > 0})
            return;

        foreach (IValidationRequirement requirement in _requirements)
        {
            requirement?.OnValidate();
        }

        _status.ValidateStatus();
    }

    public ValidationState Validate(out List<InvalidBehaviour.ValidationError> errors)
    {
        var state = ValidationState.Ok;
        errors = new List <InvalidBehaviour.ValidationError>();

        if (_requirements is not {Count: > 0})
            return state;
        
        foreach (IValidationRequirement requirement in _requirements)
        {
            if (requirement == null)
                continue;

            ValidationState requirementState = requirement.Validate(gameObject, out List <InvalidBehaviour.ValidationError> additionalErrors);
            
            if (additionalErrors.Count > 0)
                errors.AddRange(additionalErrors);
            
            if (requirementState > state)
                state = requirementState;
        }

        return state;
    }
}
