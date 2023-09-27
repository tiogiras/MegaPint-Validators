using System;
using System.Collections.Generic;
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

        var highestState = ValidationState.Ok;
        List <GameObject> invalidStates = new();
        
        foreach (Transform tr in gameObject.transform)
        {
            ValidatableMonoBehaviourStatus[] states = tr.GetComponentsInChildren <ValidatableMonoBehaviourStatus>();

            foreach (ValidatableMonoBehaviourStatus behaviourStatus in states)
            {
                behaviourStatus.ValidateStatus();
                
                if (behaviourStatus.State <= ValidationState.Ok)
                    continue;
                    
                invalidStates.Add(behaviourStatus.gameObject);
                
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

        var errorText = $"Errors found under the following gameObjects:\n{string.Join("\n", invalidStates)}";
        
        errors.Add(new ValidationError
        {
            errorName = "Invalid monoBehaviours in children",
            errorText = errorText,
            fixAction = null,
            gameObject = null,
            severity = highestState
        });
        
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
