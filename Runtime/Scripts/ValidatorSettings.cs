using System.Collections.Generic;
using System.Linq;
using MegaPint.SerializeReferenceDropdown.Runtime;
using MegaPint.ValidationRequirement;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MegaPint
{

/// <summary> Settings of a <see cref="ValidatableMonoBehaviour" /> </summary>
[CreateAssetMenu(fileName = "Settings", menuName = "MegaPint/Validators/Validator Settings", order = 1)]
public class ValidatorSettings : ScriptableObject
{
    [HideInInspector]
    public List <string> initializedRequirements;

    [SerializeReferenceDropdown] [SerializeReference]
    private List <IValidationRequirement> _requirements;

    #region Unity Event Functions

    private void OnValidate()
    {
        foreach (IValidationRequirement requirement in _requirements)
            requirement?.OnValidate(this);

        List <string> cleanedInitializedRequirements = (from requirement in _requirements
                                                        where requirement != null
                                                        select requirement.GetType().ToString()
                                                        into typeName
                                                        where initializedRequirements.Contains(typeName)
                                                        select initializedRequirements[
                                                            initializedRequirements.IndexOf(typeName)]).ToList();

        initializedRequirements = cleanedInitializedRequirements;
    }

    #endregion

    #region Public Methods

    /// <summary> Check if the requirement was initialized on this behaviour </summary>
    /// <param name="requirement"> Targeted requirement </param>
    /// <returns> Initialization status of the targeted requirement </returns>
    public bool IsInitialized(IValidationRequirement requirement)
    {
        initializedRequirements ??= new List <string>();

        return initializedRequirements.Contains(requirement.GetType().ToString());
    }

    /// <summary> Callback on initialization of a requirement </summary>
    /// <param name="requirement"> Targeted requirement </param>
    public void OnRequirementInitialization(IValidationRequirement requirement)
    {
        initializedRequirements ??= new List <string>();

        if (initializedRequirements.Contains(requirement.GetType().ToString()))
            return;

        initializedRequirements.Add(requirement.GetType().ToString());
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);  
#endif
    }

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
