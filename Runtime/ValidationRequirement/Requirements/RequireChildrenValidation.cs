using System;
using System.Collections.Generic;
using System.Linq;
using SerializeReferenceDropdown.Runtime;
using UnityEngine;

namespace ValidationRequirement.Requirements
{

[Serializable]
[SerializeReferenceDropdownName("Require Children Validation")]
public class RequireChildrenValidation : ValidationRequirementMetaData, IValidationRequirement
{
    public ValidationState Validate(GameObject gameObject, out List <ValidationError> errors)
    {
        errors = new List <ValidationError>();

        List <ValidatableMonoBehaviourStatus> behaviourStates = new();

        var highestState = ValidationState.Unknown;
        
        foreach (Transform tr in gameObject.transform)
        {
            ValidatableMonoBehaviourStatus[] states = tr.GetComponentsInChildren <ValidatableMonoBehaviourStatus>();

            foreach (ValidatableMonoBehaviourStatus behaviourStatus in states)
            {
                if (behaviourStatus.State > highestState)
                    highestState = behaviourStatus.State;
            }
            
            behaviourStates.AddRange(states);
        }

        if (behaviourStates.Count <= 0)
            return ValidationState.Ok;

        var myStatus = gameObject.GetComponent<ValidatableMonoBehaviourStatus>();

        foreach (ValidatableMonoBehaviourStatus behaviourStatus in behaviourStates)
        {
            myStatus.invalidBehaviours.AddRange(behaviourStatus.invalidBehaviours);
        }

        return highestState;
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
