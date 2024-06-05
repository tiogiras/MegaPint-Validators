using System;
using System.Collections.Generic;
using MegaPint.SerializeReferenceDropdown.Runtime;
using UnityEngine;

namespace MegaPint.ValidationRequirement.Requirements
{

/// <summary> Validation requirement requiring all child <see cref="ValidatableMonoBehaviour" /> to be valid </summary>
[Serializable]
[SerializeReferenceDropdownName("Child Validation", -29)]
public class RequireChildrenValidation : ScriptableValidationRequirement
{
    #region Protected Methods

    protected override void OnInitialization()
    {
    }

    protected override void Validate(GameObject gameObject)
    {
        List <ValidatableMonoBehaviourStatus> behaviourStates = CollectBehaviourStates(
            gameObject.transform,
            out List <GameObject> invalidGameObjects,
            out ValidationState severity);

        if (behaviourStates.Count == 0)
            return;

        var myStatus = gameObject.GetComponent <ValidatableMonoBehaviourStatus>();

        foreach (ValidatableMonoBehaviourStatus behaviourStatus in behaviourStates)
            myStatus.invalidBehaviours.AddRange(behaviourStatus.invalidBehaviours);

        var errorText = $"Errors found under the following gameObjects:\n{string.Join("\n", invalidGameObjects)}";

        AddError("Invalid monoBehaviours in children", errorText, severity, null);
    }

    #endregion

    #region Private Methods

    /// <summary> Collect all child behaviours </summary>
    /// <param name="transform"> Root transform component </param>
    /// <param name="invalidGameObjects"> Output of all invalid gameObjects </param>
    /// <param name="severity"> Maximum found severity level </param>
    /// <returns> All status behaviours </returns>
    private List <ValidatableMonoBehaviourStatus> CollectBehaviourStates(
        Transform transform,
        out List <GameObject> invalidGameObjects,
        out ValidationState severity)
    {
        List <ValidatableMonoBehaviourStatus> behaviourStates = new();

        severity = ValidationState.Ok;
        invalidGameObjects = new List <GameObject>();

        foreach (Transform tr in transform)
        {
            ValidatableMonoBehaviourStatus[] states = tr.GetComponentsInChildren <ValidatableMonoBehaviourStatus>();

            foreach (ValidatableMonoBehaviourStatus behaviourStatus in states)
            {
                behaviourStatus.ValidateStatus();

                if (behaviourStatus.State <= ValidationState.Ok)
                    continue;

                invalidGameObjects.Add(behaviourStatus.gameObject);

                if (behaviourStatus.State > severity)
                    severity = behaviourStatus.State;
            }

            behaviourStates.AddRange(states);
        }

        return behaviourStates;
    }

    #endregion
}

}
