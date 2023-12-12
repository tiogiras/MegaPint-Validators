using System.Collections.Generic;
using SerializeReferenceDropdown.Runtime;
using UnityEngine;
using ValidationRequirement;

[CreateAssetMenu(fileName = "Settings", menuName = "MegaPint/ValidatorSettings", order = 1)]
public class ValidatorSettings : ScriptableObject
{
    [SerializeReferenceDropdown] [SerializeReference]
    public List <IValidationRequirement> _requirements;

    public void SetRequirements(List <IValidationRequirement> requirements)
    {
        _requirements = requirements;
    }
}