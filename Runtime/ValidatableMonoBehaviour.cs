using System.Collections.Generic;
using SerializeReferenceDropdown.Runtime;
using UnityEngine;

[RequireComponent(typeof(ValidatableMonoBehaviourStatus))]
public class ValidatableMonoBehaviour : MonoBehaviour
{
    [SerializeReferenceDropdown] [SerializeReference] private List<IValidationRequirement> _requirements;

    private ValidatableMonoBehaviourStatus _status;

    // TODO works but validation of the whole gameObject is not working
    // TODO when changing the position the validation should update immediatly
    
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

    public ValidationState Validate()
    {
        var state = ValidationState.Ok;

        foreach (IValidationRequirement requirement in _requirements)
        {
            ValidationState requirementState = requirement.Validate(gameObject);

            if (requirementState > state)
                state = requirementState;
        }

        return state;
    }
}