#if UNITY_EDITOR
using System.Collections.Generic;
using SerializeReferenceDropdown.Runtime;
using UnityEngine;
using ValidationRequirement;

[CreateAssetMenu(fileName = "Settings", menuName = "MegaPint/Validators/ValidatorSettings", order = 1)]
public class ValidatorSettings : ScriptableObject
{
    [SerializeReferenceDropdown] [SerializeReference]
    public List <IValidationRequirement> requirements;

    #region Public Methods

    public void SetRequirements(List <IValidationRequirement> newRequirements)
    {
        requirements = newRequirements;
    }

    #endregion
}
#endif
