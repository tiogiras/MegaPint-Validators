﻿using System;
using MegaPint.SerializeReferenceDropdown.Runtime;
using UnityEngine;

namespace MegaPint.ValidationRequirement.Requirements.GameObjectValidation
{

/// <summary> Validation requirement requiring a specific active state on a gameObject </summary>
[Serializable]
[SerializeReferenceDropdownName("GameObject/Active", typeof(RequireGameObjectActive), -10, 0)]
public class RequireGameObjectActive : ScriptableValidationRequirement
{
    [SerializeField] [Tooltip("The state the gameObject should have")]
    private bool _targetState;

    #region Protected Methods

    protected override void OnInitialization()
    {
        _targetState = true;
    }

    protected override void Validate(GameObject gameObject)
    {
        var isValid = gameObject.activeSelf == _targetState;

        if (isValid)
            return;

        var errorMessage = $"GameObject should be {(_targetState ? "active" : "inactive")}";

        AddError(
            "Incorrect active state",
            errorMessage,
            ValidationState.Warning,
            FixAction);
    }

    #endregion

    #region Private Methods

    /// <summary> Fix the status of the gameObject </summary>
    /// <param name="gameObject"> Targeted gameObject </param>
    private void FixAction(GameObject gameObject)
    {
        gameObject.SetActive(_targetState);
    }

    #endregion
}

}
