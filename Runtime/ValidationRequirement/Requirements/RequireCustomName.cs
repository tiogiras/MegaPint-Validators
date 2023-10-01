﻿using System;
using System.Text.RegularExpressions;
using SerializeReferenceDropdown.Runtime;
using UnityEngine;

namespace ValidationRequirement.Requirements
{

[Serializable]
[SerializeReferenceDropdownName("Regex Naming Validation")]
public class RequireCustomName : ScriptableValidationRequirement
{
    [SerializeField] [Tooltip("Regex expression after which the name of the gameObject is validated")]
    private string _regexPattern;

    #region Protected Methods

    protected override void OnInitialization()
    {
    }

    protected override void Validate(GameObject gameObject)
    {
        if (Regex.IsMatch(gameObject.name, _regexPattern))
            return;

        AddError(
            "Invalid gameObject naming",
            "The name of the gameObject does not correspond to the specified regexPattern",
            ValidationState.Warning,
            null);
    }

    #endregion
}

}
