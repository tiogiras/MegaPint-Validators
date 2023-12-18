/*
using System.Collections.Generic;
using SerializeReferenceDropdown.Runtime;
using UnityEngine;
using ValidationRequirement;
using ValidationRequirement.Requirements;
using ValidationRequirement.Requirements.Transform;

[CreateAssetMenu(fileName = "Settings", menuName = "MegaPint/Validators/Validator Settings", order = 1)]
public class ValidatorSettings : ScriptableObject
{
    [SerializeReferenceDropdown] [SerializeReference]
    public List <IValidationRequirement> requirements = new List <IValidationRequirement>(new []{new RequireChildrenValidation()});

    #region Public Methods

    public void SetRequirements(List <IValidationRequirement> newRequirements)
    {
        // TODO gameObject must be reset and initialized too but keep settings?

        Debug.Log("Set Requirements");
        
        /*requirements.Clear();
        
        foreach (IValidationRequirement requirement in newRequirements)
        {
            requirements.Add(requirement);
        }#1#
    }

    #endregion
}
*/

using System.Collections.Generic;
using SerializeReferenceDropdown.Runtime;
using UnityEngine;
using ValidationRequirement;

[CreateAssetMenu(fileName = "Settings", menuName = "MegaPint/Validators/Validator Settings", order = 1)]
public class ValidatorSettings : ScriptableObject
{
    [SerializeReferenceDropdown] [SerializeReference]
    private List <IValidationRequirement> _requirements;

    public void SetRequirements(List <IValidationRequirement> requirements)
    {
        _requirements = requirements;
    }

    public List <IValidationRequirement> Requirements() => _requirements;
}
