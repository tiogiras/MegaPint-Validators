using System.Collections.Generic;
using MegaPint.SerializeReferenceDropdown.Runtime;
using MegaPint.ValidationRequirement;
using UnityEngine;

namespace MegaPint
{

/// <summary> Settings of a <see cref="ValidatableMonoBehaviour" /> </summary>
[CreateAssetMenu(fileName = "Settings", menuName = "MegaPint/Validators/Validator Settings", order = 1)]
public class ValidatorSettings : ScriptableObject
{
    [SerializeReferenceDropdown] [SerializeReference]
    private List <IValidationRequirement> _requirements;

    #region Public Methods

    public List <IValidationRequirement> Requirements()
    {
        return _requirements;
    }

    /// <summary> Overwrite the saved requirements </summary>
    /// <param name="requirements"> New requirements </param>
    public void SetRequirements(List <IValidationRequirement> requirements)
    {
        _requirements = requirements;
    }

    #endregion
}

}
