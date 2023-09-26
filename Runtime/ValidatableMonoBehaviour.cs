using System.Collections.Generic;
using SerializeReferenceDropdown.Runtime;
using UnityEngine;
using ValidationRequirement;

[RequireComponent(typeof(ValidatableMonoBehaviourStatus))]
public class ValidatableMonoBehaviour : MonoBehaviour
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

    public ValidationState Validate()
    {
        var state = ValidationState.Ok;

        if (_requirements is not {Count: > 0})
            return state;
        
        foreach (IValidationRequirement requirement in _requirements)
        {
            if (requirement == null)
                continue;

            ValidationState requirementState = requirement.Validate(gameObject);

            if (requirementState > state)
                state = requirementState;
        }

        return state;
    }
}
