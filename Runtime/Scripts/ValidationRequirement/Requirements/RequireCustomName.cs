using System;
using System.Text.RegularExpressions;
using MegaPint.SerializeReferenceDropdown.Runtime;
using UnityEngine;

namespace MegaPint.ValidationRequirement.Requirements
{

/// <summary> Validation requirement requiring a custom name based on a defined regex ruleset </summary>
[Serializable]
[ValidationRequirementTooltip("With this requirement you can specify custom naming rules for the gameObject based on a regex pattern.")]
[ValidationRequirement("Regex Naming Validation", typeof(RequireCustomName), -8)]
internal class RequireCustomName : ScriptableValidationRequirement
{
    [SerializeField] [Tooltip("Regex expression after which the name of the gameObject is validated")]
    [TextArea] private string _regexPattern;

    #region Protected Methods

    protected override void OnInitialization()
    {
    }

    protected override void Validate(GameObject gameObject)
    {
        AddErrorIf(
            !Regex.IsMatch(gameObject.name, _regexPattern),
            "Invalid gameObject naming",
            "The name of the gameObject does not correspond to the specified regexPattern",
            ValidationState.Warning,
            null);
    }

    #endregion
}

}
